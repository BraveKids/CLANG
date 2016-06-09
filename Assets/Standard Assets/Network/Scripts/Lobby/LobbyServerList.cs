using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

namespace UnityStandardAssets.Network
{
    public class LobbyServerList : MonoBehaviour
    {
        public LobbyManager lobbyManager;

        public RectTransform serverListRect;
        public GameObject serverEntryPrefab;
        public GameObject noServerFound;

        protected int currentPage = 0;
        protected int previousPage = 0;

        static Color OddServerColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        static Color EvenServerColor = new Color(.94f, .94f, .94f, 1.0f);

        void OnEnable()
        {
            currentPage = 0;
            previousPage = 0;

            //destroy every entry on start
            DestroyServerEntries();

            noServerFound.SetActive(false);

            RequestPage(0);
        }

        void Update () {
            //this dirty the layout to force it to recompute evryframe (a sync problem between client/server
            //sometime to child being assigned before layout was enabled/init, leading to broken layouting)
            RequestPage(currentPage);
        }

        public void OnGUIMatchList(ListMatchResponse response)
        {
            if (response.matches.Count == 0)
            {
                if (currentPage == 0)
                {
                    noServerFound.SetActive(true);
                    DestroyServerEntries();
                }

                currentPage = previousPage;
               
                return;
            }

            noServerFound.SetActive(false);
            DestroyServerEntries();

            for (int i = 0; i < response.matches.Count; ++i)
            {
                GameObject o = Instantiate(serverEntryPrefab) as GameObject;

                o.GetComponent<LobbyServerEntry>().Populate(response.matches[i], lobbyManager, (i%2 == 0) ? OddServerColor : EvenServerColor);

                o.transform.SetParent(serverListRect, false);
            }
        }

        public void ChangePage(int dir)
        {
            int newPage = Mathf.Max(0, currentPage + dir);

            //if we have no server currently displayed, need we need to refresh page0 first instead of trying to fetch any other page
            if (noServerFound.activeSelf)
                newPage = 0;

            RequestPage(newPage);
        }

        public void RequestPage(int page)
        {
            previousPage = currentPage;
            currentPage = page;
            lobbyManager.matchMaker.ListMatches(page, 6, "", OnGUIMatchList);
        }

        private void DestroyServerEntries() {
            //destroy every serverlistrect entry if there's no server
            foreach (Transform t in serverListRect)
                Destroy(t.gameObject);
        }
    }
}