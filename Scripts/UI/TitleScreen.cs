using Core;
using Core.Systems;
using IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TitleScreen : MonoBehaviour
    {

        
        [SerializeField] private Button[] buttons;
        [SerializeField] private TextMeshProUGUI text;
        public void SetAllButton(bool active)
        {
            foreach (var button in buttons)
            {
                button.interactable = active;
            }
        }

        public void SetText(string msg)
        {
            text.text = msg;
        }
        
    }
}