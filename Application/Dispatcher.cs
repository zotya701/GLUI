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
        private static ConcurrentQueue<(Action Callback, ManualResetEventSlim CallbackFinished)> Actions { get; } = new ConcurrentQueue<(Action, ManualResetEventSlim)>();

        public static void BeginInvoke(Action action)
        {
            Actions.Enqueue((action, null));
        }

        public static void Invoke(Action action)
        {
            var wHandle = new ManualResetEventSlim(false);
            Actions.Enqueue((action, wHandle));
            wHandle.Wait();
        }

        internal static void ExecuteNextAction()
        {
            if (Actions.IsEmpty) return;

            if(Actions.TryDequeue(out var wAction))
            {
                wAction.Callback?.Invoke();
                wAction.CallbackFinished?.Set();
            }
        }
    }
}
