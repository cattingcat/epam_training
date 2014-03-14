using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionTest.Attributes
{
    class InjectAttribute: Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Type InterfaceInheritor { get; set; }
    }
}
