using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class GladiatorShooting : NetworkBehaviour
{
    public int m_PlayerNumber = 1;            // Used to identify the different players.
    public Rigidbody m_Shell;                 // Prefab of the shell.
    public Transform m_FireTransform;         // A child of the tank where the shells are spawned.
    //public Slider m_AimSlider;                // A child of the tank that displays the current launch force.
    //public AudioSource m_ShootingAudio;       // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    //public AudioClip m_ChargingClip;          // Audio that plays when each shot is charging up.
    //public AudioClip m_FireClip;              // Audio that plays when each shot is fired.
    public float m_MinLaunchForce = 15f;      // The force given to the shell if the fire button is not held.
    //public float m_MaxLaunchForce = 30f;      // The force given to the shell if the fire button is held for the max charge time.
    //public float m_MaxChargeTime = 0.75f;     // How long the shell can charge for before it is fired at max force.

    [SyncVar]
    public int m_localID;
    Animator m_animator;
    private string m_FireButton;            // The input axis that is used for launching shells.
    //private Rigidbody m_Rigidbody;          // Reference to the rigidbody component.
    [SyncVar]
    private float m_CurrentLaunchForce;     // The force that will be given to the shell when the fire button is released.
    //[SyncVar]
    //private float m_ChargeSpeed;            // How fast the launch force increases, based on the max charge time.
    private bool m_Fired;                   // Whether or not the shell has been launched with this button press.

    public bool basicAttack;
    public bool specialAttack;
    public GameObject attackTrigger;
    public Transform handPosition;
    public Transform elbowPosition;
    public GameObject handWeapon;
    public GameObject fireWeapon;
    private void Awake()
    {

        // Set up the references.
        //m_Rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        fireWeapon = null;
    }


    private void Start()
    {
        attackTrigger.SetActive(false);
        basicAttack = false;
        specialAttack = false;
        
        // The fire axis is based on the player number.
        m_FireButton = "Fire" + (m_localID + 1);

        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        //m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

    [ClientCallback]
    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (fireWeapon != null)
        {
            if(fireWeapon.GetComponent<FireWeapon>().integrity <= 0)
            {
                ThrowWeapon();
            }
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            m_animator.SetTrigger("Dash");
        }
       
    }


  

    public void CommandInterpreter(string command)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (command.Equals("fire"))
        {
            if (fireWeapon != null)
            {
             
                SpecialAttack();
                
            }
            else
            {
                Debug.Log("Nessuna Arma da Fuoco");
            }
        }
        if (command.Equals("attack"))
        {
         
                BasicAttack();
            
        }
    }

    public void PickUpObject(GameObject obj, string id)
    {
        if (id.Equals("fireweapon"))
        {
            fireWeapon = obj;
            if (handWeapon != null)
            {
                ToggleWeapon("fire");
                //m_FireTransform = fireWeapon.GetComponent<FireWeapon>().shootPosition.transform;
                m_Shell = fireWeapon.GetComponent<FireWeapon>().bulletPrefab.GetComponent<Rigidbody>();
            }
        }
    }

    private void BasicAttack()
    {
        if (!basicAttack)
        {
            basicAttack = true;
            GetComponent<GladiatorMovement>().setAttacking(true);
            if (fireWeapon != null)
            {
                ToggleWeapon("hand");
            }
            m_animator.SetTrigger("Attack");
            attackTrigger.SetActive(true);
            Invoke("BasicAttackDown", 0.6f);
        }
        
    }

    private void BasicAttackDown()
    {
        attackTrigger.SetActive(false);
        if (fireWeapon != null)
        {
            ToggleWeapon("fire");
        }
        Invoke("CanAttack", 0.5f);
        GetComponent<GladiatorMovement>().setAttacking(false);
    }

    private void CanAttack()
    {
        basicAttack = false;
    }

    private void SpecialAttack()
    {
        if (fireWeapon != null)
        {

            if (!specialAttack)
            {
                //m_Rigidbody.velocity = Vector3.zero;
                //m_Rigidbody.isKinematic = true;
                specialAttack = true;
                GetComponent<GladiatorMovement>().setAttacking(true);
                m_animator.SetBool("Shoot", true);
                Invoke("Fire", 0.5f);
            }
        }
          
     
    }

   private void Fire()
    {
        if (fireWeapon != null)
        {
            CmdFire();
            Invoke("FireDown", fireWeapon.GetComponent<FireWeapon>().rateOfAttack); 
        }
        
    }

    private void ToggleWeapon(string weapon)
    {
        if(weapon == "hand")
        {
            fireWeapon.transform.FindChild("Model").gameObject.SetActive(false);
            handWeapon.transform.FindChild("Model").gameObject.SetActive(true);
            CmdToggleWeapon("hand");
        }else if (weapon == "fire")
        {
            fireWeapon.transform.FindChild("Model").gameObject.SetActive(true);
            handWeapon.transform.FindChild("Model").gameObject.SetActive(false);
            CmdToggleWeapon("fire");
        }
    }
  

    [Command]
    private void CmdToggleWeapon(string weapon)
    {
        if (fireWeapon != null)
        {
            if (weapon.Equals("hand"))
            {
                fireWeapon.transform.FindChild("Model").gameObject.SetActive(false);
                handWeapon.transform.FindChild("Model").gameObject.SetActive(true);
            }
            else if (weapon.Equals("fire"))
            {
                fireWeapon.transform.FindChild("Model").gameObject.SetActive(true);
                handWeapon.transform.FindChild("Model").gameObject.SetActive(false);
            }
        }
    }

    [Command]
    private void CmdFire()
    {
        Debug.Log("Sparo");
        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
             Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Create a velocity that is the tank's velocity and the launch force in the fire position's forward direction.
        Vector3 velocity =  m_CurrentLaunchForce * m_FireTransform.forward;

        // Set the shell's velocity to this velocity.
        shellInstance.velocity = velocity;

        NetworkServer.Spawn(shellInstance.gameObject);
        Destroy(shellInstance.gameObject, 2.0f);
        


    }
    private void FireDown()
    {
        if (fireWeapon != null)
        {
            
            specialAttack = false;
            m_animator.SetBool("Shoot", false);
            fireWeapon.GetComponent<FireWeapon>().integrity -= 1;
            GetComponent<GladiatorMovement>().setAttacking(false);
            //m_Rigidbody.isKinematic = false;
        }
    }
    private void ThrowWeapon()
    {
        ToggleWeapon("hand");
        fireWeapon = null;
        CmdThrowWeapon();
    }
 
    [Command]
    private void CmdThrowWeapon()
    {

        Destroy(this.fireWeapon);
        
    }

    [Command]
    public void CmdDestroyEnemy(GameObject obj)
    {
        Destroy(obj);
    }

    // This is used by the game manager to reset the tank.
    public void SetDefaults()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        //m_AimSlider.value = m_MinLaunchForce;
    }
}