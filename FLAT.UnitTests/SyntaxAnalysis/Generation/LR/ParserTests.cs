using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FLAT.SyntaxAnalisys.FormalGrammar;
using FLAT.SyntaxAnalisys.LR.Generation;
using FLAT.SyntaxAnalisys;
using FLAT.LexicalAnalysis;

namespace FLAT.UnitTests.SyntaxAnalysis.Generation.LR
{
    [TestClass]
    public class ParserTests
    {
        Terminal plus;

        Terminal id;

        Terminal leftP;

        Terminal rightP;

        Terminal leftSqP;

        Terminal rightSqP;

        Nonterminal Axiom;

        Nonterminal E;

        Nonterminal T;

        List<ContextFreeProduction> prods;

        ContextFreeGrammar grammar;

        [TestInitialize()]
        public void Initialize()
        {
            plus = new Terminal("+");
            id = new Terminal("id");
            leftP = new Terminal("(");
            rightP = new Terminal(")");
            leftSqP = new Terminal("[");
            rightSqP = new Terminal("]");
            Axiom = new Nonterminal("E'");
            E = new Nonterminal("E");
            T = new Nonterminal("T");

            prods = new List<ContextFreeProduction>()
            {
                new ContextFreeProduction(Axiom, E), 
                new ContextFreeProduction(E, E, plus, T),
                new ContextFreeProduction(E, T),
                new ContextFreeProduction(T, leftP, E, rightP),
                new ContextFreeProduction(T, id),
                 new ContextFreeProduction(T, id, leftSqP, E, rightSqP),
            };

            grammar = new ContextFreeGrammar(Axiom, prods);
        }

        [TestMethod]
        public void ParserCreate()
        {
            var parser = new LRParser(grammar);
        }

        [TestMethod]
        public void ParserParse()
        {
            var parser = new LRParser(grammar);

            var tokens = new List<Token>()
            {
                new Token("id", null),
                new StringEndMarker()
            };

            var tree = parser.Parse(tokens);
            Assert.IsTrue(tree.Root.Name == prods[0].ToString());
            Assert.IsTrue(tree.Root.Descendants.First().Name == prods[2].ToString());
        }
    }
}
