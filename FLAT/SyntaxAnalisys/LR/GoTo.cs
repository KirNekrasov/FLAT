using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FLAT.LexicalAnalysis;
using FLAT.Resources;

namespace FLAT.SyntaxAnalisys.LR
{
    internal class GoTo : Shift, IEquatable<GoTo>
    {
        public GoTo(int goToState) : base(goToState) { }


        public override void Accept(LRParser parser, Token token)
        {
            parser.Visit(this, token);
        }

        public bool Equals(GoTo other)
        {
            return base.Equals((Shift)other);
        }
    }
}
