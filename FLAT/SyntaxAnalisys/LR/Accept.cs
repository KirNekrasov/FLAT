using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FLAT.LexicalAnalysis;
using FLAT.Resources;

namespace FLAT.SyntaxAnalisys.LR
{
    internal class Valid : TableAction, IEquatable<Valid>
    {
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

            var temp = other as Valid;

            return this.Equals(temp);
        }

        public bool Equals(Valid other)
        {
            if (other == null)
            {
                return false;
            }

            return true;
        }
    }
}
