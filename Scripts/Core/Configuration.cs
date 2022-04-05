using System;
using System.Collections.Generic;
using System.IO;
using Core.Interfaces;
using IO;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Systems
{
    [Serializable]
    public class Configuration : MonoBehaviour, IService
    {
        [Serializable]
        public struct SerializableConfiguration
        {
            public string trialListFilePath;
            public string outputFilePath;
        }

        public SerializableConfiguration config;
        public string defaultConfigurationPath = "./config.json";

        [SerializeField] private TitleScreen titleScreen;

        private void Awake()
        {
            Locator.instance.Register(this);
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            if (!ReadConfiguration())
            {
                titleScreen.SetText("No Configuration File in the current folder,\n Consider loading it");
                titleScreen.SetAllButton(false);
#if UNITY_EDITOR && DEBUG
                Debug.LogError("Fail to load config");
#endif
            }
            else
            {
                titleScreen.SetText("Default configuration loaded successfully");
                titleScreen.SetAllButton(true);
#if UNITY_EDITOR && DEBUG
                Debug.Log("Config loaded");
#endif
            }
        }

        public void GetConfiguration()
        {
            FileUtils.GetSingleFileFromDialog(GetFileCallBack);
        }

        private void CopyFromConfiguration(SerializableConfiguration configuration)
        {
            config = configuration;
        }

        private bool ReadConfiguration()
        {
            if (!FileUtils.CheckIfFileExists(defaultConfigurationPath))
            {
                return false;
            }

            try
            {
                var output = FileUtils.ReadAll(defaultConfigurationPath);
                var json = JsonUtility.FromJson<SerializableConfiguration>(output);
                CopyFromConfiguration(json);
                Locator.instance.GetService<ExperimentSystem>().LoadTrialInfo(config.trialListFilePath);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        private void WriteConfiguration()
        {
            FileUtils.WriteAll(defaultConfigurationPath, JsonUtility.ToJson(config, true));
        }

        private void GetFileCallBack(IReadOnlyList<string> path)
        {
            if (path.Count <= 0) return;
            defaultConfigurationPath = path[0];
            if (ReadConfiguration())
            {
                titleScreen.SetText("Custom configuration loaded successfully");
                titleScreen.SetAllButton(true);
            }
            else
            {
                titleScreen.SetText("Custom configuration fail to load, please check the file!");
                titleScreen.SetAllButton(false);
            }
        }

        private void OnDestroy()
        {
            WriteConfiguration();
        }
    }
}