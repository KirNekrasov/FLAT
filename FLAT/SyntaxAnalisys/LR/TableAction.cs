using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FLAT.LexicalAnalysis;
using FLAT.Resources;

namespace FLAT.SyntaxAnalisys.LR
{
    internal abstract class TableAction : IEquatable<TableAction>
    {
        public abstract void Accept(LRParser parser, Token token);

        public abstract bool Equals(TableAction other);
    }
}
