using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;
public class GladiatorMovement : NetworkBehaviour
{
    public int m_PlayerNumber = 1;                // Used to identify which tank belongs to which player.  This is set by this tank's manager.
    public int m_LocalID = 1;
    public float m_Speed = 12f;                   // How fast the tank moves forward and back.
    public float m_TurnSpeed = 180f;              // How fast the tank turns in degrees per second.
    public float m_PitchRange = 0.2f;             // The amount by which the pitch of the engine noises can vary.
       // The particle system of dust that is kicked up from the rightt track.
    public Rigidbody m_Rigidbody;              // Reference used to move the tank.

    private string m_MovementAxis;              // The name of the input axis for moving forward and back.
    private string m_TurnAxis;                  // The name of the input axis for turning.
    private float m_MovementInput;              // The current value of the movement input.
    private float m_TurnInput;                  // The current value of the turn input.
    private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.
    private Animator anim;

    //AGGIUNTE
    //MOVIMENTO
    public float runSpeed = 0.1f;
    public float turnSmoothing = 1f;
    public float speedDampTime = 0.1f;
    private float speed;
    //public float rotationSpeed;
    VirtualJoystick joystickScript;
    private Vector3 lastDirection;
   
    private int speedFloat;
    private int hFloat;
    private int vFloat;
    private int groundedBool;
    private Transform cameraTransform;
    GameObject model;
    private float h;
    private float v;
    public Camera gladiatorCamera;
    private bool isMoving;
    private bool fly = false;
    private float distToGround;
    public bool attacking = false;
    //VARIABILI UI
    Transform buttons;
    //AGGIUNTE END
    public bool isAttacking;
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    //AGGIUNTE TOMMASO
    public override void OnStartLocalPlayer()
    {
        //GameElements.getGladiatorCanvas().transform.FindChild("VirtualJoypad").gameObject.SetActive(true);
        //camera = gameObject.transform.FindChild("Camera").GetComponent<Camera>();
        //cameraTransform = camera.gameObject.transform;
        GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("GladiatorCanvas").gameObject.SetActive(true);
        buttons = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("GladiatorCanvas/VirtualJoypad/Buttons");
        buttons.gameObject.SetActive(true);
        //camera = transform.FindChild("Camera").gameObject.GetComponent<Camera>();
        cameraTransform = gladiatorCamera.transform;
        hFloat = Animator.StringToHash("H");
        vFloat = Animator.StringToHash("V");
        speed = runSpeed;
        speedFloat = Animator.StringToHash("Speed");
        groundedBool = Animator.StringToHash("Grounded");
        distToGround = GetComponent<Collider>().bounds.extents.y;
        joystickScript = GameObject.FindGameObjectWithTag("VirtualJoystick").GetComponent<VirtualJoystick>(); ;
    }
    //AGGIUNTE END


    private void Start()
    {
        if (!this.isLocalPlayer)
        {
            gladiatorCamera.GetComponent<GladiatorCamera>().enabled = false;
            GameObject.Destroy(gladiatorCamera.gameObject);
        }
        m_Rigidbody.freezeRotation = true;
        anim = GetComponent<Animator>();
        // The axes are based on player number.
        //m_MovementAxis = "Vertical" + (m_LocalID + 1);
        //m_TurnAxis = "Horizontal" + (m_LocalID + 1);
       
        // Store the original pitch of the audio source.

    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        //AGGIUNTE
        if (!isAttacking)
        {
            h = joystickScript.Horizontal();
            //CrossPlatformInputManager.GetAxis("Horizontal");
            v = joystickScript.Vertical();
            //CrossPlatformInputManager.GetAxis("Vertical");
            isMoving = Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1;
            //AGGIUNTE END
            // Store the value of both input axes.
            //m_MovementInput = Input.GetAxis(m_MovementAxis);
            //m_TurnInput = Input.GetAxis(m_TurnAxis);
        }
     
    }


   


    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        // Adjust the rigidbodies position and orientation in FixedUpdate.
        //Move();
        //Turn();

        //AGGIUNTE
        anim.SetFloat(hFloat, h);
        anim.SetFloat(vFloat, v);
        anim.SetBool(groundedBool, IsGrounded());
        MovementManagement(h, v);
        //AGGIUNTE END
    }
    //AGGIUNTE
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }


    void MovementManagement(float horizontal, float vertical)
    {

        if (!attacking)
        {
            Rotating(horizontal, vertical);
            if (isMoving)
            {

                speed = runSpeed;
                anim.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
            }
            else
            {
                speed = 0f;
                anim.SetFloat(speedFloat, 0f);
            }
            transform.Translate(Vector3.forward * speed);
            //GetComponent<Rigidbody>().AddForce(Vector3.forward*speed);
        }
    }

    Vector3 Rotating(float horizontal, float vertical)
    {
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        if (!fly)
            forward.y = 0.0f;
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        Vector3 targetDirection;

        float finalTurnSmoothing;



        targetDirection = forward * vertical + right * horizontal;
        finalTurnSmoothing = 20.0f;


        if ((isMoving && targetDirection != Vector3.zero))
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            Quaternion newRotation = Quaternion.Slerp(GetComponent<Rigidbody>().rotation, targetRotation, finalTurnSmoothing * Time.deltaTime);
            GetComponent<Rigidbody>().MoveRotation(newRotation);
            lastDirection = targetDirection;
        }
        //idle - fly or grounded
        if (!(Mathf.Abs(h) > 0.9 || Mathf.Abs(v) > 0.9))
        {
            Repositioning();
        }

        return targetDirection;
    }

    private void Repositioning()
    {
        Vector3 repositioning = lastDirection;
        if (repositioning != Vector3.zero)
        {
            repositioning.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(repositioning, Vector3.up);
            Quaternion newRotation = Quaternion.Slerp(GetComponent<Rigidbody>().rotation, targetRotation, turnSmoothing * Time.deltaTime);
            GetComponent<Rigidbody>().MoveRotation(newRotation);
        }
    }
    //AGGIUNTE END


    private void Move()
    {
        // Create a movement vector based on the input, speed and the time between frames, in the direction the tank is facing.
        anim.SetFloat("Speed", m_MovementInput);
        Vector3 movement = transform.forward * m_MovementInput * m_Speed * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = m_TurnInput * m_TurnSpeed * Time.deltaTime;

        // Make this into a rotation in the y axis.
        Quaternion inputRotation = Quaternion.Euler(0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * inputRotation);
    }


    // This function is called at the start of each round to make sure each tank is set up correctly.
    public void SetDefaults()
    {
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;

        m_MovementInput = 0f;
        m_TurnInput = 0f;

     
    }

    //We freeze the rigibody when the control is disabled to avoid the tank drifting!
    protected RigidbodyConstraints m_OriginalConstrains;
    void OnDisable()
    {
        m_OriginalConstrains = m_Rigidbody.constraints;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    void OnEnable()
    {
        m_Rigidbody.constraints = m_OriginalConstrains;
    }
}