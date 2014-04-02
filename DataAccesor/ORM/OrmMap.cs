using DataAccessor.ORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessor.ORM
{
    internal enum ColumnType { Simple, Relation };

    internal class OrmMap
    {
        private IDictionary<string, PropertyInfo> simpleColumnToProperty;
        private IDictionary<string, PropertyInfo> relationColumnToProperty;
        private Type objectType;

        public string TableName { get; private set; }
        public string ID { get; private set; }

        private OrmMap()
        {
            simpleColumnToProperty = new Dictionary<string, PropertyInfo>();
            relationColumnToProperty = new Dictionary<string, PropertyInfo>();
        }

        public PropertyInfo this[string columnName, ColumnType type = ColumnType.Simple]
        {
            get
            {
                if (type == ColumnType.Simple)
                    return simpleColumnToProperty[columnName];
                else if (type == ColumnType.Relation)
                    return relationColumnToProperty[columnName];
                else
                    throw new Exception("");
            }
        }
        public ICollection<string> Columns
        {
            get
            {
                return simpleColumnToProperty.Keys;
            }
        }
        public ICollection<string> Relations
        {
            get
            {
                return relationColumnToProperty.Keys;
            }
        }
        public PropertyInfo GetIDPropertyInfo() 
        {
            return this[ID];
        }
        public System.Data.DbType GetDbType(string columnName)
        {
            ColumnAttribute attr = this[columnName].GetCustomAttribute<ColumnAttribute>();
            return attr.ColumnType;
        }
        public Type ObjectType
        {
            get
            {
                return objectType;
            }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("Table name: ");
            b.Append(TableName);
            b.Append("\nColumns: \n");
            foreach (var c in simpleColumnToProperty)
            {
                b.Append("  ");
                b.Append(c.Key);
                b.Append(" => ");
                b.Append(c.Value.Name);
                b.Append("\n");
            }
            return b.ToString();
        }

        public static OrmMap FromType(Type type)
        {
            TableAttribute attr = (TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), true).SingleOrDefault();
            if (attr == null)
            {
                throw new Exception("this type have no TableAttribute");
            }
            OrmMap map = new OrmMap();
            map.objectType = type;
            map.TableName = attr.TableName;

            foreach (PropertyInfo p in type.GetProperties())
            {
                ColumnAttribute ca = p.GetCustomAttribute<ColumnAttribute>();
                if (ca != null)
                {
                    map.simpleColumnToProperty[ca.ColumnName] = p;
                    if (ca is IdAttribute)
                    {
                        map.ID = ca.ColumnName;
                    }
                }
                else
                {
                    RelationAttribute ra = p.GetCustomAttribute<RelationAttribute>();
                    if (ra != null)
                    {
                        if (ra.Type == RelationType.One)
                        {
                            map.relationColumnToProperty[ra.ThisColumn] = p;
                        }
                        else if(ra.Type == RelationType.Many)
                        {
                            map.relationColumnToProperty[map.ID] = p;
                        }
                    }
                }
            }
            return map;
        }
    }
}
