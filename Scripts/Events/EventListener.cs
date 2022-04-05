using System;
using Event.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Event
{
    public sealed class EventListener : MonoBehaviour, IEventListener
    {
        public ScriptableEvent scriptableEvent;
        public UnityEvent<EventArgs> eventTriggered;

        private void OnEnable()
        {
            scriptableEvent.AddListener(this);
        }

        private void OnDisable()
        {
            scriptableEvent.RemoveListener(this);
        }

        public void OnEventTriggered(EventArgs args)
        {
            eventTriggered.Invoke(args);
        }
    }
}