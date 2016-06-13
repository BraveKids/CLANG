using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets.Network {
    public class LobbyAutoJoinServerList : MonoBehaviour {
        public LobbyManager lobbyManager;
        public List<LobbyServerProperties> serverList;

        protected int currentPage = 0;
        protected int previousPage = 0;

        static Color OddServerColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        static Color EvenServerColor = new Color(.94f, .94f, .94f, 1.0f);

        public float serverRefreshRate = 5.0f;
        public float connectingRefreshRate = 5.0f;

        private int serverIndex = 0;

        void OnEnable () {
            currentPage = 0;
            previousPage = 0;

            //initialize list on enable
            serverList = new List<LobbyServerProperties>();

            StartCoroutine(RefreshServerList());
        }

        IEnumerator RefreshServerList () {
            while (!lobbyManager.isMatchmaking) {
                print("Refreshing server list...");
                RequestPage(currentPage);
                yield return new WaitForSeconds(serverRefreshRate);
            }
        }

        public void PopulateServerListCallback (ListMatchResponse response) {
            if (response.matches.Count == 0) {
                if (currentPage == 0) {
                    DestroyServerEntries();
                }

                currentPage = previousPage;
                return;
            }

            DestroyServerEntries();

            for (int i = 0; i < response.matches.Count; ++i) {
                LobbyServerProperties server = new LobbyServerProperties(response.matches[i], (i % 2 == 0) ? OddServerColor : EvenServerColor);
                serverList.Add(server);
            }

            serverIndex = 0;
            StartCoroutine(AutoJoin());
        }

        IEnumerator AutoJoin () {
            while (!lobbyManager.isMatchmaking && serverList.Count != 0) {
                if (serverIndex == serverList.Count)
                    serverIndex = 0;
                TryToJoinMatch(serverList[serverIndex++]);
                yield return new WaitForSeconds(connectingRefreshRate);
            }
        }

        void TryToJoinMatch (LobbyServerProperties server) {
            print("Auto joining server: " + server);
            lobbyManager.matchMaker.JoinMatch(server.networkID, "", OnMatchJoined);
        }

        public void OnMatchJoined (JoinMatchResponse response) {
            if (response.success) {
                Debug.Log("Join match succeeded: " + response);
                lobbyManager.OnMatchJoined(response);
                lobbyManager.DisplayIsConnecting();
                lobbyManager.backDelegate = lobbyManager.StopClientClbk;
                lobbyManager.isMatchmaking = true;

                //IN CASE OF EMERGENCY BREAK THE GLASS
                /*matchJoined = true;
                matchInfo = new MatchInfo(matchJoin);
                Utility.SetAccessTokenForNetwork(matchJoin.networkId, new NetworkAccessToken(matchJoin.accessTokenString)); ;
                StartClient(matchInfo);*/

            } else {
                Debug.LogWarning("Join match failed: " + response);
                lobbyManager.isMatchmaking = false;
            }
        }

        public void ChangePage (int dir) {
            int newPage = Mathf.Max(0, currentPage + dir);

            //if we have no server currently displayed, need we need to refresh page0 first instead of trying to fetch any other page
            if (serverList.Count == 0)
                newPage = 0;

            RequestPage(newPage);
        }

        public void RequestPage (int page) {
            previousPage = currentPage;
            currentPage = page;
            lobbyManager.matchMaker.ListMatches(page, 6, "", PopulateServerListCallback);
        }

        private void DestroyServerEntries () {
            //destroy every serverlistrect entry if there's no server
            serverList.Clear();
        }
    }
}
