using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionTest.Attributes.Mappers
{
    [AttributeUsage(AttributeTargets.Property)]
    class IdAttribute: ColumnAttribute
    {
    }
}
