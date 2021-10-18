using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FLAT.Resources;

namespace FLAT.SyntaxAnalisys.FormalGrammar
{
    public class ContextFreeProduction : IContextFreeProduction, IProduction, IEquatable<ContextFreeProduction>
    {
        private Nonterminal left;

        private IEnumerable<Symbol> right;


        public ContextFreeProduction(Nonterminal left, IEnumerable<Symbol> right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left", Strings.Nonterminal + Strings.NullException);
            }
            this.left = left;

            if (right == null || !right.Any())
            {
                this.right = ImmutableList.Create((Symbol)new Empty());
            }
            else
            {
                if (right.Contains(new Empty()) && right.Count() > 1)
                {
                    throw new ArgumentException(Strings.UnexpectedEmptySymbolException, "right");
                }

                this.right = ImmutableList.CreateRange(right);
            }
        }

        public ContextFreeProduction(Nonterminal left, params Symbol[] right)
            : this(left, (IEnumerable<Symbol>)right) { }

        public ContextFreeProduction(Nonterminal left)
            : this(left, (IEnumerable<Symbol>)null) { }


        public Nonterminal Left
        {
            get { return this.left; }
        }

        IEnumerable<Symbol> IProduction.Left
        {
            get { return ImmutableList.Create(this.left); }
        }

        public IEnumerable<Symbol> Right
        {
            get { return this.right; }
        }


        public bool Equals(ContextFreeProduction other)
        {
            return this.Equals((IContextFreeProduction)other);
        }

        public bool Equals(IContextFreeProduction other)
        {
            if (other == null)
            {
                return false;
            }

            if (this == other)
            {
                return true;
            }

            return this.left.Equals(other.Left) && this.right.SequenceEqual(other.Right);
        }

        public bool Equals(IProduction other)
        {
            if (other == null)
            {
                return false;
            }

            var temp = other as IContextFreeProduction;

            return this.Equals(temp);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as IContextFreeProduction;

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.left.GetHashCode();
        }

        public bool IsEmpty()
        {
            return this.right.First().Equals(new Empty());
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<{0}> -> ", this.left);
            if (!this.IsEmpty())
            {
                foreach (var s in this.right)
                {
                    sb.Append(s);
                }
            }

            return sb.ToString();
        }
    }
}
