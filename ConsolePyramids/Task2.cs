using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsolePyramids
{
    class Task2: IPyramidTask
    {
        public void Run(int n, TextWriter w, int offset = 0)
        {
            for (int i = 0; i < n; ++i)
            {
                StringBuilder b = new StringBuilder();
                b.Append(' ', offset);
                b.Append('*', i + 1);
                w.WriteLine(b.ToString());
            }
        }
    }
}
