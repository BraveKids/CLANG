using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameTimer : NetworkBehaviour
{
    [SyncVar]
    public float gameTime; //The length of a game, in seconds.
    [SyncVar]
    public float timer; //How long the game has been running. -1=waiting for players, -2=game is done
    [SyncVar]
    public int minPlayers; //Number of players required for the game to start
    [SyncVar]
    public bool masterTimer = false; //Is this the master timer?
                                     //public ServerTimer timerObj;
    public GameObject timerCanvas;
    GameTimer serverTimer;
    
    
    public override void OnStartLocalPlayer()
    {
        timerCanvas = GameObject.FindGameObjectWithTag("Timer");
    }

    void Start()
    {
        



        if (isServer)
        { // For the host to do: use the timer and control the time.
            if (isLocalPlayer)
            {
                serverTimer = this;
                masterTimer = true;
            }
        }
        else if (isLocalPlayer)
        { //For all the boring old clients to do: get the host's timer.
            GameTimer[] timers = FindObjectsOfType<GameTimer>();
            for (int i = 0; i < timers.Length; i++)
            {
                if (timers[i].masterTimer)
                {
                    serverTimer = timers[i];
                }
            }
            
        }
    }
    void Update()
    {
        if(GameElements.getGladiator() == null || GameElements.getStrategist() == null)
        {
            return;
        }
        if (masterTimer)
        { //Only the MASTER timer controls the time
            if (timer >= gameTime)
            {
                timer = -2;
            }
            else if (timer == -1)
            {
                if (NetworkServer.connections.Count >= minPlayers)
                {
                    timer = 0;
                }
            }
            else if (timer == -2 && !GameManager.s_Instance.endGame)
            {
                GameManager.s_Instance.winner = "GLADIATOR";
                GameManager.s_Instance.SetGameWinner(GameElements.getGladiator());
                GameManager.s_Instance.setEndGame(true);
            }
            else
            {
                timer += Time.deltaTime;
            }

            
        }
       

        if (isLocalPlayer)
        { //EVERYBODY updates their own time accordingly.
            if (serverTimer)
            {
                gameTime = serverTimer.gameTime;
                timer = serverTimer.timer;
                minPlayers = serverTimer.minPlayers;
                if (timer <= 90f)
                {
                    timerCanvas.GetComponent<Text>().text = "" + (90 - (int)timer);
                }



            }
            else
            { //Maybe we don't have it yet?
                GameTimer[] timers = FindObjectsOfType<GameTimer>();
                for (int i = 0; i < timers.Length; i++)
                {
                    if (timers[i].masterTimer)
                    {
                        serverTimer = timers[i];
                    }
                }
            }
        }
    }
}