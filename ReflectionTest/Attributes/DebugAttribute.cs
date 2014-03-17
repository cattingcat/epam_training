using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionTest.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited=false)]
    class DebugAttribute: Attribute
    {
    }
}
