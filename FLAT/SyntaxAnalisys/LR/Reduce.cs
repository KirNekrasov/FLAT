using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLAT.LexicalAnalysis;
using FLAT.Resources;
using FLAT.SyntaxAnalisys.FormalGrammar;

namespace FLAT.SyntaxAnalisys.LR
{
    internal class Reduce : TableAction, IEquatable<Reduce>
    {
        private IContextFreeProduction production;


        public Reduce(IContextFreeProduction production)
        {
            if (production == null)
            {
                throw new ArgumentNullException("production", Strings.Production + Strings.NullException);
            }

            this.production = production;
        }


        public IContextFreeProduction Production
        {
            get { return this.production; }
        }


        public override void Accept(LRParser parser, Token token)
        {
            parser.Visit(this, token);
        }

        public override bool Equals(TableAction other)
        {
            if (other == null)
            {
                return false;
            }

            if (other == this)
            {
                return true;
            }

            var temp = other as Reduce;

            return this.Equals(temp);
        }

        public bool Equals(Reduce other)
        {
            if (other == null)
            {
                return false;
            }

            return this.production.Equals(other.Production);
        }
    }
}
