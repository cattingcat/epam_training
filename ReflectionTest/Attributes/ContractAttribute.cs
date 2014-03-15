using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionTest.Attributes
{
    enum ContractType { More, Less, Equa, NotEqual};

    [AttributeUsage(AttributeTargets.Property, AllowMultiple=true)]
    class ContractAttribute: Attribute
    {
        public object Value { get; set; }
        public ContractType Type { get; set; }
        public object NewValue { get; set; }
    }
}
