namespace Deliverystack.Core.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
 
    public class ThreadList
    {
        public bool ThreadingDisabled { get; private set; }

        public bool UseTasks { get; private set; }

        private List<Thread> _threads = null;

        private List<Task> _tasks = null;

        public ThreadList(bool disableThreading = false, bool useTasks = false)
        {
            ThreadingDisabled = disableThreading;
            UseTasks = useTasks;

            if (!ThreadingDisabled)
            {
                if (UseTasks)
                {
                    _tasks = new List<Task>();
                }
                else
                {
                    _threads = new List<Thread>();
                }
            }
        }

        public void Add(Action action, bool isForeground = false, bool disableThreading = false)
        {
            if (ThreadingDisabled || disableThreading)
            {
                action();
            }
            else
            {
                if (UseTasks)
                {
                    _tasks.Add(Task.Factory.StartNew(action));
                }
                else
                {
                    Thread thread = new Thread((t) => action());
                    thread.IsBackground = !isForeground;
                    thread.Start();
                    _threads.Add(thread);
                }
            }
        }

        public void JoinAll()
        {
            if (_threads != null)
            {
                foreach (Thread thread in _threads)
                {
                    thread.Join();
                }
            }

            if (_tasks != null)
            {
                foreach (Task task in _tasks)
                {
                    task.Wait();
                }
            }
        }
    }
}
