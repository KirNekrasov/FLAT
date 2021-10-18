using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLAT.LexicalAnalysis
{
    public interface IScanner
    {
        IEnumerable<Token> Scan(String input);
    }
}
