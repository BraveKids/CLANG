using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityStandardAssets.Network {
    public class LobbyTopPanel : MonoBehaviour {
        public bool isInGame = false;

        protected bool isDisplayed = true;
        public GameObject mainLogo;

        void Update () {
            if (Input.GetKeyDown(KeyCode.Escape) && !isInGame) {
                Application.Quit();
            }
            mainLogo.SetActive(!isInGame);
        }

        public void ToggleVisibility (bool visible) {
            isDisplayed = visible;
            foreach (Transform t in transform) {
                t.gameObject.SetActive(isDisplayed);
            }

        }
    }
}