using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReflectionTest.Attributes;

namespace ReflectionTest.Classes
{
    class Author: Human
    {
        [InjectAttribute(InterfaceInheritor=typeof(LinkedList<string>))]
        public IEnumerable<string> Books { get; set; }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            if (Books != null)
            {
                foreach (string s in Books)
                {
                    b.Append(s);
                    b.Append(", ");
                }
            }
            return String.Format("{0}; Books: {1}", base.ToString(), b.ToString());
        }
    }
}
