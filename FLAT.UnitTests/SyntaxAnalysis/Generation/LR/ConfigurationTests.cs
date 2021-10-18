using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FLAT.SyntaxAnalisys.FormalGrammar;
using FLAT.SyntaxAnalisys.LR.Generation;

namespace FLAT.UnitTests.Generation.LR
{
    [TestClass]
    public class ConfigurationTests
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
        public void ConfigurationCreate()
        {
            var config = new Configuration(prods[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConfigurationNullCreation()
        {
            var config = new Configuration((IContextFreeProduction)null);

            var config2 = new Configuration((Configuration)null);
        }

        [TestMethod]
        public void ConfigurationMarked()
        {
            var config = new Configuration(prods[0]);

            Assert.IsTrue(config.MarkedSymbol.Equals(prods[0].Right.First()));
        }

        [TestMethod]
        public void ConfigurationMarkedEmptyProduction()
        {
            var prod = new ContextFreeProduction(A);

            var config = new Configuration(prod);

            Assert.IsTrue(config.IsMarkAtEnd());

            Assert.IsTrue(config.MarkedSymbol == null);
        }

        [TestMethod]
        public void ConfigurationEquals()
        {
            var config = new Configuration(prods[0]);
            var config2 = new Configuration(prods[0]);

            Assert.IsTrue(config.Equals(config2));
            Assert.IsTrue(config.Equals(config));
            Assert.IsFalse(config.Equals(null));
        }

        [TestMethod]
        public void ConfigurationMoveToNext()
        {
            var config = new Configuration(prods[2]);

            Assert.IsFalse(config.IsMarkAtEnd());

            config.MoveMarkToNextSymbol();

            Assert.IsTrue(config.IsMarkAtEnd());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConfigurationMoveToNextOutOfRange()
        {
            var prod = new ContextFreeProduction(A);

            var config = new Configuration(prod);

            Assert.IsTrue(config.IsMarkAtEnd());

            config.MoveMarkToNextSymbol();
        }
    }
}
