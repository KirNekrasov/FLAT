using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLAT.SyntaxAnalisys.FormalGrammar
{
    public class Terminal : Symbol, IEquatable<Terminal>
    {
        public Terminal(String name) : base(name, Symbol.SymbolType.Terminal) { }


        public bool Equals(Terminal other)
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
    }
}
