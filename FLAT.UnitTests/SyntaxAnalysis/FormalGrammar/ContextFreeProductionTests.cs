using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FLAT.SyntaxAnalisys.FormalGrammar;

namespace FLAT.UnitTests.SyntaxAnalysis.FormalGrammar
{
    [TestClass]
    public class ContextFreeProductionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ContextFreeProductionCreate()
        {
            var prod = new ContextFreeProduction(new Nonterminal("A"));

            Assert.IsTrue(prod.IsEmpty());

            prod = new ContextFreeProduction(new Nonterminal("B"), new Terminal("a"));

            Assert.IsFalse(prod.IsEmpty());

            prod = new ContextFreeProduction(new Nonterminal("B"), new Empty(), new Terminal("a"));
        }

        [TestMethod]
        public void ContextFreeProductionEquals()
        {
            var first = new ContextFreeProduction(new Nonterminal("B"), new Terminal("a"));

            var second = new ContextFreeProduction(new Nonterminal("B"), new Terminal("a"));

            Assert.IsTrue(first.Equals(first));

            Assert.IsTrue(first.Equals(second));

            Assert.IsFalse(first.Equals(null));

            second = new ContextFreeProduction(new Nonterminal("B"));

            Assert.IsFalse(first.Equals(second));
        }
    }
}
