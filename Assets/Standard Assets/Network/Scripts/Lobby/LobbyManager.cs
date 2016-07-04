using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets.Network
{
    public class LobbyManager : NetworkLobbyManager 
    {
        Dictionary<int, int> currentPlayers;
        const int STRATEGIST = 0;
        const int GLADIATOR = 1;
        static public LobbyManager s_Singleton;
        
        [Tooltip("The minimum number of players in the lobby before player can be ready")]
        public int minPlayer;

        public LobbyTopPanel topPanel;
        public Image background;
        
        public RectTransform mainMenuPanel;
        public RectTransform lobbyPanel;
        public RectTransform creditsPanel;
        public RectTransform roundEndPanel;

        public LobbyInfoPanel infoPanel;

        protected RectTransform currentPanel;

        public Button backButton;

        public Text statusInfo;
        public Text hostInfo;
       
        //used to disconnect a client properly when exiting the matchmaker
        public bool isMatchmaking = false;
        protected bool _disconnectServer = false;
        
        protected ulong _currentMatchID;

        protected LobbyHook _lobbyHooks;
        public bool endGame = false;
        public bool disconnectedStrategist = false;

        void Awake()
        {
            if (FindObjectsOfType<LobbyManager>().Length > 1)
                Destroy(gameObject);
        }

        void Start()
        {
            s_Singleton = this;

            _lobbyHooks = GetComponent<UnityStandardAssets.Network.LobbyHook>();
            currentPanel = mainMenuPanel;

            backButton.gameObject.SetActive(false);
            GetComponent<Canvas>().enabled = true;

            background = GetComponent<Image>();

            DontDestroyOnLoad(gameObject);
            currentPlayers = new Dictionary<int, int>();
            SetServerInfo("Offline", "None");
        }

        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            if (!conn.playerControllers[0].unetView.isLocalPlayer)
                return;

            if (SceneManager.GetActiveScene().name == lobbyScene)
            {
                background.enabled = true;
                if (topPanel.isInGame)
                {
                    
                    if (isMatchmaking)
                    {                        
                        if (conn.playerControllers[0].unetView.isServer)
                        {
                            backDelegate = StopHostClbk;
                        }
                        else
                        {
                            backDelegate = StopClientClbk;
                        }
                    }
                    else
                    {
                        if (conn.playerControllers[0].unetView.isClient)
                        {
                            backDelegate = StopHostClbk;
                        }
                        else
                        {
                            backDelegate = StopClientClbk;
                        }
                    }
                    GoBackButton(); //force to exit from lobby
                }
                else
                {
                    Debug.Log("menu da Lobby manager - OnlobbyClientSceneChanged");
                    ChangeTo(mainMenuPanel);
                }

                
                topPanel.ToggleVisibility(true);
                topPanel.isInGame = false;
            }
            else
            {
                ChangeTo(null);

                Destroy(GameObject.Find("MainMenuUI(Clone)"));
                background.enabled = false;

                backDelegate = StopGameClbk;
                topPanel.isInGame = true;
                topPanel.ToggleVisibility(false);
            }
        }

        public void ChangeTo(RectTransform newPanel)
        {
            if (currentPanel != null)
            {
                currentPanel.gameObject.SetActive(false);
            }

            if (newPanel != null)
            {
                newPanel.gameObject.SetActive(true);
            }

            currentPanel = newPanel;

            if (currentPanel != mainMenuPanel)
            {
                backButton.gameObject.SetActive(true);
            }
            else
            {
                backButton.gameObject.SetActive(false);
                SetServerInfo("Offline", "None");
                isMatchmaking = false;
                background.enabled = true;
                topPanel.isInGame = false;
            }

            if (newPanel == creditsPanel) {
                backDelegate = SimpleBackClbk;
            }

            if (newPanel == roundEndPanel)
            {
                backButton.gameObject.SetActive(false);
                topPanel.isInGame = false;
                background.enabled = true;
            }
        }

        public void DisplayIsConnecting()
        {
            var _this = this;
            infoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
        }

        public void SetServerInfo(string status, string host)
        {
            statusInfo.text = status;
            hostInfo.text = host;
        }


        public delegate void BackButtonDelegate();
        public BackButtonDelegate backDelegate;
        public void GoBackButton()
        {
            backDelegate();
        }

        // ----------------- Server management

        public void SimpleBackClbk()
        {

            ChangeTo(mainMenuPanel);
        }

        public void StopHostClbk()
        {
            if (isMatchmaking)
            {
                this.matchMaker.DestroyMatch((NetworkID)_currentMatchID, OnMatchDestroyed);
                _disconnectServer = true;
            }
            else
            {
                StopHost();
            }

            ChangeTo(mainMenuPanel);
        }

        public void StopClientClbk()
        {
            StopClient();

            if (isMatchmaking)
            {
                StopMatchMaker();
            }

            ChangeTo(mainMenuPanel);
        }

        public void StopServerClbk()
        {
    
            StopServer();
            ChangeTo(mainMenuPanel);
        }

        public void StopGameClbk()
        {
            SendReturnToLobby();
            ChangeTo(lobbyPanel);
        }

        //===================

        public override void OnStartHost()
        {
            base.OnStartHost();

            ChangeTo(lobbyPanel);
            backDelegate = StopHostClbk;
            SetServerInfo("Hosting", networkAddress);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            infoPanel.gameObject.SetActive(false);

            if (!NetworkServer.active)
            {//only to do on pure client (not self hosting client)
                ChangeTo(lobbyPanel);
                backDelegate = StopClientClbk;
                SetServerInfo("Client", networkAddress);
            }
        }

        public override void OnMatchCreate(UnityEngine.Networking.Match.CreateMatchResponse matchInfo)
        {
            base.OnMatchCreate(matchInfo);

            _currentMatchID = (System.UInt64)matchInfo.networkId;
        }

        public void OnMatchDestroyed(BasicResponse resp)
        {
            if (_disconnectServer)
            {
                StopMatchMaker();
                StopHost();
            }
        }

        // ----------------- Server callbacks ------------------

        //we want to disable the button JOIN if we don't have enough player
        //But OnLobbyClientConnect isn't called on hosting player. So we override the lobbyPlayer creation
        

        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

            AutoLobbyPlayer newPlayer = obj.GetComponent<AutoLobbyPlayer>();

            if (!currentPlayers.ContainsKey(conn.connectionId))
            {
                currentPlayers.Add(conn.connectionId, numPlayers);
            }

            newPlayer.RpcToggleJoinButton(numPlayers + 1 >= minPlayer); ;

            for (int i = 0; i < numPlayers; ++i)
            {
                AutoLobbyPlayer p = lobbySlots[i] as AutoLobbyPlayer;

                if (p != null)
                {
                    p.RpcToggleJoinButton(numPlayers + 1 >= minPlayer);
                }
            }

            return obj;
        }

        public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
        {
            int index = currentPlayers[conn.connectionId];
            GameObject _temp = Instantiate(spawnPrefabs[index], startPositions[conn.connectionId].position, Quaternion.identity) as GameObject;

            return _temp;
        }

        public override void OnLobbyServerDisconnect(NetworkConnection conn)
        {
            for (int i = 0; i < numPlayers; ++i)
            {
                AutoLobbyPlayer p = lobbySlots[i] as AutoLobbyPlayer;

                if (p != null)
                {
                    p.RpcToggleJoinButton(numPlayers >= minPlayer);
                }
            }

        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            //This hook allows you to apply state data from the lobby-player to the game-player
            //just subclass "LobbyHook" and add it to the lobby object.

            if (_lobbyHooks)
                _lobbyHooks.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

            return true;
        }


        // --- Countdown management

        static protected float _matchStartCountdown = 5.0f;

        public override void OnLobbyServerPlayersReady()
        {
            StartCoroutine(ServerCountdownCoroutine());
        }

        public IEnumerator ServerCountdownCoroutine()
        {
            float remainingTime = _matchStartCountdown;
            int floorTime = Mathf.FloorToInt(remainingTime);

            while(remainingTime > 0)
            {
                yield return null;

                remainingTime -= Time.deltaTime;
                int newFloorTime = Mathf.FloorToInt(remainingTime);

                if(newFloorTime != floorTime)
                {//to avoid flooding the network of message, we only send a notice to cleint when the number of second do change.
                    floorTime = newFloorTime;

                    for (int i = 0; i < lobbySlots.Length; ++i)
                    {
                        if (lobbySlots[i] != null)
                        {//there is max player slot, so some could be == null, need to test it ebfore accessing!
                            (lobbySlots[i] as AutoLobbyPlayer).RpcUpdateCountdown(floorTime);
                        }
                    }
                }
            }

            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i] != null)
                {
                    (lobbySlots[i] as AutoLobbyPlayer).RpcUpdateCountdown(0);
                }
            }

            ServerChangeScene(playScene);
        }



        // ----------------- Client callbacks ------------------

        public override void OnClientDisconnect(NetworkConnection conn)
        {

            
            if (endGame)
            {
                Debug.Log("Fine normale");
                base.OnClientDisconnect(conn);
                ChangeTo(mainMenuPanel);
            }
            else
            {
                Debug.Log("Fine brutale");
                ChangeTo(roundEndPanel);
            }

        }

        public override void OnStopHost()
        {
   

            base.OnStopHost();
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            
            ChangeTo(mainMenuPanel);
            infoPanel.Display("Client error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
        }
    }
}
