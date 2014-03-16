using ReflectionTest.Attributes;
using ReflectionTest.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ReflectionTest.Attributes.Mappers;

namespace ReflectionTest
{
    public sealed class ReflectProcessor
    {
        private IDictionary<string, object> injectDictionary;
        
        public ReflectProcessor()
        {
            injectDictionary = new Dictionary<string, object>();
            injectDictionary["SuperResource"] = new SuperResource();
            injectDictionary["Name"] = "ResourceName";
            injectDictionary["a"] = "a";
            injectDictionary["b"] = "b";
            injectDictionary["car"] = "DeLorean DMC-12";
            injectDictionary["String"] = "s";
            LinkedList<string> l = new LinkedList<string>();
            l.AddLast("qwe1");
            l.AddLast("qwe2");
            l.AddLast("qwe3");
            injectDictionary[typeof(LinkedList<string>).Name] = new MyPrivateIEnumerable<string> { List = l };
        }
        
        public void Process(object o)
        {
            Type t = o.GetType();
            bool debugMode = false;
            IEnumerable<Attribute> typeAttrs = t.GetCustomAttributes();
            foreach (var attr in typeAttrs)
            {
                if (attr is DebugAttribute)
                {
                    Console.WriteLine("Before: {0}", o.ToString());
                    debugMode = true;
                }

                if (attr is TableAttribute)
                {
                    ProcessEntity(o);
                }
            }

            PropertyInfo[] propertyInfoArr = t.GetProperties();
            foreach (var propInfo in propertyInfoArr)
            {
                if (debugMode)
                {
                    Console.Write("> Property {0} have attributes: ", propInfo.Name);
                }
                IEnumerable<Attribute> attributes =  propInfo.GetCustomAttributes();
                foreach (var attr in attributes)
                {                    
                    if (debugMode)
                    {
                        Console.Write("{0}, ", attr.GetType().Name);
                    }

                    if (attr is InjectAttribute)
                    {
                        ProcessInject(attr as InjectAttribute, propInfo, o);
                    }                    
                    else if (attr is ContractAttribute)
                    {
                        ProcessContract(attr as ContractAttribute, propInfo, o);
                    }
                }
                if (debugMode)
                {
                    Console.WriteLine(";");
                }
            }

            if (debugMode)
            {
                Console.WriteLine("After: {0}\n", o.ToString());
            }
        }

        private void ProcessContract(ContractAttribute attr, PropertyInfo propInfo, object o)
        {
            object propVal = propInfo.GetValue(o);
            switch (attr.Type)
            {
                case ContractType.Equa:
                    if (!propVal.Equals(attr.Value))
                    {
                        propInfo.SetValue(o, attr.NewValue);
                    }
                    break;
                case ContractType.Less:
                    if (propVal is IComparable && (propVal as IComparable).CompareTo(attr.Value) == 1)
                    {
                        propInfo.SetValue(o, attr.NewValue);
                    }
                    break;
                case ContractType.More:
                    if (propVal is IComparable && (propVal as IComparable).CompareTo(attr.Value) == -1)
                    {
                        propInfo.SetValue(o, attr.NewValue);
                    }
                    break;
                case ContractType.NotEqual:
                    if (propVal.Equals(attr.Value))
                    {
                        propInfo.SetValue(o, attr.NewValue);
                    }
                    break;
            }
        }

        private void ProcessInject(InjectAttribute attr, PropertyInfo propInfo, object o)
        {
            if (!String.IsNullOrEmpty(attr.Name))
            {
                object injectedVal = null;
                try
                {
                    injectedVal = injectDictionary[attr.Name];
                }
                catch (KeyNotFoundException e)
                {
                    injectedVal = Activator.CreateInstance(Type.GetType(propInfo.PropertyType.Name));
                    injectDictionary[propInfo.PropertyType.Name] = injectedVal;
                }
                propInfo.SetValue(o, injectedVal);
                return;
            }

            if (attr.InterfaceInheritor == null)
            {
                attr.InterfaceInheritor = propInfo.PropertyType;
            }

            if (attr.InterfaceInheritor != null)
            {
                object injectedVal = null;
                try
                {
                    injectedVal = injectDictionary[attr.InterfaceInheritor.Name];
                }
                catch (KeyNotFoundException e)
                {
                    injectedVal = Activator.CreateInstance(attr.InterfaceInheritor);
                    injectDictionary[attr.InterfaceInheritor.Name] = injectedVal;
                }
                propInfo.SetValue(o, injectedVal);
                return;
            }

            throw new Exception("Inject Exception");
        }

        public void ProcessEntity(object o)
        {
            string tableName = string.Empty;
            IList<string> columnList = new List<string>();
            TableAttribute tableAttr = null;
            string idColumnName = string.Empty;

            Type type = o.GetType();
            IEnumerable<Attribute> attributes = type.GetCustomAttributes();
            foreach (Attribute a in attributes)
            {
                if (a is TableAttribute)
                {
                    tableAttr = (a as TableAttribute);
                    tableName = tableAttr.Name;
                }
            }
            if (String.IsNullOrEmpty(tableAttr.SelectQuery) || String.IsNullOrEmpty(tableAttr.InsertQuery) 
                || String.IsNullOrEmpty(tableAttr.DeleteQuery))
            {
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    IEnumerable<Attribute> propAttributes = property.GetCustomAttributes();
                    foreach (Attribute a in propAttributes)
                    {
                        if (a is ColumnAttribute)
                        {
                            string name = (a as ColumnAttribute).Name;
                            columnList.Add(name);
                            if (a is IdAttribute)
                            {
                                idColumnName = name;
                            }
                        }
                    }
                }

                // make list of columns
                StringBuilder builder = new StringBuilder();
                foreach (string colName in columnList)
                {
                    builder.Append(colName);
                    builder.Append(", ");
                }
                builder.Remove(builder.Length - 2, 2);
                string stringColList = builder.ToString();
                builder.Clear();

                // make select query
                builder.Append("SELECT ");
                builder.Append(stringColList);
                builder.Append(" FROM ");
                builder.Append(tableName);
                builder.Append(";");
                tableAttr.SelectQuery = builder.ToString();
                builder.Clear();

                // make insert query
                builder.Append("INSERT (");
                builder.Append(stringColList);
                builder.Append(") INTO ");
                builder.Append(tableName);
                builder.Append(" VALUES (");
                for (int i = 0; i < columnList.Count; ++i)
                {
                    if (i != columnList.Count - 1)
                        builder.Append(String.Format("{0}, ", i));
                    else
                        builder.Append(String.Format("{0}", i));
                }
                builder.Append(");");
                tableAttr.InsertQuery = builder.ToString();
                builder.Clear();

                // make delete query
                builder.Append("DELETE FROM ");
                builder.Append(tableName);
                builder.Append(" WHERE (");
                builder.Append(idColumnName);
                builder.Append(" = 0);");
                tableAttr.DeleteQuery = builder.ToString();
            }

            Console.WriteLine(tableAttr.SelectQuery);
            Console.WriteLine(tableAttr.InsertQuery);
            Console.WriteLine(tableAttr.DeleteQuery);
        }

        private class MyPrivateIEnumerable<T> : IEnumerable<T>
        {
            private LinkedList<T> list;

            public LinkedList<T> List
            {
                get { return list; }
                set { list = value; }
            }

            public IEnumerator<T> GetEnumerator()
            {
                Console.WriteLine("From PRIVATE class MyPrivateIEnumerable<T> : IEnumerable<T>");
                return list.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
