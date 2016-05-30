using System;
using UnityEngine;
[Serializable]
public abstract class Manager {
    // This class is to manage various settings on a tank.
    // It works with the GameManager class to control how the tanks behave
    // and whether or not players have control of their tank in the 
    // different phases of the game.

    public Color m_PlayerColor;               // This is the color this tank will be tinted.
    public Transform m_SpawnPoint;            // The position and direction the tank will have when it spawns.
    [HideInInspector]
    public int m_PlayerNumber;                // This specifies which player this the manager for.
    [HideInInspector]
    public GameObject m_Instance;             // A reference to the instance of the tank when it is created.
    [HideInInspector]
    public GameObject m_PlayerRenderers;        // The transform that is a parent of all the tank's renderers.  This is deactivated when the tank is dead.
    [HideInInspector]
    public int m_Wins;                        // The number of wins this player has so far.
    [HideInInspector]
    public string m_PlayerName;                    // The player name set in the lobby
    [HideInInspector]
    public int m_LocalPlayerID;
    public PlayerSetup m_Setup;


    public abstract bool IsReady();

    public abstract void Setup();
    
    public abstract void DisableControl();

    public abstract void EnableControl();

    public abstract string GetName();

    public abstract void SetLeader(bool leader);

    public abstract void Reset();

}
