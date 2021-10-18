using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FLAT.Resources;
using FLAT.SyntaxAnalisys.FormalGrammar;

namespace FLAT.SyntaxAnalisys.LR.Generation
{
    internal class Configuration : IEquatable<Configuration>
    {
        private int mark;

        private IContextFreeProduction production;


        public Configuration(IContextFreeProduction production)
        {
            if (production == null)
            {
                throw new ArgumentNullException("ContextFreeProduction");
            }

            this.mark = 0;

            this.production = production;
        }

        public Configuration(Configuration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.production = configuration.production;

            this.mark = configuration.Mark;
        }


        public int Mark
        {
            get { return this.mark; }
        }

        public Symbol MarkedSymbol
        {
            get
            {
                Symbol current = null;
                if (!this.IsMarkAtEnd())
                {
                    current = this.production.Right.ElementAt(this.mark);
                }

                return current;
            }
        }

        public IContextFreeProduction Production
        {
            get { return this.production; }
        }



        public bool Equals(Configuration other)
        {
            if (other == null)
            {
                return false;
            }

            if (other == this)
            {
                return true;
            }


            return this.mark == other.Mark && this.production.Equals(other.Production);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as Configuration;

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.production.GetHashCode();
        }

        public bool IsMarkAtEnd()
        {
            return this.production.IsEmpty() || (this.mark == this.production.Right.Count());
        }

        public Symbol MoveMarkToNextSymbol()
        {
            if (!this.IsMarkAtEnd())
            {
                this.mark += 1;
            }
            else
            {
                throw new InvalidOperationException(Strings.MoveMarkOutOfProduction);
            }

            return this.MarkedSymbol;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} -> ", this.production.Left);

            if (this.production.IsEmpty())
            {
                sb.Append(".");
            }
            else
            {
                var temp = this.production.Right.ToList();
                for (var i = 0; i < temp.Count(); ++i)
                {
                    if (i == this.mark)
                    {
                        sb.Append(".");
                    }

                    sb.Append(temp[i]);
                }

                if (this.IsMarkAtEnd())
                {
                    sb.Append(".");
                }
            }

            return sb.ToString();
        }
    }
}
