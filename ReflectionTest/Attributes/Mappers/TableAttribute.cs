using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionTest.Attributes.Mappers
{
    [AttributeUsage(AttributeTargets.Class)]
    class TableAttribute: Attribute
    {
        public string Name { get; set; }
        public string SelectQuery { get; set; }
        public string InsertQuery { get; set; }
        public string DeleteQuery { get; set; }

        public TableAttribute()
        {
            SelectQuery = string.Empty;
            InsertQuery = string.Empty;
            DeleteQuery = string.Empty;
        }
    }
}
