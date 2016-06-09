using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GladiatorHealth : NetworkBehaviour
{
    public float m_StartingHealth = 32f;
    public float m_Resistance = 14f;
    // The amount of health each tank starts with.
    public float m_StartingArmor = 0f;
    public Slider m_Slider;
    public GameObject model;
    Color curColor;
    // The slider to represent how much health the tank currently has.
    public Image m_FillImage;                         // The image component of the slider.
    public Color m_FullHealthColor = Color.green;     // The color the health bar will be when on full health.
    public Color m_ZeroHealthColor = Color.red;       // The color the health bar will be when on no health.
    //public AudioClip m_TankExplosion;                 // The clip to play when the tank explodes.
    //public ParticleSystem m_ExplosionParticles;       // The particle system the will play when the tank is destroyed.
    public GameObject m_PlayerRenderers;                // References to all the gameobjects that need to be disabled when the tank is dead.
    //public GameObject m_HealthCanvas;
    //public GameObject m_AimCanvas;
    //public GameObject m_LeftDustTrail;
    //public GameObject m_RightDustTrail;
    public GladiatorSetup m_Setup;
    public GladiatorManager m_Manager;                   //Associated manager, to disable control when dying.

    [SyncVar(hook = "OnCurrentHealthChanged")]
    public float m_CurrentHealth;                  // How much health the tank currently has.*
    [SyncVar]
    private float m_Armor;
    [SyncVar]
    private bool m_ZeroHealthHappened;              // Has the tank been reduced beyond zero health yet?
    private BoxCollider m_Collider;                 // Used so that the tank doesn't collide with anything when it's dead.


    private void Awake()
    {
        curColor = model.GetComponent<SkinnedMeshRenderer>().material.color;
        m_Collider = GetComponent<BoxCollider>();
    }


    // This is called whenever the tank takes damage.
    public void Damage(float amount)
    {
        float calculatedDamage = amount - (m_Resistance * 0.15f);
        if (calculatedDamage <= 0.0f)
        {
            return;
        }
        if (m_Armor > 0)
        {
            
            if (m_Armor >= calculatedDamage)
            {
                m_Armor -= calculatedDamage;

            }
            else
            {
                m_CurrentHealth -= (calculatedDamage- m_Armor);
                SetArmor(0f);
            }          
        }

        // Reduce current health by the amount of damage done.
        m_CurrentHealth -= calculatedDamage;
        DamageColor();
        // If the current health is at or below zero and it has not yet been registered, call OnZeroHealth.
        if (m_CurrentHealth <= 0f && !m_ZeroHealthHappened)
        {
            OnZeroHealth();
        }
    }
    private void DamageColor()
    {
        
        model.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
        Invoke("DamageColorBack", 0.5f);
       
    }

    private void DamageColorBack()
    {
        model.GetComponent<SkinnedMeshRenderer>().material.color = curColor;
    }

    


    public void SetArmor(float value)
    {
        this.m_Armor = value;
    }

    public float GetArmor()
    {
        return m_Armor;
    }
    
    private void SetHealthUI()
    {
        // Set the slider's value appropriately.
        //m_Slider.value = m_CurrentHealth;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        //m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    void OnCurrentHealthChanged(float value)
    {
        m_CurrentHealth = value;
        // Change the UI elements appropriately.
        SetHealthUI();

    }

    private void OnZeroHealth()
    {
        // Set the flag so that this function is only called once.
        m_ZeroHealthHappened = true;
        GameManager.s_Instance.winner = "STRATEGIST";
        GameManager.s_Instance.SetGameWinner(GameElements.getStrategist());
        GameManager.s_Instance.endGame = true;
        InternalOnZeroHealth();
        //RpcOnZeroHealth();
    }

    private void InternalOnZeroHealth()
    {

        // Disable the collider and all the appropriate child gameobjects so the tank doesn't interact or show up when it's dead.
        SetPlayerActive(false);
    }

    [ClientRpc]
    private void RpcOnZeroHealth()
    {
        // Play the particle system of the tank exploding.
        //m_ExplosionParticles.Play();

        // Create a gameobject that will play the tank explosion sound effect and then destroy itself.
        //AudioSource.PlayClipAtPoint(m_TankExplosion, transform.position);

        InternalOnZeroHealth();
    }

    private void SetPlayerActive(bool active)
    {
        m_Collider.enabled = active;

        m_PlayerRenderers.SetActive(active);
        //m_HealthCanvas.SetActive(active);
        //m_AimCanvas.SetActive(active);
        //m_LeftDustTrail.SetActive(active);
        //m_RightDustTrail.SetActive(active);

        if (active) m_Manager.EnableControl();
        else m_Manager.DisableControl();
        //gameObject.SetActive(false);
        //m_Setup.ActivateCrown(active);
    }

    // This function is called at the start of each round to make sure each tank is set up correctly.
    public void SetDefaults()
    {
        m_CurrentHealth = m_StartingHealth;
        m_ZeroHealthHappened = false;
        SetPlayerActive(true);
    }
}
