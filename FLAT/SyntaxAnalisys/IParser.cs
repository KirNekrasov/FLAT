using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FLAT.LexicalAnalysis;

namespace FLAT.SyntaxAnalisys
{
    public interface IParser
    {
        ParseTree Parse(IEnumerable<Token> tokens);
    }
}
