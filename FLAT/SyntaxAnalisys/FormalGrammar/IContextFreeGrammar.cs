using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLAT.SyntaxAnalisys.FormalGrammar
{
    public interface IContextFreeGrammar : IGrammar
    {
        new IEnumerable<IContextFreeProduction> Productions { get; }


        IEnumerable<IContextFreeProduction> ProductionsByLeft(Nonterminal nonterminal);

        IEnumerable<Symbol> First(IEnumerable<Symbol> symbols);

        IEnumerable<Symbol> Follow(Nonterminal symbol);
    }
}
