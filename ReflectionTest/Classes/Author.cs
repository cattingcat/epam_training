using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReflectionTest.Attributes;
using ReflectionTest.Attributes.Mappers;

namespace ReflectionTest.Classes
{
    [Table(Name="Authors")]
    [Debug]
    class Author: Human
    {
        [Inject(InterfaceInheritor=typeof(LinkedList<string>))]
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
                b.Remove(b.Length - 3, 2);
            }
            return String.Format("{0}; Books: {1}", base.ToString(), b.ToString());
        }
    }
}
