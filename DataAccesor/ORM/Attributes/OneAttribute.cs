using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessor.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    class OneAttribute: RelationAttribute
    {
        public OneAttribute()
        {
            this.Type = RelationType.One;
        }
    }
}
