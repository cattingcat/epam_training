using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReflectionTest.Attributes;
using ReflectionTest.Classes;
using System.Reflection;

namespace ReflectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ReflectProcessor processor = new ReflectProcessor();

            Human[] humanArray = { new Human { Name = "John", Growth = 1150, Weight = 55 },
                                 new Driver { Name = "Doc", Growth=188, Weight=76 },
                                 new Author { Name = "Edgar", Growth=178, Weight=1000 }
                               };

            foreach (Human h in humanArray) 
            { 
                processor.Process(h);
            }

            processor.Process(new SomeEntity());
            

            Console.ReadKey();
        }

    }
}
