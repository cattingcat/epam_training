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


            Author author = new Author { Name = "Edgar", Growth = 178, Weight = 1000 };
            Human[] humanArray = { new Driver { Name = "Doc", Growth=188, Weight=76 }, author };

            foreach (Human h in humanArray) 
            { 
                processor.Process(h);
            }

            // private class MyPrivateIEnumerable<T>
            foreach (string book in author.Books)
            {
                Console.WriteLine(book);
            }

            Console.ReadKey();

            
        }

    }
}
