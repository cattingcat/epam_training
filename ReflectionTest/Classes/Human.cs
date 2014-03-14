using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReflectionTest.Attributes;

namespace ReflectionTest.Classes
{
    [DebugAttribute]
    public class Human
    {
        [ContractAttribute(Type = ContractType.Less, Value = 150, NewValue = 80),
        ContractAttribute(Type = ContractType.More, Value = 40, NewValue = 80)]
        public int Weight { get; set; }

        [ContractAttribute(Type=ContractType.Less, Value=250, NewValue=210)]
        public int Growth { get; set; }

        [InjectAttribute]
        public string Name { get; set; }

        [InjectAttribute(Name="SuperResource")]
        public object Tag { get; set; }

        public override string ToString()
        {
            return String.Format("Name: {0}; Tag: {1}; Growth: {2}; Weight: {3}", Name, (Tag ?? "empty").ToString(), Growth, Weight);
        }
    }
}
