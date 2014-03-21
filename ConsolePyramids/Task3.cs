using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsolePyramids
{
    class Task3: IPyramidTask
    {
        public void Run(int n, TextWriter w, int offset = 0)
        {
            int maxWidth = n * 2 + 1;
            for (int i = 0; i < n; ++i)
            {
                StringBuilder b = new StringBuilder();
                b.Append(' ', (maxWidth - 1) / 2 - i + offset);
                b.Append('*', i * 2 + 1);
                w.WriteLine(b.ToString());
            }
        }
    }
}
