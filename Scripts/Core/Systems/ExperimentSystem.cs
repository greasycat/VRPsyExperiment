using System;
using System.Collections.Generic;
using Core.Interfaces;
using Core.Mechanics;
using Experiment;
using Experiment.GraphTask;
using IO;
using UnityEngine;

namespace Core.Systems
{
    public class ExperimentSystem : MonoBehaviour, IService
    {
        protected List<TrialInfo> trialInfos = new List<TrialInfo>();
        protected HashSet<Interactable> interactables = new HashSet<Interactable>();

        protected string TrialListFilePath;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            Locator.instance.Register(this);
            DontDestroyOnLoad(this.gameObject);
            Debug.Log(this.GetType());
        }

        public void AddInteractable(Interactable interactable)
        {
            interactables.Add(interactable);
        }
        

        public void LoadTrialInfo(string path)
        {
            if (!FileUtils.CheckIfFileExists(path))
                return;
            var csv = new Csv(path, CsvFileMode.CsvRead, 1);
            
            trialInfos = new List<TrialInfo>();
            foreach (var info in csv.ReadTrialInfo<GraphTaskTrialInfo>())
            {
                trialInfos.Add(info);
            }
        }
        
        private void CleanTrialInfo()
        {
            trialInfos.Clear();
        }
    }
}