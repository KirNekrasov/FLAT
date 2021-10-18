using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLAT.LexicalAnalysis
{
    public class StringEndMarker : Token
    {
        public override bool IsEnd()
        {
            return true;
        }
    }
}
