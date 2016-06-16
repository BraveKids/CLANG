using UnityEngine;
using System;

[Serializable]
public class StrategistManager : Manager {

    // This class is to manage various settings on a tank.
    // It works with the GameManager class to control how the tanks behave
    // and whether or not players have control of their tank in the 
    // different phases of the game.

                  // The player localID (if there is more than 1 player on the same machine)

    public StrategistSpawner m_Spawner;        // References to various objects for control during the different game phases.
    public StrategistPulse m_Pulse;
    

    public override void Setup()
    {
        // Get references to the components.
        m_Spawner = m_Instance.GetComponent<StrategistSpawner>();
        m_Pulse = m_Instance.GetComponent<StrategistPulse>();
        m_Setup = m_Instance.GetComponent<StrategistSetup>();

        // Get references to the child objects.
        m_PlayerRenderers = m_Pulse.m_PlayerRenderers;

        //Set a reference to that amanger in the health script, to disable control when dying
        m_Pulse.m_Manager = this;

        // Set the player numbers to be consistent across the scripts.
        m_Spawner.m_PlayerNumber = m_PlayerNumber;
        m_Spawner.m_localID = m_LocalPlayerID;

        //setup is use for diverse Network Related sync
        m_Setup.m_Color = m_PlayerColor;
        m_Setup.m_PlayerName = m_PlayerName;
        m_Setup.m_PlayerNumber = m_PlayerNumber;
        m_Setup.m_LocalID = m_LocalPlayerID;
    }



    // Used during the phases of the game where the player shouldn't be able to control their tank.
    public override void DisableControl()
    {
        
        m_Spawner.enabled = false;
        
    }


    // Used during the phases of the game where the player should be able to control their tank.
    public override void EnableControl()
    {
   
        m_Spawner.enabled = true;
      


    }

    public override string GetName()
    {
        return m_Setup.m_PlayerName;
    }
    
    public override void SetLeader(bool leader)
    { 
        m_Setup.SetLeader(leader);
    }
    
    public override bool IsReady()
    {
        return m_Setup.m_IsReady;
    }

    // Used at the start of each round to put the tank into it's default state.
    public override void Reset()
    {
        m_Spawner.SetDefaults();
        m_Pulse.SetDefaults();

        if (m_Spawner.hasAuthority)
        {
            m_Spawner.m_Rigidbody.position = m_SpawnPoint.position;
            m_Spawner.m_Rigidbody.rotation = m_SpawnPoint.rotation;
        }
    }
}


