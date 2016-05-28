using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace UnityStandardAssets.Network 
{
    public class LobbyInfoPanel : MonoBehaviour
    {
        public Text infoText;
        public Text buttonText;
        public Button singleButton;

        public void Display(string info, string buttonInfo, UnityEngine.Events.UnityAction buttonClbk, bool displayButton = true)
        {
            infoText.text = info;

            singleButton.gameObject.SetActive(displayButton);
            buttonText.text = buttonInfo;

            singleButton.onClick.RemoveAllListeners();

            if (buttonClbk != null)
            {
                singleButton.onClick.AddListener(buttonClbk);
            }

            singleButton.onClick.AddListener(() => { gameObject.SetActive(false); });

            gameObject.SetActive(true);
        }
    }
}