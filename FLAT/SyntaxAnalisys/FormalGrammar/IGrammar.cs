using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLAT.SyntaxAnalisys.FormalGrammar
{
    public interface IGrammar
    {
        Nonterminal Axiom { get; }

        IEnumerable<Nonterminal> Nonterminals { get; }

        IEnumerable<IProduction> Productions { get; }

        IEnumerable<Terminal> Terminals { get; }
    }
}
