using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsolePyramids
{
    class Task1
    {
        public void Run()
        {
            Console.Write("Введите А: ");
            int i = ConsoleExtension.NumRequest();
            Console.Write("Введите В: ");
            int j = ConsoleExtension.NumRequest();
            Console.WriteLine("Площадь: {0}", i * j);
        }      
    }
}
