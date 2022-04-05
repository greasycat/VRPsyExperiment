using System;
using System.Collections.Generic;
using System.Data;
using Event.Interfaces;
using UnityEngine;

namespace Event
{
    [CreateAssetMenu(fileName = "ScriptableEvent", menuName = "ScriptableEvent", order = 0)]
    public class ScriptableEvent : ScriptableObject, IEvent
    {
        private readonly List<EventListener> listeners = new List<EventListener>();

        public void TriggerEvent(EventArgs args)
        {
            for (var i = listeners.Count - 1; i>= 0; --i) listeners[i].OnEventTriggered(args);
        }

        public void AddListener(EventListener listener)
        {
            if (listener == null)
                throw new NoNullAllowedException("adding null listener");
            listeners.Add(listener);
        }

        public void RemoveListener(EventListener listener)
        {
            if (listener == null || !listeners.Contains(listener))
                throw new NoNullAllowedException("removing null listener");
            listeners.Remove(listener);
        }
    }
}