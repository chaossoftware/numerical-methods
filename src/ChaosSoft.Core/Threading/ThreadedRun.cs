using System;
using System.Diagnostics;
using System.Threading;

namespace ChaosSoft.Core.Threading
{
    public class ThreadedRun
    {
        private static int TotalProcessors = Environment.ProcessorCount;
        private static int ProcessorInWork = 0;

        public void RunOnSeparateProcessor(Action action)
        {
            while (ProcessorInWork > TotalProcessors - 1) ;
            Interlocked.Increment(ref ProcessorInWork);
            new Thread(delegate() { Task(action); }).Start();
        }

        private void Task(Action action)
        {
            try
            {
                action();
            }
            catch
            {

            }

            Interlocked.Decrement(ref ProcessorInWork);
        }

        public void WaitForAllTasks()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            while (ProcessorInWork > 0 && timer.ElapsedMilliseconds < 60000) ;
        }
    }
}
