using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FLAT.SyntaxAnalisys.FormalGrammar;
using FLAT.SyntaxAnalisys.LR.Generation;

namespace FLAT.UnitTests.SyntaxAnalysis.Generation.LR
{
    [TestClass]
    public class ConfiguratingSetTests
    {
        private Terminal a;

        private Terminal b;

        private Terminal c;

        private Nonterminal Axiom;

        private Nonterminal A;

        private Nonterminal B;

        private Nonterminal C;

        private List<ContextFreeProduction> prods;

        private ContextFreeGrammar grammar;

        private List<Configuration> configs;

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

            configs = prods.Take(1).Select(prod => new Configuration(prod)).ToList();
        }

        [TestMethod]
        public void ConfiguratingSetCreation()
        {
            var configSet = new ConfiguratingSet(grammar, configs);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConfiguratingSetNullCreation()
        {
            var configSet = new ConfiguratingSet(null, null);
        }

        [TestMethod]
        public void ConfiguratingSetClosure()
        {
            var configSet = new ConfiguratingSet(grammar, configs);

            var expected = prods.Select(prod => new Configuration(prod));
            Assert.IsTrue(configSet.Closure.SetEquals(expected));

            configSet = new ConfiguratingSet(grammar, configs.Take(1));
            Assert.IsTrue(configSet.Closure.SetEquals(expected));
        }

        [TestMethod]
        public void ConfiguratingSetSuccessors()
        {
            var configSet = new ConfiguratingSet(grammar, configs);
            Assert.IsTrue(configSet.Successors.Count == 5);
            Assert.IsTrue(configSet.Successors[A].CoreConfigurations.Count() == 4);

            var list = new List<Configuration>()
            {
                new Configuration(prods[0]),
                new Configuration(prods[1]),
                new Configuration(prods[6]),
                new Configuration(prods[8])
            };

            foreach (var config in list)
            {
                config.MoveMarkToNextSymbol();
            }

            var other = new ConfiguratingSet(grammar, list);
            Assert.IsTrue(configSet.Successors[A].Equals(other));
        }

        [TestMethod]
        public void ConfiguratingSetEquals()
        {
            var configSet = new ConfiguratingSet(grammar, configs);

            var other = new ConfiguratingSet(grammar, configs);

            Assert.IsTrue(configSet.Equals(other));

            Assert.IsTrue(configSet.Equals(configSet));

            Assert.IsFalse(configSet.Equals(null));
        }
    }
}
