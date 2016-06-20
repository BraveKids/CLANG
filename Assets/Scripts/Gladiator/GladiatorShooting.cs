using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using System;

public class GladiatorShooting : NetworkBehaviour
{
    public int m_PlayerNumber = 1;            // Used to identify the different players.
    public Rigidbody m_Shell;                 // Prefab of the shell.
    public Transform m_FireTransform;         // A child of the tank where the shells are spawned.
    public Rigidbody Grenade;
    public Transform grenadeTransform;
    //public Slider m_AimSlider;                // A child of the tank that displays the current launch force.
    //public AudioSource m_ShootingAudio;       // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    //public AudioClip m_ChargingClip;          // Audio that plays when each shot is charging up.
    //public AudioClip m_FireClip;              // Audio that plays when each shot is fired.
    public float m_MinLaunchForce = 15f;      // The force given to the shell if the fire button is not held.
    public GameObject grenadeModel;
    public bool grenadeTaken;
    bool grenadeLaunching;

    [SyncVar]
    public int m_localID;
    Animator m_animator;
    
    [SyncVar]
    private float m_CurrentLaunchForce;     // The force that will be given to the shell when the fire button is released.
    //[SyncVar]
    //private float m_ChargeSpeed;            // How fast the launch force increases, based on the max charge time.
   

    public bool basicAttack;
    public bool specialAttack;
    public Transform attackTransform;
    public Transform handPosition;
    public Transform elbowPosition;
    public GameObject handWeapon;
    public GameObject fireWeapon;
    public List<Transform> targets;
    public GameObject meleePrefab;
    GladiatorHealth healthScript;
    GladiatorMovement movementScript;
    public bool damaged;


    private void Awake()
    {

        // Set up the references.
        //m_Rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        healthScript = GetComponent<GladiatorHealth>();
        movementScript = GetComponent<GladiatorMovement>();
        fireWeapon = null;
    }


    private void Start()
    {
        grenadeTaken = false;
        grenadeLaunching = false;
        targets = new List<Transform>();
        basicAttack = false;
        specialAttack = false;
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
            if (fireWeapon.GetComponent<FireWeapon>().integrity <= 0)
            {
                ThrowWeapon();
            }
        }

        if(grenadeTaken == true)
        {
            grenadeModel.SetActive(true);
        }
        

    }
    
    public void AddTarget(Transform target)
    {
        if (!targets.Contains(target))
        {
            targets.Add(target);
        }
    }


    public void RemoveTarget(Transform obj)
    {
            targets.Remove(obj);
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
        if (command.Equals("dodge"))
        {
            movementScript.Dodge();
        }
        if (command.Equals("grenade"))
        {
            GrenadeAttack();
        }
    }

    public void PickUpObject(GameObject obj, string id)
    {
        CmdRemoveItem(obj);
        if (id.Equals("fireweapon"))
        {
            if (fireWeapon != null)
            {
                ThrowWeapon();
            }
            fireWeapon = obj;
            if (handWeapon != null)
            {
                ToggleWeapon("fire");
                
                m_Shell = fireWeapon.GetComponent<FireWeapon>().bulletPrefab.GetComponent<Rigidbody>();
            }
        }
        else if (id.Equals("medpack"))
        {
            healthScript.Recover(10.6f);
        }
        else if (id.Equals("armor"))
        {
            healthScript.SetArmor(16f);
        }
        else if (id.Equals("grenade"))
        {
            grenadeTaken = true;
        }
    }

    [Command]
    void CmdRemoveItem(GameObject obj)
    {
        // this code is only executed on the server
        RpcRemoveItem(obj); // invoke Rpc on all clients
    }

    [ClientRpc]
    void RpcRemoveItem(GameObject obj)
    {
        // this code is executed on all clients
        GameElements.itemSpawned.Remove(obj);
    }

    private void BasicAttack()
    {
        if (!basicAttack && !damaged && !specialAttack && !grenadeLaunching)
        {
            basicAttack = true;
            movementScript.setAttacking(true);
            if (targets.Count > 0)
            {
                Transform target = FindNearestTarget();

                if (target.tag == "WurmCore")
                {
                    transform.LookAt(new Vector3(target.position.x + 4.5f, transform.position.y, target.position.z));
                }
                else
                {
                    transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                }
            }
            if (fireWeapon != null)
            {
                ToggleWeapon("hand");
            }
            CmdSetAnimTrigger("Attack");
            CmdAttack();
            Invoke("BasicAttackDown", 0.6f);
        }
}
    [Command]
    public void CmdSetAnimTrigger(string triggerName)
    {
        if (!isServer)
        {
            m_animator.SetTrigger(triggerName);
        }
        RpcSetAnimTrigger(triggerName);
    }

    [ClientRpc]
    public void RpcSetAnimTrigger(string triggerName)
    {
        m_animator.SetTrigger(triggerName);
    }



    private void BasicAttackDown()
    {
     
        if (fireWeapon != null)
        {
            ToggleWeapon("fire");
        }
        Invoke("CanAttack", 0.5f);
        movementScript.setAttacking(false);
    }

    private void CanAttack()
    {
        basicAttack = false;
    }

    private void SpecialAttack()
    {
        if (fireWeapon != null)
        {

            if (!specialAttack && !damaged && !basicAttack && !grenadeLaunching)
            {
                movementScript.setAttacking(true);
                if (targets.Count > 0)
                {
                    Transform target = FindNearestTarget();
                   

                        if (target.tag == "WurmCore")
                        {
                            transform.LookAt(new Vector3(target.position.x + 4.5f, transform.position.y, target.position.z));
                        }
                        else
                        {
                            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                        }
                    }
                    //m_Rigidbody.velocity = Vector3.zero;
                    //m_Rigidbody.isKinematic = true;
                    specialAttack = true;

                    m_animator.SetBool("Shoot", true);
                    Invoke("Fire", 0.3f);
                }
            }
        



    }

    void GrenadeAttack()
    {
        if (grenadeTaken == true)
        {
            if (!specialAttack && !damaged && !basicAttack && !grenadeLaunching)
        {
                grenadeLaunching = true;
                movementScript.setAttacking(true);
                if (targets.Count > 0)
                {
                    Transform target = FindNearestTarget();


                    if (target.tag == "WurmCore")
                    {
                        transform.LookAt(new Vector3(target.position.x + 4.5f, transform.position.y, target.position.z));
                    }
                    else
                    {
                        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                    }
                }
                m_animator.SetBool("Grenade", true);
                Invoke("LaunchGrenade", 0.8f);
            }
        }
    }

    void LaunchGrenade()
    {
        ThrowGranade();
        CmdGrenade();
        Invoke("LaunchBack", 0.1f);
    }

    void LaunchBack()
    {
        grenadeLaunching = false;
        m_animator.SetBool("Grenade", false);
        //granadeModel.SetActive(true); 
        movementScript.setAttacking(false);
        grenadeTaken = false;
    }

    public Transform FindNearestTarget()
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform t in targets)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

    

    private void Fire()
    {
        if (fireWeapon != null)
        {
            CmdFire();
            Invoke("FireDown", 0.7f);
        }

    }

    private void ToggleWeapon(string weapon)
    {
        if (weapon == "hand")
        {
         
                fireWeapon.transform.FindChild("Model").gameObject.SetActive(false);
            
                handWeapon.transform.FindChild("Model").gameObject.SetActive(true);
            CmdToggleWeapon("hand");
        }
        else if (weapon == "fire")
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
        
        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
             Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Create a velocity that is the tank's velocity and the launch force in the fire position's forward direction.
        Vector3 velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        // Set the shell's velocity to this velocity.
        shellInstance.velocity = velocity;

        NetworkServer.Spawn(shellInstance.gameObject);
        Destroy(shellInstance.gameObject, 2.0f);



    }


    [Command]
    private void CmdGrenade()
    {

        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
             Instantiate(Grenade, grenadeTransform.position, grenadeTransform.rotation) as Rigidbody;
        
       

        // Create a velocity that is the tank's velocity and the launch force in the fire position's forward direction.
        Vector3 velocity = 6f * (grenadeTransform.forward + grenadeTransform.up);

        // Set the shell's velocity to this velocity.
        shellInstance.velocity = velocity;

        NetworkServer.Spawn(shellInstance.gameObject);
        //Destroy(shellInstance.gameObject, 2.0f);



    }

    [Command]
    private void CmdAttack()
    {
        
        // Create an instance of the shell and store a reference to it's rigidbody.
        GameObject triggerInstance =
             Instantiate(meleePrefab, attackTransform.position, attackTransform.rotation) as GameObject;

        

        NetworkServer.Spawn(triggerInstance.gameObject);
        Destroy(triggerInstance.gameObject, 0.2f);



    }


  

    private void FireDown()
    {
        if (fireWeapon != null)
        {

            specialAttack = false;
            m_animator.SetBool("Shoot", false);
            fireWeapon.GetComponent<FireWeapon>().integrity -= 1;
            movementScript.setAttacking(false);
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
    private void ThrowGranade()
    {
        grenadeModel.SetActive(false);
        grenadeTaken = false;
        
        CmdThrowGrenade();
    }

    [Command]
    private void CmdThrowGrenade()
    {

        grenadeModel.SetActive(false);
        grenadeTaken = false;

    }
    public void DestroyEnemy(GameObject obj)
    {
        //RemoveTarget(obj);
        CmdDestroyEnemy(obj);
    }

    [Command]
    private void CmdDestroyEnemy(GameObject obj)
    {
        Destroy(obj);
        //RemoveTarget(obj);
        RpcDecreaseEnemy();

    }

    [ClientRpc]
    private void RpcDecreaseEnemy()
    {
        if (!isLocalPlayer)
        {

            GameElements.decreaseEnemy();
        }

    }

    // This is used by the game manager to reset the tank.
    public void SetDefaults()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        //m_AimSlider.value = m_MinLaunchForce;
    }
}
