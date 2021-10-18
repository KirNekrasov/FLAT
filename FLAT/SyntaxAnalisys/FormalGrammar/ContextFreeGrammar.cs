using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FLAT.Resources;

namespace FLAT.SyntaxAnalisys.FormalGrammar
{
    public class ContextFreeGrammar : IContextFreeGrammar, IGrammar
    {
        private Nonterminal axiom;

        private Dictionary<Nonterminal, HashSet<Symbol>> first;

        private Dictionary<Nonterminal, HashSet<Symbol>> follow;

        private IEnumerable<Nonterminal> nonterminals;

        private IEnumerable<IContextFreeProduction> productions;

        private ILookup<Nonterminal, IContextFreeProduction> productionsGroupedByLeft;

        private IEnumerable<Terminal> terminals;

        
        public ContextFreeGrammar(Nonterminal axiom, IEnumerable<IContextFreeProduction> productions)
        {
            if (axiom == null)
            {
                throw new ArgumentNullException("axiom", Strings.Axiom + Strings.NullException);
            }

            if (productions == null)
            {
                throw new ArgumentNullException("productions", Strings.Productions + Strings.NullException);
            }

            if (!productions.Any())
            {
                throw new ArgumentException(Strings.Productions + Strings.EmptyException, "productions");
            }

            var axioms = productions.Where(p => p.Left == axiom);

            if (!axioms.Any())
            {
                throw new ArgumentException(Strings.NoAxiomInProductionsException);
            }

            if (axioms.Count() > 1)
            {
                throw new ArgumentException(Strings.MoreThanOneAxiomProduction);
            }

            this.axiom = axiom;

            this.productions = ImmutableList.CreateRange(productions.Distinct());

            if (this.Nonterminals.Select(nonterm => nonterm.Name).Intersect(this.Terminals.Select(term => term.Name)).Any())
            {
                throw new ArgumentException(Strings.TerminalAndNonterminalSetsIntersectExciption);
            }
        }

        public ContextFreeGrammar(Nonterminal axiom, params IContextFreeProduction[] productions)
            : this(axiom, (IEnumerable<IContextFreeProduction>)productions) { }

        
        public Nonterminal Axiom
        {
            get { return this.axiom; }
        }

        public IEnumerable<Nonterminal> Nonterminals
        {
            get
            {
                if (this.nonterminals == null)
                {
                    var fromRight = new HashSet<Nonterminal>(
                        this.productions.SelectMany(p => p.Right).Where(s => s.IsNonterminal()).Cast<Nonterminal>()
                    );

                    NonterminalsValidation(fromRight);

                    fromRight.Add(this.axiom);

                    this.nonterminals = fromRight.ToImmutableList();
                }

                return this.nonterminals;
            }
        }

        public IEnumerable<IContextFreeProduction> Productions
        {
            get { return this.productions; }
        }

        IEnumerable<IProduction> IGrammar.Productions
        {
            get { return this.productions; }
        }

        public IEnumerable<Terminal> Terminals
        {
            get
            {
                if (this.terminals == null)
                {
                    this.terminals = this.productions.SelectMany(p => p.Right).Where(s => s.IsTerminal())
                        .Distinct().Cast<Terminal>().ToImmutableList();
                }

                return this.terminals;
            }
        }

        
        public IEnumerable<Symbol> First(IEnumerable<Symbol> symbols)
        {
            this.SymbolsValidationForFirst(symbols);

            if (this.first == null)
            {
                this.FirstCompute();
            }

            var result = new HashSet<Symbol>();

            var firstSymbol = symbols.First();

            if (firstSymbol.IsTerminal() || firstSymbol.IsEmpty())
            {
                result.Add(firstSymbol);
            }
            else if (firstSymbol.IsNonterminal())
            {
                result = this.first[(Nonterminal)firstSymbol];

                var tempSymbols = new List<Symbol>(symbols.Skip(1));

                var empty = (Symbol)new Empty();

                if (result.Contains(empty) && tempSymbols.Any())
                {
                    result.Remove(empty);

                    result.UnionWith(this.First(tempSymbols));
                }
            }
            else
            {
                throw new ArgumentException(Strings.Unknown + " " + Strings.Symbol.ToLower(), "symbols");
            }

            return result;
        }

        private void FirstCompute()
        {
            // Init
            this.ProductionsByLeftCompute();

            var groupedProductions = this.productionsGroupedByLeft.ToDictionary(
                group => group.Key, group => new HashSet<IContextFreeProduction>(group)
            );
            var nontermsWithFirstNonterms = new HashSet<Nonterminal>(this.Nonterminals);

            FirstComputeNontermsWithFirstNonterms(groupedProductions, nontermsWithFirstNonterms);

            var nullables = new HashSet<Nonterminal>();

            while (nontermsWithFirstNonterms.Count != 0)
            {
                FirstComputeReplaceNullables(groupedProductions, nullables);
                FirstComputeRemoveLeftRecursion(groupedProductions);
                FirstComputeNontermsWithFirstNonterms(groupedProductions, nontermsWithFirstNonterms);

                if (nontermsWithFirstNonterms.Count == 0)
                {
                    break;
                }

                Nonterminal nonterm = FirstComputeSelectNontermToReplace(groupedProductions);
                if (nonterm == null)
                {
                    break;
                }

                FirstComputeReplaceNontermInProductions(groupedProductions, nontermsWithFirstNonterms, nonterm);
                FirstComputeNontermsWithFirstNonterms(groupedProductions, nontermsWithFirstNonterms);
            }

            this.first = groupedProductions.ToDictionary(pair => pair.Key, pair => new HashSet<Symbol>(pair.Value.Select(prod => prod.Right.First())));
        }

        public IEnumerable<Symbol> Follow(Nonterminal symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException("symbol", Strings.Nonterminal + Strings.NullException);
            }

            if (!this.Nonterminals.Contains(symbol))
            {
                throw new ArgumentException(Strings.Unknown + symbol, "symbol");
            }

            if (this.follow == null)
            {
                this.FollowCompute();
            }

            return this.follow[symbol].ToImmutableList();
        }

        private void FollowCompute()
        {
            this.follow = this.Nonterminals.ToDictionary(n => n, n => new HashSet<Symbol>());
            var followDepends = this.Nonterminals.ToDictionary(n => n, n => new HashSet<Nonterminal>());
            FollowComputeInit(followDepends);

            while (followDepends.Count(pair => pair.Value.Any()) != 0)
            {
                Nonterminal toReplace = FollowComputeSelectNonterm(followDepends);
                FollowComputeFilter(followDepends, toReplace);
            }
        }

        private void FollowComputeFilter(Dictionary<Nonterminal, HashSet<Nonterminal>> followDepends, Nonterminal toReplace)
        {
            foreach (var depends in followDepends)
            {
                if (depends.Value.Contains(toReplace))
                {
                    depends.Value.Remove(toReplace);
                    foreach (var newDepend in followDepends[toReplace])
                    {
                        // No right rec.
                        if (!newDepend.Equals(depends.Key))
                        {
                            depends.Value.Add(newDepend);
                        }
                    }

                    this.follow[depends.Key].UnionWith(this.follow[toReplace]);
                }
            }
        }

        private static Nonterminal FollowComputeSelectNonterm(Dictionary<Nonterminal, HashSet<Nonterminal>> dependsOfNonterms)
        {
            var numberOfDependsNontermsHave = dependsOfNonterms.Select(
                pair => new
                {
                    Key = pair.Key,
                    Counter = pair.Value.Count
                })
                .OrderBy(a => a.Counter);

            var freq = dependsOfNonterms.ToDictionary(
                pair => pair.Key,
                pair => dependsOfNonterms.Values.SelectMany(set => set).Where(nonterm => nonterm.Equals(pair.Key)).Count()
            );

            if (numberOfDependsNontermsHave.All(a => a.Counter == 0))
            {
                return null;
            }
            else
            {
                return numberOfDependsNontermsHave.Where(a => freq[a.Key] != 0).First().Key;
            }
        }

        private void FollowComputeInit(Dictionary<Nonterminal, HashSet<Nonterminal>> dependsOfNonterms)
        {
            this.follow[this.axiom].Add(new EndMarker());
            foreach (var production in this.productions)
            {
                var right = production.Right.ToList();
                while (right.Any())
                {
                    var symbol = right.First();
                    right.RemoveAt(0);
                    if (symbol.IsNonterminal())
                    {
                        if (right.Count != 0)
                        {
                            var first = this.First(right);
                            this.follow[(Nonterminal)symbol].UnionWith(first.Where(s => !s.IsEmpty()));

                            if (first.Contains(new Empty()) && !symbol.Equals(production.Left))
                            {
                                dependsOfNonterms[(Nonterminal)symbol].Add(production.Left);
                            }
                        }
                        else
                        {
                            if (!symbol.Equals(production.Left))
                            {
                                dependsOfNonterms[(Nonterminal)symbol].Add(production.Left);
                            }
                        }
                    }
                }
            }
        }

        private static void FirstComputeNontermsWithFirstNonterms(Dictionary<Nonterminal, HashSet<IContextFreeProduction>> groupedProductions
            , HashSet<Nonterminal> nontermsWithFirstNonterms)
        {
            foreach (var group in groupedProductions)
            {
                if (group.Value.All(prod => prod.Right.First().IsTerminal() || prod.Right.First().IsEmpty()))
                {
                    nontermsWithFirstNonterms.Remove(group.Key);
                }
            }
        }
        
        private void NonterminalsValidation(HashSet<Nonterminal> fromRight)
        {
            var fromLeft = new HashSet<Nonterminal>(this.productions.Select(p => p.Left));
            var except = fromRight.Except(fromLeft).ToList();
            if (except.Count != 0)
            {
                var sb = new StringBuilder();

                foreach (var nonterm in except)
                {
                    sb.AppendFormat("<{0}> ", nonterm);
                }

                throw new InvalidOperationException(Strings.NoProductionsForNonterminalsException + sb);
            }
        }
        
        public IEnumerable<IContextFreeProduction> ProductionsByLeft(Nonterminal nonterminal)
        {
            if (this.productionsGroupedByLeft == null)
            {
                this.ProductionsByLeftCompute();
            }

            if (!this.productionsGroupedByLeft.Contains(nonterminal))
            {
                throw new ArgumentException(Strings.NoProductionsForNonterminalsException + nonterminal, "nonterminal");
            }

            return this.productionsGroupedByLeft[nonterminal];
        }

        private void ProductionsByLeftCompute()
        {
            this.productionsGroupedByLeft = this.productions.ToLookup(p => p.Left);
        }

        private static void FirstComputeRemoveLeftRecursion(Dictionary<Nonterminal, HashSet<IContextFreeProduction>> groupedProductions)
        {
            foreach (var group in groupedProductions.Values)
            {
                group.RemoveWhere(prod => prod.Right.First().Equals(prod.Left));
            }
        }

        private static void FirstComputeReplaceNontermInProductions(Dictionary<Nonterminal, HashSet<IContextFreeProduction>> groupedProductions
            , HashSet<Nonterminal> nontermsWithFirstNonterms
            , Nonterminal nonterm)
        {
            foreach (var group in groupedProductions)
            {
                if (nontermsWithFirstNonterms.Contains(group.Key) && !group.Key.Equals(nonterm))
                {
                    foreach (var production in group.Value.ToList())
                    {
                        var first = production.Right.First();
                        if (first.Equals(nonterm))
                        {
                            group.Value.Remove(production);
                            foreach (var prodToReplace in groupedProductions[nonterm])
                            {
                                var temp = new List<Symbol>(production.Right);
                                temp.RemoveAt(0);
                                if (!prodToReplace.IsEmpty())
                                {
                                    temp.InsertRange(0, prodToReplace.Right);
                                }

                                group.Value.Add(new ContextFreeProduction(production.Left, temp));
                            }
                        }
                    }
                }
            }
        }

        private void FirstComputeReplaceNullables(Dictionary<Nonterminal, HashSet<IContextFreeProduction>> groupedProductions
            , HashSet<Nonterminal> nullables)
        {
            nullables.UnionWith(this.productions.Where(prod => prod.IsEmpty()).Select(prod => prod.Left));

            var queue = new Queue<Nonterminal>(nullables);

            while (queue.Any())
            {
                var nullable = queue.Dequeue();
                foreach (var group in groupedProductions)
                {
                    var productionsToReplaceNullableNonterminal
                        = new Queue<IContextFreeProduction>(group.Value.Where(prod => prod.Right.Contains(nullable)));
                    while (productionsToReplaceNullableNonterminal.Any())
                    {
                        var toReplace = productionsToReplaceNullableNonterminal.Dequeue();
                        var toReplaceRight = toReplace.Right.ToList();
                        for (var i = 0; i < toReplaceRight.Count; ++i)
                        {
                            if (toReplaceRight[i].Equals(nullable))
                            {
                                var temp = new List<Symbol>(toReplaceRight);
                                temp.RemoveAt(i);

                                var newProd = new ContextFreeProduction(toReplace.Left, temp);
                                group.Value.Add(newProd);
                                if (newProd.Right.Contains(nullable))
                                {
                                    productionsToReplaceNullableNonterminal.Enqueue(newProd);
                                }
                            }
                        }
                    }

                    if (group.Value.Contains(new ContextFreeProduction(group.Key)) && !nullables.Contains(group.Key))
                    {
                        nullables.Add(group.Key);
                        queue.Enqueue(group.Key);
                    }
                }
            }
        }

        private static Nonterminal FirstComputeSelectNontermToReplace(Dictionary<Nonterminal, HashSet<IContextFreeProduction>> groupedProductions)
        {
            var countDependsForKey = groupedProductions.Select(
                pair => new
                {
                    Key = pair.Key,
                    Counter = pair.Value.Select(prod => prod.Right.First()).Where(s => s.IsNonterminal()).Distinct().Count()
                }
            ).OrderBy(p => p.Counter);

            var countFreq = groupedProductions.ToDictionary(
                pair => pair.Key,
                pair => groupedProductions.Values.SelectMany(set => set).Where(prod => prod.Right.First().Equals(pair.Key)).Count()
            );            

            return countDependsForKey.Where(a => countFreq[a.Key] != 0).First().Key;
        }

        private void SymbolsValidationForFirst(IEnumerable<Symbol> symbols)
        {
            if (symbols == null)
            {
                throw new ArgumentNullException(Strings.Symbols + Strings.NullException, "symbols");
            }

            if (!symbols.Any())
            {
                throw new ArgumentException("symbols", Strings.Symbols + Strings.EmptyException);
            }

            foreach (var symbol in symbols)
            {
                if (symbol.IsTerminal() && !this.Terminals.Contains(symbol))
                {
                    throw new ArgumentException(Strings.Unknown + " " + Strings.Symbol.ToLower() + ": " + symbol, "symbols");
                }
                else if (symbol.IsNonterminal() && !this.Nonterminals.Contains(symbol))
                {
                    throw new ArgumentException(Strings.Unknown + symbol, "symbols");
                }
                else if (symbol.IsEmpty() && (symbols.Count() != 1))
                {
                    throw new ArgumentException(Strings.UnexpectedEmptySymbolException, "symbols");
                }
            }
        }
    }
}
