using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FLAT.Resources;
using FLAT.SyntaxAnalisys.FormalGrammar;
using FLAT.SyntaxAnalisys.LR;
using FLAT.SyntaxAnalisys.LR.Generation;

namespace FLAT.SyntaxAnalisys
{
    internal class Table
    {
        private TableAction[,] table;

        private Dictionary<Symbol, int> map;

        
        public Table(int numberOfStates, IEnumerable<Symbol> symbols)
        {
            if (symbols == null)
            {
                throw new ArgumentNullException("symbols", Strings.Symbols);
            }

            if (!symbols.Any())
            {
                throw new ArgumentException(Strings.Symbols, "symbols");
            }

            if (symbols.Where(s => s.IsEmpty()).Any())
            {
                throw new ArgumentException(Strings.Symbol, "symbols");
            }

            if (numberOfStates < 2)
            {
                throw new ArgumentException(Strings.NumberOfStatesLessThanOneException, "numberOfStates");
            }

            this.table = new TableAction[numberOfStates, symbols.Count()];

            this.map = symbols.Select((symbol, i) => new { Key = symbol, Value = i })
                    .ToDictionary(a => a.Key, a => a.Value);
        }


        public TableAction this[int state, Symbol symbol]
        {
            get
            {
                this.ValidateIndexerArguments(state, symbol);

                return this.table[state, this.map[symbol]];
            }
            set
            {
                this.ValidateIndexerArguments(state, symbol);

                this.table[state, this.map[symbol]] = value;
            }
        }


        public bool HasAction(int state, Symbol symbol)
        {
            return this.HasState(state) && this.HasSymbol(symbol);
        }

        public bool HasState(int state)
        {
            return (state <= this.table.GetUpperBound(0)) && (state >= this.table.GetLowerBound(0));
        }

        public bool HasSymbol(Symbol symbol)
        {
            return this.map.ContainsKey(symbol);
        }

        private void ValidateIndexerArguments(int state, Symbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException("symbol", Strings.SymbolName);
            }

            if (!this.HasSymbol(symbol))
            {
                throw new ArgumentOutOfRangeException("symbol", Strings.SymbolNotFoundException);
            }

            if (!this.HasState(state))
            {
                throw new ArgumentOutOfRangeException("state", Strings.StateNotFoundException);
            }
        }
    }
}
