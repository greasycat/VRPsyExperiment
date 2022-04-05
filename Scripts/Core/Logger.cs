using System;
using Core.Interfaces;
using UnityEngine;

namespace Core
{
    public class Logger : MonoBehaviour, IService
    {
        private void Awake()
        {
            Locator.instance.Register(this);
        }

        private void OnDestroy()
        {
            Locator.instance.Unregister<Logger>();
        }


        public void Debug (string msg) { UnityEngine.Debug.Log(msg); }
    }
    
}