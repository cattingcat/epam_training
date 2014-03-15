using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReflectionTest.Attributes.Mappers;

namespace ReflectionTest.Classes
{
    [Table(Name="EntityTable")]
    class SomeEntity
    {
        [Id(Name="id")]
        public int ID { get; set; }
        [Column(Name="des")]
        public string Description { get; set; }
        [Column(Name="floatValue")]
        public float Value { get; set; }
    }
}
