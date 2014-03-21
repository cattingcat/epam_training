using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsolePyramids
{
    class Program
    {
        static void Main(string[] args)
        {                      
            bool exitFlag = false;
            do
            {
                Console.Write("Выберите задание (Задания: 1-4, Выход: 5-...): ");  
                int taskNumber = ConsoleExtension.NumRequest();
                IPyramidTask t = null;
                switch (taskNumber)
                {
                    case 1:
                        Task1 t1 = new Task1();
                        t1.Run();
                        break;
                    case 2:
                        t = new Task2();
                        break;
                    case 3:
                        t = new Task3();
                        break;
                    case 4:
                        t = new Task4();
                        break;
                    default:
                        exitFlag = true;
                        break;
                }
                if (t != null)
                {
                    Console.Write("N = ");
                    int n = ConsoleExtension.NumRequest();
                    t.Run(n, Console.Out);
                }
            } while (!exitFlag);

            /*
            TextWriter tw = new StreamWriter("file");
            ITask t = new Task4();
            t.Run(80, tw);
            */
        }
    }
}
