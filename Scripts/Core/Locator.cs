using System;
using System.Collections.Generic;
using Core.Interfaces;
using UnityEngine;

namespace Core
{
    public class Locator 
    {
        //C# Lazy class
        private static readonly Lazy<Locator> Lazy
            = new Lazy<Locator>(() => new Locator());

        public static Locator instance => Lazy.Value;
        private readonly Dictionary<string, IService> service;

        // private void Awake()
        // {
        //     lazy = new Lazy<Locator>(() =>
        //     {
        //         service = new Dictionary<string, IService>();
        //         return this;
        //     });
        // }

        //Get service by calling this method

        private Locator()
        {
            service = new Dictionary<string, IService>();
        }
        
        public T GetService<T>() where T : IService
        {
            var key = typeof(T).Name;
            if (service.ContainsKey(key)) return (T) service[key];
            
            Debug.LogError($"{key} has not been registered in Locator");
            throw new Exception("Service not found");
        }

        public void Register<T>(T service) where T : IService
        {
           var key = typeof(T).Name;
           if (!this.service.ContainsKey(key)) this.service.Add(key, service);
           else
           {
               Debug.LogError("Service already registered");
               throw new Exception("Service already registered");
           }
        }

        public void Unregister<T>() where T : IService
        {
            var key = typeof(T).Name;
            if (service.ContainsKey(key)) { service.Remove(key);}
            else
            {
               Debug.LogError("Service can't be unregistered"); 
               throw new Exception("Service notfound");
            }
            
        }

        private void OnDestroy()
        {
            service.Clear();
        }
    }
}
