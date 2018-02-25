using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class Dispatcher
    {
        internal static ConcurrentQueue<Action> Actions { get; } = new ConcurrentQueue<Action>();

        public static void Invoke(Action action)
        {
            Actions.Enqueue(action);
        }
    }
}
