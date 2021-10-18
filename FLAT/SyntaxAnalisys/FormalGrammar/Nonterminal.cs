using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLAT.SyntaxAnalisys.FormalGrammar
{
    public class Nonterminal : Symbol, IEquatable<Nonterminal>
    {
        public Nonterminal(String name) : base(name, Symbol.SymbolType.Nonterminal) { }


        public bool Equals(Nonterminal other)
        {
            if (other == null)
            {
                return false;
            }

            if (this == other)
            {
                return true;
            }

            return this.name == other.Name;
        }

        public override string ToString()
        {
            return "<" + this.name + ">";
        }
    }
}
