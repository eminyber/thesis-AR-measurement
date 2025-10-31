using System;

namespace ARMeasurementApp.Scripts.Events
{
    public class Event
    {
        private event Action action = delegate { };

        public void RaiseEvent()
        {
            action?.Invoke();
        }

        public void AddListener(Action listener)
        {
            action += listener;
        }

        public void RemoveListener(Action listener)
        {
            action -= listener;
        }
    }

    public class Event<T>
    {
        private event Action<T> action = delegate { };

        public void RaiseEvent(T param)
        {
            action?.Invoke(param);
        }

        public void AddListener(Action<T> listener)
        {
            action += listener;
        }

        public void RemoveListener(Action<T> listener)
        {
            action -= listener;
        }
    }

    public class EventWithHandler<T> where T : EventArgs
    {
        private event EventHandler<T> action = delegate { };

        public void RaiseEvent(object sender, T param)
        {
            action?.Invoke(sender, param);
        }

        public void AddListener(EventHandler<T> listener)
        {
            action += listener;
        }

        public void RemoveListener(EventHandler<T> listener)
        {
            action -= listener;
        }
    }
}

