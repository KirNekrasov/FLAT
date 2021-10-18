using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FLAT.LexicalAnalysis;
using FLAT.Resources;
using FLAT.SyntaxAnalisys.FormalGrammar;
using FLAT.SyntaxAnalisys.LR;
using FLAT.SyntaxAnalisys.LR.Generation;

namespace FLAT.SyntaxAnalisys
{
    public class LRParser : IParser
    {
        private bool doParsing;

        private ConfiguratingSetFamily family;

        private IContextFreeGrammar grammar;

        private ParseTree last;

        private Stack<int> states;

        private Stack<ParseTreeNode> nodes;

        private Table table;

        private Queue<Token> queue;


        public LRParser(IContextFreeGrammar grammar)
        {
            if (grammar == null)
            {
                throw new ArgumentNullException("grammar", Strings.Grammar + Strings.NullException);
            }

            this.family = new ConfiguratingSetFamily(grammar);

            this.grammar = family.ExtendedGrammar;

            this.GenerateTable();

            this.states = new Stack<int>();
            this.states.Push(0);

            this.nodes = new Stack<ParseTreeNode>();
        }


        private void GenerateTable()
        {
            this.table = this.InitTable();

            var states = InitStatesDictionary();

            foreach (var state in states)
            {
                var stateNumber = state.Value;
                var stateConfigSet = state.Key;

                this.GenerateShifts(states, stateNumber, stateConfigSet);
                this.GenerateReduces(stateNumber, stateConfigSet);
                this.GenerateGoTos(states, stateNumber, stateConfigSet);
            }
        }

        private Table InitTable()
        {
            var symbols = this.grammar.Terminals.Cast<Symbol>().Concat(this.grammar.Nonterminals).ToList();
            symbols.Add(new EndMarker());

            var table = new Table(this.family.Family.Count, symbols);

            return table;
        }

        private Dictionary<ConfiguratingSet, int> InitStatesDictionary()
        {
            var states = new Dictionary<ConfiguratingSet, int>();

            states[this.family.StartConfiguratingSet] = 0;

            var i = 0;
            var allConfigsExcludeStart = this.family.Family.Where(configSet => !configSet.Equals(this.family.StartConfiguratingSet));
            foreach (var configSet in allConfigsExcludeStart)
            {
                states[configSet] = ++i;
            }

            return states;
        }

        private void GenerateShifts(Dictionary<ConfiguratingSet, int> states, int stateNumber, ConfiguratingSet stateConfigSet)
        {
            var terminalMarkedSuccessors = stateConfigSet.Successors.Where(pair => pair.Key.IsTerminal());
            foreach (var pair in terminalMarkedSuccessors)
            {
                var markedTerminal = pair.Key;
                var successor = pair.Value;

                if (this.table[stateNumber, markedTerminal] == null)
                {
                    this.table[stateNumber, markedTerminal] = new Shift(states[successor]);
                }
                else
                {
                    throw new InvalidOperationException(Strings.ConflictException);
                }
            }
        }

        private void GenerateReduces(int stateNumber, ConfiguratingSet stateConfigSet)
        {
            var endMarkedConfigs = stateConfigSet.Closure.Where(config => config.IsMarkAtEnd());
            foreach (var config in endMarkedConfigs)
            {
                var leftNontermOfConfigProduction = config.Production.Left;
                foreach (var follow in this.grammar.Follow(leftNontermOfConfigProduction))
                {
                    if (this.table[stateNumber, follow] == null)
                    {
                        if (config.Production.Left.Equals(this.grammar.Axiom))
                        {
                            this.table[stateNumber, follow] = new Valid();
                        }
                        else
                        {
                            this.table[stateNumber, follow] = new Reduce(config.Production);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(Strings.ConflictException);
                    }
                }
            }
        }

        private void GenerateGoTos(Dictionary<ConfiguratingSet, int> states, int stateNumber, ConfiguratingSet stateConfigSet)
        {
            var nonterminalMarkedSuccessors = stateConfigSet.Successors.Where(pair => pair.Key.IsNonterminal());

            foreach (var successor in nonterminalMarkedSuccessors)
            {
                var nonterm = successor.Key;
                var configSet = successor.Value;

                if (this.table[stateNumber, nonterm] == null)
                {
                    this.table[stateNumber, nonterm] = new GoTo(states[configSet]);
                }
                else
                {
                    throw new InvalidOperationException(Strings.ConflictException);
                }
            }
        }


        /*public ParseTree Parse(IEnumerable<Token> tokens, IDictionary<String, Terminal> tokenClassToTerminalMap)
        {

        }*/

        public ParseTree Parse(IEnumerable<Token> tokens)
        {
            ValidateTokens(tokens);

            this.nodes.Clear();

            this.states.Clear();
            this.states.Push(0);

            this.doParsing = true;

            this.queue = new Queue<Token>(tokens);
            while (this.queue.Any())
            {
                if (this.doParsing)
                {
                    var token = this.queue.Peek();
                    var symbol = token.IsEnd() ? (Symbol)new EndMarker() : (Symbol)new Terminal(token.Name);

                    var action = this.table[this.states.Peek(), symbol];
                    if (action == null)
                    {
                        this.last = null;
                        throw new ArgumentException(Strings.InvalidTokensException);
                    }
                    else
                    {
                        action.Accept(this, token);
                    }
                }
                else
                {
                    break;
                }
            }

            if ((this.queue.Count > 1) && !this.queue.Peek().IsEnd())
            {
                this.last = null;
                throw new ArgumentException(Strings.InvalidTokensException);
            }

            return this.last;
        }

        private void ValidateTokens(IEnumerable<Token> tokens)
        {
            if (tokens == null)
            {
                throw new ArgumentNullException("tokens", Strings.Tokens + Strings.NullException);
            }

            if (!tokens.Any())
            {
                throw new ArgumentException(Strings.Tokens + Strings.EmptyException, "tokens");
            }

            if (!tokens.Last().IsEnd())
            {
                throw new ArgumentException(Strings.Tokens + Strings.NoStringEndMarker, "tokens");
            }

            var except = tokens.Select(token => token.Name).Except(this.grammar.Terminals.Select(term => term.Name));
            if ((except.Count() > 1) && (except.First() != ""))
            {
                throw new ArgumentException(Strings.InvalidTokensException);
            }
        }


        internal void Visit(GoTo goTo, Token token)
        {
            this.states.Push(goTo.GoToState);
        }

        internal void Visit(Valid valid, Token token)
        {
            this.doParsing = false;
            var axiom = this.grammar.ProductionsByLeft(this.grammar.Axiom).First();
            var root = new ParseTreeNode(axiom.ToString(), null, this.nodes.Pop());
            this.last = new ParseTree(root);
        }

        internal void Visit(Shift shift, Token token)
        {
            this.states.Push(shift.GoToState);
            this.nodes.Push(new ParseTreeNode(token.Name, token.Attribute, null));
            this.queue.Dequeue();
        }

        internal void Visit(Reduce reduce, Token token)
        {
            var descendants = new LinkedList<ParseTreeNode>();

            foreach (var symbol in reduce.Production.Right)
            {
                this.states.Pop();
                descendants.AddFirst(this.nodes.Pop());
            }

            this.nodes.Push(new ParseTreeNode(reduce.Production.ToString(), null, descendants));

            this.table[this.states.Peek(), reduce.Production.Left].Accept(this, token);
        }
    }
}
