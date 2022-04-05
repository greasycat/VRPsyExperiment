using System;
using Core.Interfaces;
using Core.Systems;
using UnityEngine;

namespace Core
{
    public class MazeSystem : MonoBehaviour, IService
    {
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private void Awake()
        {
            Locator.instance.Register(this);
        }

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
        }

        public void ClearMaze()
        {
            foreach (Transform child in this.transform)
            {
                child.gameObject.SetActive(false);
            }
            meshRenderer.enabled = false;
            meshCollider.enabled = false;
            Locator.instance.GetService<EnvironmentSystem>().SetMaxLightIntensity();
        }

        public void ResetMaze()
        {
            
        }
    }
}