using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLAT.SyntaxAnalisys.FormalGrammar
{
    public interface IContextFreeProduction : IProduction, IEquatable<IContextFreeProduction>
    {
        new Nonterminal Left { get; }
    }
}
