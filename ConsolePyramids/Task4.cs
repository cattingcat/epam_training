using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsolePyramids
{
    class Task4: IPyramidTask
    {
        private IPyramidTask _subTask;
        public Task4()
        {
            _subTask = new Task3();
        }
        public void Run(int n, TextWriter w, int offset)
        {
            int maxWidth = n * 2 + 1;
            for (int i = 1; i < n + 1; ++i)
            {
                _subTask.Run(i, w, offset + (n - i));
            }
        }
    }
}
