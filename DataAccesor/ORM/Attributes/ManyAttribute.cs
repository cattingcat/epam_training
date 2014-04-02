using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessor.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    class ManyAttribute: RelationAttribute
    {
        public ManyAttribute()
        {
            this.Type = RelationType.Many;
        }
    }
}
