using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLAT.SyntaxAnalisys.FormalGrammar
{
    public static class GrammarExtensions
    {
        public static IContextFreeGrammar ExtendGrammar(this IContextFreeGrammar grammar)
        {
            IContextFreeGrammar result = grammar;

            var axiomProds = grammar.ProductionsByLeft(grammar.Axiom);

            if (axiomProds.Count() > 1)
            {
                var maxLength = grammar.Nonterminals.Max(nonterm => nonterm.Name.Length);
                var random = new Random();
                var newAxiom = new Nonterminal(new String(Enumerable.Range(0, maxLength + 1).Select(_ => (char)random.Next('A', 'Z')).ToArray()));

                var newProds = grammar.Productions.ToList();
                newProds.Add(new ContextFreeProduction(newAxiom, grammar.Axiom));

                result = new ContextFreeGrammar(newAxiom, newProds);
            }

            return result;
        }

        public static IContextFreeGrammar ExtendGrammar(this IContextFreeGrammar grammar, String newAxiomName)
        {
            IContextFreeGrammar result = grammar;

            var axiomProds = grammar.ProductionsByLeft(grammar.Axiom);

            if (axiomProds.Count() > 1)
            {
                var newAxiom = new Nonterminal(newAxiomName);

                var newProds = grammar.Productions.ToList();
                newProds.Add(new ContextFreeProduction(newAxiom, grammar.Axiom));

                result = new ContextFreeGrammar(newAxiom, newProds);
            }

            return result;
        }
    }
}
