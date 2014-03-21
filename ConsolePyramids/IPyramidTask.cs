using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsolePyramids
{
    public interface IPyramidTask
    {
        void Run(int n, TextWriter w, int offset = 0);
    }
}
