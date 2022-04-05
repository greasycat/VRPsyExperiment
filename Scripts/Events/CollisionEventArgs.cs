using System;
using UnityEngine;

namespace Event
{
    public class CollisionEventArgs: EventArgs
    {
        public Collider Collider { get; private set; }
        public CollisionEventArgs(Collider collider)
        {
            Collider = collider;
        }
    }
}