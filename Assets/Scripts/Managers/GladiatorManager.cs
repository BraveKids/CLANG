using System;
using UnityEngine;

[Serializable]
public class GladiatorManager : Manager
{
                   // The player localID (if there is more than 1 player on the same machine)

    public GladiatorMovement m_Movement;        // References to various objects for control during the different game phases.
    public GladiatorShooting m_Shooting;
    public GladiatorHealth m_Health;
  
   

    public override void Setup()
    {
        // Get references to the components.
        
        m_Movement = m_Instance.GetComponent<GladiatorMovement>();
        m_Shooting = m_Instance.GetComponent<GladiatorShooting>();
        m_Health = m_Instance.GetComponent<GladiatorHealth>();
        m_Setup = m_Instance.GetComponent<GladiatorSetup>();

        // Get references to the child objects.
        m_PlayerRenderers = m_Health.m_PlayerRenderers;

        //Set a reference to that amanger in the health script, to disable control when dying
        m_Health.m_Manager = this;

        // Set the player numbers to be consistent across the scripts.
        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Movement.m_LocalID = m_LocalPlayerID;
        
        m_Shooting.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_localID = m_LocalPlayerID;
        
        //setup is use for diverse Network Related sync
        m_Setup.m_Color = m_PlayerColor;
        m_Setup.m_PlayerName = m_PlayerName;
        m_Setup.m_PlayerNumber = m_PlayerNumber;
        m_Setup.m_LocalID = m_LocalPlayerID;
    }


    // Used during the phases of the game where the player shouldn't be able to control their tank.
    public override void DisableControl()
    {
        
        m_Movement.enabled = false;
        m_Shooting.enabled = false;
    }


    // Used during the phases of the game where the player should be able to control their tank.
    public override void EnableControl()
    {
     
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

       
    }

    public override string GetName()
    {
        return m_Setup.m_PlayerName;
    }
    
    public override void SetLeader(bool leader)
    { 
        m_Setup.SetLeader(leader);
    }
    
    public override bool  IsReady()
    {
        return m_Setup.m_IsReady;
    }

    // Used at the start of each round to put the tank into it's default state.
    public override void Reset()
    {
        m_Movement.SetDefaults();
        m_Shooting.SetDefaults();
        m_Health.SetDefaults();

        if (m_Movement.hasAuthority)
        {
            m_Movement.m_Rigidbody.position = m_SpawnPoint.position;
            m_Movement.m_Rigidbody.rotation = m_SpawnPoint.rotation;
        }
    }
}
