using System;

namespace Event.Interfaces
{
    public interface IEvent
    {
        public void TriggerEvent(EventArgs args);
        public void AddListener(EventListener listener);
        public void RemoveListener(EventListener listener);
    }
}