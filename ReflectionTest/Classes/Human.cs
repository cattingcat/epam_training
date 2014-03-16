using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReflectionTest.Attributes;
using ReflectionTest.Attributes.Mappers;

namespace ReflectionTest.Classes
{
    public abstract class Human
    {
        [Column(Name="human_weight")]
        [Contract(Type = ContractType.Less, Value = 150, NewValue = 80),
        Contract(Type = ContractType.More, Value = 40, NewValue = 80)]
        public int Weight { get; set; }

        [Column(Name = "human_growth")]
        [Contract(Type=ContractType.Less, Value=250, NewValue=210)]
        public int Growth { get; set; }

        [Id(Name="unique_name_id")]
        public string Name { get; set; }

        [Inject(Name="SuperResource")]
        public object Tag { get; set; }

        public override string ToString()
        {
            return String.Format("Name: {0}; Tag: {1}; Growth: {2}; Weight: {3}", Name, (Tag ?? "empty").ToString(), Growth, Weight);
        }
    }
}
