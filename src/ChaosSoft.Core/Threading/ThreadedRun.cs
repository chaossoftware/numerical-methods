using System;
using System.Diagnostics;
using System.Threading;

namespace ChaosSoft.Core.Threading
{
    public class ThreadedRun
    {
        private static int TotalProcessors = Environment.ProcessorCount;
        private static int ProcessorInWork = 0;

        public void RunOnSeparateProcessor(Action<object[]> task, object[] parameters)
        {
            while (ProcessorInWork > TotalProcessors - 1) ;
            Interlocked.Increment(ref ProcessorInWork);
            new Thread(delegate() { Task(task, parameters); }).Start();
        }

        private void Task(Action<object[]> task, object[] parameters)
        {
            try
            {
                task(parameters);
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
