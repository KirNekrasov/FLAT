using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLAT.SyntaxAnalisys.FormalGrammar
{
    internal class Empty : Symbol, IEquatable<Empty>
    {
        public Empty() : base(SymbolType.Empty) { }


        public bool Equals(Empty other)
        {
            if (other == null)
            {
                return false;
            }

            return true;
        }
    }
}
