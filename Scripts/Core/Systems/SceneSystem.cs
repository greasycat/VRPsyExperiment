using Core.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Systems
{
    public class SceneSystem : MonoBehaviour, IService
    {
        private void Awake()
        {
            Locator.instance.Register(this); 
            DontDestroyOnLoad(this.gameObject); 
        }

        public void LoadMaze()
        {
            SceneManager.LoadScene("Maze");
        }

        public void LoadTitleScreen()
        {
            SceneManager.LoadScene("TitleScreen");
        }
    }
}