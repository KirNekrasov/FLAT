using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLAT.SyntaxAnalisys
{
    public class ParseTree
    {
        protected ParseTreeNode root;

        
        public ParseTree(ParseTreeNode root)
        {
            this.root = root;
        }


        public ParseTreeNode Root
        {
            get { return this.root; }
        }
    }
}
