using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLAT.Resources;

namespace FLAT.SyntaxAnalisys.FormalGrammar
{
    public abstract class Symbol : IEquatable<Symbol>
    {
        protected enum SymbolType
        {
            Terminal,
            Nonterminal,
            Empty,
            End
        }


        protected String name;

        protected Symbol.SymbolType type;


        protected Symbol() { }

        protected Symbol(Symbol.SymbolType type)
        {
            this.name = "";

            this.type = type;
        }

        protected Symbol(String name, Symbol.SymbolType type)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name", Strings.SymbolName);
            }

            if (name.Length == 0)
            {
                throw new ArgumentException(Strings.SymbolName, "name");
            }

            this.name = name;

            this.type = type;
        }


        public String Name
        {
            get { return this.name; }
        }

        protected Symbol.SymbolType Type
        {
            get { return this.type; }
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as Symbol;

            return this.Equals(other);
        }

        public bool Equals(Symbol other)
        {
            if (other == null)
            {
                return false;
            }

            if (this == other)
            {
                return true;
            }

            return this.Type == other.Type && this.Name == other.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ (int)this.Type.GetHashCode();
        }

        public bool IsEmpty()
        {
            return this.Type == Symbol.SymbolType.Empty;
        }

        public bool IsEnd()
        {
            return this.Type == Symbol.SymbolType.End;
        }

        public bool IsNonterminal()
        {
            return this.Type == Symbol.SymbolType.Nonterminal;
        }

        public bool IsTerminal()
        {
            return this.Type == Symbol.SymbolType.Terminal;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
