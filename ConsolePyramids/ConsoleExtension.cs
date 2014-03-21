using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePyramids
{
    static class ConsoleExtension
    {
        public static int NumRequest()
        {
            bool correct = false;
            int num = 0;
            do
            {
                string s = Console.ReadLine();
                bool isNum = Int32.TryParse(s, out num);
                if (!isNum)
                {
                    //Console.WriteLine("Пожалуйста, введите число.");
                }
                else if (num <= 0)
                {
                    Console.WriteLine("Число должно быть > 0.");
                }
                else
                {
                    correct = true;
                }
            } while (!correct);
            return num;
        }
    }
}
