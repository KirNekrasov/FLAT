using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FLAT.SyntaxAnalisys.FormalGrammar;

namespace FLAT.UnitTests.SyntaxAnalysis.FormalGrammar
{
    [TestClass]
    public class ContextFreeGrammarTests
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
        [ExpectedException(typeof(ArgumentException))]
        public void ContextFreeGrammarCreate()
        {
            var error = new ContextFreeGrammar(new Nonterminal("X"), prods);
        }

        [TestMethod]
        public void ContextFreeGrammarFirstSimple()
        {
            var list = new List<ContextFreeProduction>()
            {
                new ContextFreeProduction(A, B),
                new ContextFreeProduction(B, b)
            };

            var simple = new ContextFreeGrammar(A, list);

            var symbols = new List<Symbol>()
            {
                A, A, B, b
            };

            var expected = new HashSet<Symbol>()
            {
                b
            };

            var first = simple.First(symbols);

            Assert.IsFalse(expected.Except(first).Any());
        }

        [TestMethod]
        public void ContextFreeGrammarFirst()
        {
            var symbols = new List<Symbol>()
            {
                A
            };

            var expected = new HashSet<Symbol>()
            {
                a, b, c, new Empty()
            };

            Assert.IsTrue(expected.SetEquals(grammar.First(symbols)));

            symbols = new List<Symbol>()
            {
                A, B, b
            };

            expected = new HashSet<Symbol>()
            {
                a, b, c
            };

            Assert.IsTrue(expected.SetEquals(grammar.First(symbols)));

            symbols = new List<Symbol>()
            {
                b
            };

            expected = new HashSet<Symbol>()
            {
                b
            };

            Assert.IsTrue(expected.SetEquals(grammar.First(symbols)));
        }



        [TestMethod]
        public void ContextFreeGrammarFollow()
        {
            var expected = new HashSet<Symbol>()
            {
                a, new EndMarker()
            };

            Assert.IsTrue(expected.SetEquals(grammar.Follow(A)));

            expected = new HashSet<Symbol>()
            {
                a, new EndMarker()
            };

            Assert.IsTrue(expected.SetEquals(grammar.Follow(B)));

            expected = new HashSet<Symbol>()
            {
                a, new EndMarker()
            };

            Assert.IsTrue(expected.SetEquals(grammar.Follow(C)));
        }
    }
}
