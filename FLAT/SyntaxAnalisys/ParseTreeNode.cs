using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FLAT.Resources;
using FLAT.SyntaxAnalisys.FormalGrammar;

namespace FLAT.SyntaxAnalisys
{
    public class ParseTreeNode
    {
        protected String attribute;

        protected IEnumerable<ParseTreeNode> descendants;

        protected String name;


        public ParseTreeNode(String name, String attribute, IEnumerable<ParseTreeNode> descendants)
        {
            if (name == null)
            {
                throw new ArgumentNullException("value", Strings.ParseTreeNodeName + Strings.NullException);
            }

            this.name = name;

            this.attribute = attribute;

            if ((descendants != null) && descendants.Any())
            {
                this.descendants = new List<ParseTreeNode>(descendants);
            }
            else
            {
                this.descendants = new List<ParseTreeNode>();
            }
        }

        public ParseTreeNode(String name, String attribute, params ParseTreeNode[] descendants)
            : this(name, attribute, (IEnumerable<ParseTreeNode>)descendants) { }


        public String Attribute
        {
            get { return this.attribute; }
        }

        public IEnumerable<ParseTreeNode> Descendants
        {
            get { return this.descendants; }
        }

        public String Name
        {
            get { return this.name; }
        }


        public bool IsLeaf()
        {
            return !this.descendants.Any();
        }
    }
}
