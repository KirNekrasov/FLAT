using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLAT.Resources;

namespace FLAT.LexicalAnalysis
{
    public class Token
    {
        private String attribute;

        private String name;


        protected Token()
        {
            this.name = "";
        }

        public Token(String name, String attribute)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name", Strings.TokenName + Strings.NullException);
            }

            if (name.Length == 0)
            {
                throw new ArgumentException(Strings.TokenName + Strings.EmptyException, "name");
            }

            this.attribute = attribute;

            this.name = name;
        }

        public Token(String name) : this(name, null) { }


        public String Attribute
        {
            get { return this.attribute; }
        }

        public String Name
        {
            get { return this.name; }
        }


        public virtual bool IsEnd()
        {
            return false;
        }
    }
}
