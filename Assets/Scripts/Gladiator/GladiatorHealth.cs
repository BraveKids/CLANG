using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GladiatorHealth : NetworkBehaviour
{
    public float m_StartingHealth = 32f;
    public float m_Resistance = 14f;
    // The amount of health each tank starts with.
    public float m_MaxArmor = 16f;
    public Slider m_Slider;
    public GameObject model;
    Color curColor;
    // The slider to represent how much health the tank currently has.
    public Image healthBar;                         // The image component of the slider.
    public Image armorBar;
    public Color m_FullHealthColor = Color.green;     // The color the health bar will be when on full health.
    public Color m_ZeroHealthColor = Color.red;       // The color the health bar will be when on no health.
    public GameObject m_PlayerRenderers;                // References to all the gameobjects that need to be disabled when the tank is dead.
    public GladiatorSetup m_Setup;
    public GladiatorManager m_Manager;                   //Associated manager, to disable control when dying.
    [SyncVar(hook = "OnCurrentHealthChanged")]
    public float m_CurrentHealth;                  // How much health the tank currently has.*
    [SyncVar(hook = "OnCurrentArmorChanged")]
    public float m_Armor;
    [SyncVar]
    private bool m_ZeroHealthHappened;              // Has the tank been reduced beyond zero health yet?
    private BoxCollider m_Collider;                 // Used so that the tank doesn't collide with anything when it's dead.
    bool invulnerable;
    Animator anim;
    NetworkAnimator netAnim;
    GladiatorShooting attackScript;
    GladiatorMovement movementScript;

    void Start()
    {
        invulnerable = false;
        curColor = model.GetComponent<SkinnedMeshRenderer>().material.color;
        m_Collider = GetComponent<BoxCollider>();
        GameElements.getStrategist().GetComponent<CrowdIA>().enabled = true;
        if (isLocalPlayer)
        {
            healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Image>();
            armorBar = GameObject.FindGameObjectWithTag("ArmorBar").GetComponent<Image>();
        }
        attackScript = GetComponent<GladiatorShooting>();
        anim = GetComponent<Animator>();
    }
    public void Recover(float amount)
    {
        if (m_CurrentHealth < m_StartingHealth)
        {
            if (m_CurrentHealth + amount > m_StartingHealth)
            {
                m_CurrentHealth = m_StartingHealth;
            }
            else
            {
                m_CurrentHealth += amount;
            }
        }
    }

    [Command]
    public void CmdSetAnimTrigger(string triggerName)
    {
        if (!isServer)
        {
            anim.SetTrigger(triggerName);
        }
        RpcSetAnimTrigger(triggerName);
    }

    [ClientRpc]
    public void RpcSetAnimTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }

    // This is called whenever the tank takes damage.
    public void Damage(float amount)
    {
        if (!invulnerable)
        {
            attackScript.damaged = true;
            CmdSetAnimTrigger("Damage");
            Invoke("NotDamaged", 1f);
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
                    m_CurrentHealth -= (calculatedDamage - m_Armor);
                    SetArmor(0f);
                }
            }
            else
            {

                // Reduce current health by the amount of damage done.
                m_CurrentHealth -= calculatedDamage;
                invulnerable = true;
                Invoke("Vulnerable", 2f);
            }
            DamageColor();
            // If the current health is at or below zero and it has not yet been registered, call OnZeroHealth.
            if (m_CurrentHealth <= 0f && !m_ZeroHealthHappened)
            {
                movementScript.setAttacking(true);
                attackScript.damaged = true;

                CmdSetAnimTrigger("Death");

                Invoke("OnZeroHealth", 2f);
            }
        }
    }
    void NotDamaged()
    {
        //movementScript.setAttacking(false);
        attackScript.damaged = false;
    }

    void Vulnerable()
    {
        invulnerable = false;
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
        if (isLocalPlayer)
        {
            //float value = m_CurrentHealth / m_StartingHealth;
            //healthBar.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z);
            healthBar.fillAmount = mapValueTo01(m_CurrentHealth, 0f, m_StartingHealth);
        }
    }

    // Maps a value from some arbitrary range to the 0 to 1 range
    public static float mapValueTo01(float value, float min, float max) {
        return (value - min) * 1f / (max - min);
    }

    private void SetArmorUI()
    {
        if (isLocalPlayer)
        {
            //float value = m_Armor / m_MaxArmor;
            //armorBar.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z);
            armorBar.fillAmount = mapValueTo01(m_Armor, 0, m_MaxArmor);
        }
    }


    void OnCurrentHealthChanged(float value)
    {
        m_CurrentHealth = value;
        // Change the UI elements appropriately.
        SetHealthUI();

    }

    void OnCurrentArmorChanged(float value)
    {
        m_Armor = value;
        // Change the UI elements appropriately.
        SetArmorUI();

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
        m_Armor = 0f;
        m_CurrentHealth = m_StartingHealth;
        m_ZeroHealthHappened = false;
        SetPlayerActive(true);
    }

    public float getLife()
    {
        return m_CurrentHealth;
    }

    public float getArmor()
    {
        return m_Armor;
    }

    public float getMaxLife()
    {
        return m_StartingHealth;
    }

    public float getMaxArmor()
    {
        return m_MaxArmor;
    }
}
