using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLAT.SyntaxAnalisys.FormalGrammar
{
    public interface IProduction : IEquatable<IProduction>
    {
        IEnumerable<Symbol> Left { get; }

        IEnumerable<Symbol> Right { get; }


        bool IsEmpty();
    }
}
