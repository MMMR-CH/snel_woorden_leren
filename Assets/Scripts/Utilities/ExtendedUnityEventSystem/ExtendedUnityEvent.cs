using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace ExtendedUnityEventSystem
{
    public class ExtendedAsyncUnityEvent : UnityEvent
    {
        private sealed class Listener
        {
            public int Order;
            public Func<CancellationToken, UniTask> Callback;
        }
        
        private readonly List<Listener> _listeners = new();
        public bool IsCompleted { get; private set; }
        CancellationToken _cancellationToken = default;

        // --- Add: void listener ---
        public void AddListener(UnityAction action, int order = 0)
        {
            if (action == null) return;

            _listeners.Add(new Listener
            {
                Order = order,
                Callback = _ =>
                {
                    action.Invoke();
                    return UniTask.CompletedTask;
                }
            });
        }

        // --- Add: async listener ---
        public void AddListener(Func<UniTask> asyncAction, int order = 0)
        {
            if (asyncAction == null) return;

            _listeners.Add(new Listener
            {
                Order = order,
                Callback = _ => asyncAction.Invoke()
            });
        }

        // --- Add: async + cancellation ---
        public void AddListener(Func<CancellationToken, UniTask> asyncAction, int order = 0)
        {
            if (asyncAction == null) return;

            _listeners.Add(new Listener
            {
                Order = order,
                Callback = asyncAction
            });
        }

        /// <summary>
        /// sequential=true => order'a göre sırayla (birisi bitmeden diğeri başlamaz)
        /// sequential=false => hepsi aynı anda (WhenAll)
        /// </summary>
        public async UniTask Invoke(bool sequential = true, CancellationToken token = default)
        {
            IsCompleted = false;
            _cancellationToken = token;

            // Snapshot: invoke sırasında liste değişirse patlamasın
            var list = _listeners
                .OrderBy(l => l.Order)
                .ToArray();

            try
            {
                if (sequential)
                {
                    foreach (var l in list)
                    {
                        token.ThrowIfCancellationRequested();
                        await l.Callback(token);
                    }
                }
                else
                {
                    // paralel
                    var tasks = list.Select(l => l.Callback(token));
                    await UniTask.WhenAll(tasks);
                }
            }
            finally
            {
                IsCompleted = true;
            }
        }

        /// <summary>
        /// Cancels after invoke is called
        /// </summary>
        public void Cancel()
        {
            // check if already completed
            if (IsCompleted) return;

            // check if invoke has been called
            if (_cancellationToken == default) return;

            // check if token can be cancelled
            if (!_cancellationToken.IsCancellationRequested)
            {
                var cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken);
                cts.Cancel();
            }
        }

        private void Reset()
        {
            IsCompleted = false;
            _cancellationToken = default;
            _listeners.Clear();
        }
    }
}