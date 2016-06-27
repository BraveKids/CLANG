using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class StrategistPulse : NetworkBehaviour {
    public float m_StartingPulse = 5.0f;

    // The amount of health each tank starts with.
    public float m_MaxPulse = 10.0f;
    public float pulseRegenerationTime = 3.0f;
    //public Slider m_Slider;                           // The slider to represent how much health the tank currently has.
    //public Image m_FillImage;                         // The image component of the slider.
    //public Color m_FullHealthColor = Color.green;     // The color the health bar will be when on full health.
    //public Color m_ZeroHealthColor = Color.red;       // The color the health bar will be when on no health.
    //public AudioClip m_TankExplosion;                 // The clip to play when the tank explodes.
    //public ParticleSystem m_ExplosionParticles;       // The particle system the will play when the tank is destroyed.
    public GameObject m_PlayerRenderers;
    public GameObject strategistCanvas;
    public Image pulseBar;
    public Text pulseText;
    // References to all the gameobjects that need to be disabled when the tank is dead.
    //public GameObject m_HealthCanvas;
    //public GameObject m_AimCanvas;
    //public GameObject m_LeftDustTrail;
    //public GameObject m_RightDustTrail;
    public StrategistSetup m_Setup;
    public StrategistManager m_Manager;                   //Associated manager, to disable control when dying.

    [SyncVar(hook = "OnCurrentPulseChanged")]
    public float m_CurrentPulse = 5.0f;                  // How much health the tank currently has.*
    [SyncVar]
    private bool m_ZeroHealthHappened;              // Has the tank been reduced beyond zero health yet?
                                                    //private BoxCollider m_Collider;
                                                    // Used so that the tank doesn't collide with anything when it's dead.


    private void Awake () {


        //m_Collider = GetComponent<BoxCollider>();
    }

    private void Start () {
        m_CurrentPulse = m_StartingPulse;
        strategistCanvas = GameObject.FindGameObjectWithTag("StrategistCanvas");
        pulseBar = GameObject.FindGameObjectWithTag("Pulse").GetComponent<Image>();
        pulseText = GameObject.Find("PulseText").GetComponent<Text>();

        //SetHealthUI(m_CurrentPulse);
        StartCoroutine(addPulse());
    }


    IEnumerator addPulse () {
        while (true) { // loops forever...
            if (m_CurrentPulse < m_MaxPulse) { // if health < 100...
                m_CurrentPulse += 1; // increase health and wait the specified time
                SetPulseUI(m_CurrentPulse);
                yield return new WaitForSeconds(pulseRegenerationTime);
            } else { // if health >= 100, just yield 
                yield return null;
            }
        }
    }

    // This is called whenever the tank takes damage.
    public void SpawnPrice (float amount) {
        // Reduce current health by the amount of damage done.
        m_CurrentPulse -= amount;
        SetPulseUI(m_CurrentPulse);

    }

    public float GetPulse () {
        return m_CurrentPulse;
    }

    public void SetPulse (float amount) {
        m_CurrentPulse = amount;
    }


    private void SetPulseUI (float newPulse) {
        StartCoroutine(FillPulseUI(newPulse));
    }

    IEnumerator FillPulseUI(float newPulse) {
        float actualFillAmount = pulseBar.fillAmount;
        float mappedNewPulse = GladiatorHealth.mapValueTo01(newPulse, 0, m_MaxPulse);
        pulseText.text = string.Format("{0}", newPulse);
        for (float t = 0.0f; t < pulseRegenerationTime; t += Time.deltaTime) {
            pulseBar.fillAmount = Mathf.Lerp(actualFillAmount, mappedNewPulse, t);
            yield return null;
        }

        yield return null;
    }

    void OnCurrentPulseChanged (float value) {
        m_CurrentPulse = value;
        // Change the UI elements appropriately.
        //SetHealthUI();

    }

    private void OnZeroHealth () {
        // Set the flag so that this function is only called once.
        m_ZeroHealthHappened = true;

        RpcOnZeroHealth();
    }

    private void InternalOnZeroHealth () {
        // Disable the collider and all the appropriate child gameobjects so the tank doesn't interact or show up when it's dead.
        SetPlayerActive(false);
    }

    [ClientRpc]
    private void RpcOnZeroHealth () {
        // Play the particle system of the tank exploding.
        //m_ExplosionParticles.Play();

        // Create a gameobject that will play the tank explosion sound effect and then destroy itself.
        //AudioSource.PlayClipAtPoint(m_TankExplosion, transform.position);

        InternalOnZeroHealth();
    }

    private void SetPlayerActive (bool active) {
        // m_Collider.enabled = active;

        m_PlayerRenderers.SetActive(active);
        //m_HealthCanvas.SetActive(active);
        //m_AimCanvas.SetActive(active);
        //m_LeftDustTrail.SetActive(active);
        //m_RightDustTrail.SetActive(active);

        if (active) m_Manager.EnableControl();
        else m_Manager.DisableControl();

        //m_Setup.ActivateCrown(active);
    }

    // This function is called at the start of each round to make sure each tank is set up correctly.
    public void SetDefaults () {
        m_CurrentPulse = m_StartingPulse;
        m_ZeroHealthHappened = false;
        SetPlayerActive(true);
    }
}
