using System;
using Core;
using Core.Mechanics;
using Event;
using UnityEngine;

namespace Mechanics.GraphTask
{
    public class GraphTaskInteractable : Interactable
    {
        
        
        private void OnTriggerEnter(Collider other)
        {
            triggerEnterScriptableEvent.TriggerEvent(new CollisionEventArgs(other));
        }

        private void OnTriggerExit(Collider other)
        {
            triggerExitScriptableEvent.TriggerEvent(new CollisionEventArgs(other));
        }
        
    }
}