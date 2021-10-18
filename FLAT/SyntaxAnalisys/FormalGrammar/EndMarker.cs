using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLAT.SyntaxAnalisys.FormalGrammar
{
    internal class EndMarker : Symbol, IEquatable<EndMarker>
    {
        public EndMarker() : base(SymbolType.End) { }


        public bool Equals(EndMarker other)
        {
            if (other == null)
            {
                return false;
            }

            return true;
        }
    }
}
