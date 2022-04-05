using System.Collections;
using Core.Systems;
using Event;
using UnityEngine;

namespace Core.Mechanics
{
    public class Interactable : MonoBehaviour
    {
        [SerializeField] protected ScriptableEvent triggerEnterScriptableEvent;
        [SerializeField] protected ScriptableEvent triggerExitScriptableEvent;

        public Interactable()
        {
            Locator.instance.GetService<ExperimentSystem>().AddInteractable(this);
        }

        public IEnumerator WaitAndHide(float time)
        {
            yield return new WaitForSeconds(time);
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        
    }
}