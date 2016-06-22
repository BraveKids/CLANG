using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

public class StrategistSpawner : NetworkBehaviour {

    public Camera strategistCamera;
    public int m_PlayerNumber = 1;            // Used to identify the different players.
    //public Rigidbody m_Shell;                 // Prefab of the shell.
    //public Transform m_FireTransform;         // A child of the tank where the shells are spawned.
    //public Slider m_AimSlider;                // A child of the tank that displays the current launch force.
    //public AudioSource m_ShootingAudio;       // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    //public AudioClip m_ChargingClip;          // Audio that plays when each shot is charging up.
    //public AudioClip m_FireClip;              // Audio that plays when each shot is fired.
    //public float m_MinLaunchForce = 15f;      // The force given to the shell if the fire button is not held.
    //public float m_MaxLaunchForce = 30f;      // The force given to the shell if the fire button is held for the max charge time.
    //public float m_MaxChargeTime = 0.75f;     // How long the shell can charge for before it is fired at max force.

    [SyncVar]
    public int m_localID;
    
    //private string m_FireButton;            // The input axis that is used for launching shells.
    
    public Rigidbody m_Rigidbody;          // Reference to the rigidbody component.
    /*
    [SyncVar]
    private float m_CurrentLaunchForce;     // The force that will be given to the shell when the fire button is released.
    //[SyncVar]
    //private float m_ChargeSpeed;            // How fast the launch force increases, based on the max charge time.
    */
    private bool m_Fired;                   // Whether or not the shell has been launched with this button press.

    public GameObject strategistCanvas;


    private void Awake()
    {
        // Set up the references.
        m_Rigidbody = GetComponent<Rigidbody>();
        
    }
    public override void OnStartLocalPlayer()
    {
        GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("StrategistCanvas").gameObject.SetActive(true);
        //GetComponent<GameTimer>().enabled = true;
        GetComponent<StrategistPulse>().enabled = true;
    }


    void Start () {
        if (!this.isLocalPlayer)
        {
            strategistCamera.GetComponent<StrategistCamera>().enabled = false;
            GameObject.Destroy(strategistCamera.gameObject);
        }

    }

    public void Spawn(GameObject spawnObj, Vector3 position)
    {
        
        // Set the fired flag so only Fire is only called once.
        m_Fired = true;
        CmdSpawn(spawnObj, position);
        
    }

    [Command]
    private void CmdSpawn(GameObject spawnObj, Vector3 position)
    {
        GameObject instance = Instantiate(spawnObj, position, Quaternion.identity) as GameObject;
        if(instance.tag == "Droppable")
        {
            IncreaseItems(instance);
        }
        NetworkServer.Spawn(instance);
        if (isLocalPlayer)
        {
            RpcIncreaseEnemy();
        }
    }
    
   
    void IncreaseItems(GameObject obj)
    {

        GameElements.itemSpawned.Add(obj);
    }
    [ClientRpc]
    void RpcIncreaseEnemy()
    {
        
        GameElements.increaseEnemy();
    }



    public void SetArmorDropped()
    {
        GameElements.setArmorDropped(true);
    }

  

   

    public void SetMedDropped()
    {
        GameElements.setMedDropped(true);
    }

   

   

    [ClientCallback]
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
        {
            return;
        }
	}


    public void SetDefaults()
    {
        //m_CurrentLaunchForce = m_MinLaunchForce;
        //m_AimSlider.value = m_MinLaunchForce;
    }

    void OnDestroy()
    {
        GameManager.s_Instance.winner = "GLADIATOR";
        GameManager.s_Instance.SetGameWinner(GameElements.getGladiator());
        GameManager.s_Instance.endGame = true;
    }
}
