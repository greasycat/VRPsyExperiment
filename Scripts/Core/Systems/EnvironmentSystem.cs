using Core.Interfaces;
using UnityEngine;

namespace Core.Systems
{
    public class EnvironmentSystem : MonoBehaviour, IService
    {
        public float mazeLightIntensity = 9200f;
        public float maxLightIntensity = 130000f;

        public Light sun;
        private void Awake()
        {
            Locator.instance.Register(this);
        }

        public void SetMaxLightIntensity()
        {
            sun.intensity = maxLightIntensity;
        }

        public void SetMazeLightIntensity()
        {
            sun.intensity = mazeLightIntensity;
        }
        
    }
}