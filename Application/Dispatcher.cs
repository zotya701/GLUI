using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application
{
    public static class Dispatcher
    {
        private static ConcurrentQueue<Tuple<Action, ManualResetEventSlim>> Actions { get; } = new ConcurrentQueue<Tuple<Action, ManualResetEventSlim>>();

        public static ManualResetEventSlim Invoke(Action action)
        {
            var wHandle = new ManualResetEventSlim(false);
            Actions.Enqueue(Tuple.Create(action, wHandle));
            return wHandle;
        }

        internal static void ExecuteNextAction()
        {
            Tuple<Action, ManualResetEventSlim> wTuple;
            if (Actions.Any() && Actions.TryDequeue(out wTuple))
            {
                wTuple.Item1?.Invoke();
                wTuple.Item2.Set();
            }
        }
    }
}
