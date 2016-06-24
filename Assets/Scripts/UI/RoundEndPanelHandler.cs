using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityStandardAssets.Network
{
    public class RoundEndPanelHandler : MonoBehaviour
    {
        float leftWaitTime = 5.0f;
        int flooredWaitTime = 5;
        public Text m_MessageText;
        public LobbyManager lobbyManager;
        public RectTransform mainMenuPanel;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(countDownTimer());
        }

        IEnumerator countDownTimer()
        {
            while (leftWaitTime > 0.0f)
            {
                yield return null;

                leftWaitTime -= Time.deltaTime;

                int newFlooredWaitTime = Mathf.FloorToInt(leftWaitTime);

                if (newFlooredWaitTime != flooredWaitTime)
                {
                    flooredWaitTime = newFlooredWaitTime;
                    string message = EndMessage(flooredWaitTime);
                    m_MessageText.text = message;
                }
            }
            lobbyManager.ChangeTo(mainMenuPanel);

        }

        private string EndMessage(int waitTime)
        {
            // By default, there is no winner of the round so it's a draw.
            string message = "GLADIATOR WINS THE GAME!";

            // After either the message of a draw or a winner, add some space before the leader board.
            message += "\n\n";

            message += "\nReturn to lobby in " + waitTime;

            return message;
        }
    }
}
