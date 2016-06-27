using UnityEngine;
using UnityEngine.Networking;

public class GladiatorMovement : NetworkBehaviour {
    public int m_PlayerNumber = 1;
    public int m_LocalID = 1;
    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;
    public float m_PitchRange = 0.2f;
    public Rigidbody m_Rigidbody;
    private string m_MovementAxis;
    private string m_TurnAxis;
    private float m_MovementInput;
    private float m_TurnInput;
    private float m_OriginalPitch;
    private Animator anim;
    public Transform movingTranform;
    public float runSpeed = 0.3f;
    public float turnSmoothing = 1f;
    public float speedDampTime = 0.1f;
    private float speed;
    VirtualJoystick joystickScript;
    private Vector3 lastDirection;
    public bool dash = false;
    private int speedFloat;
    private int hFloat;
    private int vFloat;
    private int groundedBool;
    private Transform cameraTransform;
    public Transform dashPoint;
    GameObject model;
    private float h;
    private float v;
    public Camera gladiatorCamera;
    private bool isMoving;

    private float distToGround;
    public bool attacking = false;
    Transform buttons;
    public bool isAttacking;
    private void Awake () {
        m_Rigidbody = GetComponent<Rigidbody>();
        gladiatorCamera = GameObject.FindGameObjectWithTag("GladiatorCamera").GetComponent<Camera>();
    }


    public override void OnStartLocalPlayer () {
        gladiatorCamera.gameObject.GetComponent<GladiatorCamera>().enabled = true;
        cameraTransform = gladiatorCamera.transform;
        GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("GladiatorCanvas").gameObject.SetActive(true);
        GetComponent<GameTimer>().enabled = true;
        buttons = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("GladiatorCanvas/VirtualJoypad/Buttons");
        buttons.gameObject.SetActive(true);
        hFloat = Animator.StringToHash("H");
        vFloat = Animator.StringToHash("V");
        speed = runSpeed;
        speedFloat = Animator.StringToHash("Speed");
        groundedBool = Animator.StringToHash("Grounded");
        distToGround = GetComponent<Collider>().bounds.extents.y;
        joystickScript = GameObject.FindGameObjectWithTag("VirtualJoystick").GetComponent<VirtualJoystick>(); ;
    }


    private void Start () {


        if (!this.isLocalPlayer) {

            gladiatorCamera.GetComponent<GladiatorCamera>().enabled = false;
            GameObject.Destroy(gladiatorCamera.gameObject);
        }
        m_Rigidbody.freezeRotation = true;
        anim = GetComponent<Animator>();
        


    }

    private void Update () {
        if (!isLocalPlayer) {
            return;
        }

        if (dash) {

            transform.position = Vector3.MoveTowards(transform.position, dashPoint.position, 5f * Time.deltaTime);
        }


        h = joystickScript.Horizontal();

        v = joystickScript.Vertical();

        isMoving = Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1;


        anim.SetFloat(hFloat, h);
        anim.SetFloat(vFloat, v);
        anim.SetBool(groundedBool, IsGrounded());
        MovementManagement(h, v);


    }
    public void Dodge () {
        if (!isAttacking) {
            anim.SetBool("Dash", true);
            m_Rigidbody.velocity = Vector3.zero;
            dash = true;

            Invoke("DashDown", 0.4f);
        }
    }

    void DashDown () {
        dash = false;
        anim.SetBool("Dash", false);
    }


    private void FixedUpdate () {
        if (!isLocalPlayer) {
            return;
        }





    }

    bool IsGrounded () {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }


    void MovementManagement (float horizontal, float vertical) {

        if (!isAttacking && !dash) {
            Rotating(horizontal, vertical);
            if (isMoving) {

                speed = runSpeed;
                anim.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
            } else {
                speed = 0f;
                anim.SetFloat(speedFloat, 0f);
            }

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(movingTranform.position.x, transform.position.y, movingTranform.position.z), speed * Time.deltaTime);

        }
    }

    Vector3 Rotating (float horizontal, float vertical) {
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        Vector3 targetDirection;

        float finalTurnSmoothing;



        targetDirection = forward * vertical + right * horizontal;
        finalTurnSmoothing = 20.0f;


        if ((isMoving && targetDirection != Vector3.zero)) {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            Quaternion newRotation = Quaternion.Lerp(transform.rotation, targetRotation, finalTurnSmoothing * Time.deltaTime);

            gameObject.transform.rotation = newRotation;
            lastDirection = targetDirection;
        }


        return targetDirection;
    }

    private void Repositioning () {
        Vector3 repositioning = lastDirection;
        if (repositioning != Vector3.zero) {
            repositioning.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(repositioning, Vector3.up);
            Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSmoothing * Time.deltaTime);
            transform.rotation = newRotation;
        }
    }




    public void SetDefaults () {


        m_MovementInput = 0f;
        m_TurnInput = 0f;


    }


    protected RigidbodyConstraints m_OriginalConstrains;
    void OnDisable () {
        m_OriginalConstrains = m_Rigidbody.constraints;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    void OnEnable () {
        m_Rigidbody.constraints = m_OriginalConstrains;
    }

    public void setAttacking (bool attacking) {
        this.isAttacking = attacking;
    }
}