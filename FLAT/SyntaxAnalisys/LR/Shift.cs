using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLAT.LexicalAnalysis;
using FLAT.Resources;

namespace FLAT.SyntaxAnalisys.LR
{
    internal class Shift : TableAction, IEquatable<Shift>
    {
        protected int goToState;


        public Shift(int goToState)
        {
            this.goToState = goToState;
        }


        public int GoToState
        {
            get { return this.goToState; }
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

            var temp = other as Shift;

            return this.Equals(temp);
        }

        public bool Equals(Shift other)
        {
            if (other == null)
            {
                return false;
            }

            return this.goToState == other.GoToState;
        }
    }
}
