using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

using FLAT.SyntaxAnalisys.FormalGrammar;
using FLAT.SyntaxAnalisys.LR.Generation;

namespace FLAT.UnitTests.SyntaxAnalysis.Generation.LR
{
    [TestClass]
    public class ConfiguratingSetFamilyTests
    {
        Terminal a;

        Terminal b;

        Terminal c;

        Nonterminal Axiom;

        Nonterminal A;

        Nonterminal B;

        Nonterminal C;

        List<ContextFreeProduction> prods;

        ContextFreeGrammar grammar;

        [TestInitialize()]
        public void Initialize()
        {
            a = new Terminal("a");
            b = new Terminal("b");
            c = new Terminal("c");

            Axiom = new Nonterminal("A'");
            A = new Nonterminal("A");
            B = new Nonterminal("B");
            C = new Nonterminal("C");

            prods = new List<ContextFreeProduction>()
            {
                new ContextFreeProduction(Axiom, A),

                new ContextFreeProduction(A, A, a),
                new ContextFreeProduction(A, C),
                new ContextFreeProduction(A),

                new ContextFreeProduction(B, b, B),
                new ContextFreeProduction(B, C),
                new ContextFreeProduction(B, A),

                new ContextFreeProduction(C, c),
                new ContextFreeProduction(C, A),
                new ContextFreeProduction(C, B)
            };

            grammar = new ContextFreeGrammar(Axiom, prods);
        }

        [TestMethod]
        public void ConfiguratingSetFamilyCreate()
        {
            var family = new ConfiguratingSetFamily(grammar);
        }

        [TestMethod]
        public void ConfiguratingSetFamilyFamily()
        {
            var family = new ConfiguratingSetFamily(grammar);

            // With extended.
            Assert.IsTrue(family.StartConfiguratingSet.Closure.Count == 10);

            Assert.IsTrue(family.Family.Count == 9);

            var set = family.Family;

            var config = new Configuration(prods[4]);

            config.MoveMarkToNextSymbol();

            // From B -> b.B
            var configSet = new ConfiguratingSet(grammar, config);

            Assert.IsTrue(set.Contains(configSet));

            var reference = set.First(cs => cs.Equals(configSet));

            Assert.IsTrue(reference.Successors[b] == reference);

            Assert.IsTrue(family.StartConfiguratingSet.Successors[b] == reference);
        }
    }
}
