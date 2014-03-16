using ReflectionTest.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionTest.Classes
{
    [Debug]
    class Driver: Human
    {
        [Inject(Name="car")]
        public string Car { get; set; }

        public override string ToString()
        {
            return String.Format("{0}; Car: {1}", base.ToString(), Car);
        }
    }
}
