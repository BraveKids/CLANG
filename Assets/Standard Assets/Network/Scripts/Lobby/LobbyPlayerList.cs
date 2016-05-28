using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets.Network
{
    //List of players in the lobby
    public class LobbyPlayerList : MonoBehaviour
    {
        public static LobbyPlayerList _instance = null;

        public RectTransform playerListContentTransform;
        public GameObject warningDirectPlayServer;

        public void Awake()
        {
            _instance = this;
        }

        public void DisplayDirectServerWarning(bool enabled)
        {
            if(warningDirectPlayServer != null)
                warningDirectPlayServer.SetActive(enabled);
        }

        void Update()
        {
            //this dirty the layout to force it to recompute evryframe (a sync problem between client/server
            //sometime to child being assigned before layout was enabled/init, leading to broken layouting)
            VerticalLayoutGroup layout = playerListContentTransform.GetComponent<VerticalLayoutGroup>();
            if(layout)
                layout.childAlignment = Time.frameCount%2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;

            if(Input.GetButtonDown("Fire2") && NetworkServer.localConnections.Count > 0 && NetworkServer.localConnections[0].playerControllers.Count < 2)
            {
                LobbyManager.s_Singleton.TryToAddPlayer();
            }
        }

        public void AddPlayer(LobbyPlayer player)
        {
            player.transform.SetParent(playerListContentTransform, false);
        }
    }
}
