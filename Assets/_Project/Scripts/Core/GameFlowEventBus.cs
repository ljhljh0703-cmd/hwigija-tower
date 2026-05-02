using System;

namespace HwigiTower.Core
{
    public sealed class GameFlowEventBus
    {
        public event Action<GameFlowEvent> OnEventRaised;

        public void Raise(GameFlowEvent flowEvent)
        {
            OnEventRaised?.Invoke(flowEvent);
        }

        public IDisposable Subscribe(Action<GameFlowEvent> handler)
        {
            OnEventRaised += handler;
            return new Subscription(this, handler);
        }

        private void Unsubscribe(Action<GameFlowEvent> handler)
        {
            OnEventRaised -= handler;
        }

        private sealed class Subscription : IDisposable
        {
            private readonly GameFlowEventBus _bus;
            private readonly Action<GameFlowEvent> _handler;
            private bool _disposed;

            public Subscription(GameFlowEventBus bus, Action<GameFlowEvent> handler)
            {
                _bus = bus;
                _handler = handler;
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _bus.Unsubscribe(_handler);
                _disposed = true;
            }
        }
    }
}
