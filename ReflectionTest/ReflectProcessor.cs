using ReflectionTest.Attributes;
using ReflectionTest.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            injectDictionary[typeof(LinkedList<string>).Name] = l;
        }
        
        public void Process(object h)
        {
            Type t = h.GetType();
            bool debugMode = false;
            IEnumerable<Attribute> typeAttrs = t.GetCustomAttributes();
            foreach (var attr in typeAttrs)
            {
                if (attr is DebugAttribute)
                {
                    Console.WriteLine("Before: {0}", h.ToString());
                    debugMode = true;
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
                        ProcessInject(attr as InjectAttribute, propInfo, h);
                    }
                    else if (attr is ContractAttribute)
                    {
                        ProcessContract(attr as ContractAttribute, propInfo, h);
                    }
                }
                if (debugMode)
                {
                    Console.WriteLine(";");
                }
            }

            if (debugMode)
            {
                Console.WriteLine("After: {0}\n", h.ToString());
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
            if (attr.Name != null && attr.Name != String.Empty)
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
    }
}
