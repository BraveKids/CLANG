using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwarmAI : MonoBehaviour {

    public GameObject beePrefab;

    public float boxDimension = 2;
    public int swarmSize = 5;

    public float maxVel = 3f;   //max speed of the bees
    private List<GameObject> swarm;     //contains all the bees

    public float alignmentWeight = 1.5f;
    public float cohesionWeight = 2.5f;
    public float separationWeight = 2;
    public float followWeight = 5f;
    public bool follow = true;  //follow the target

    public GameObject target;       //the target to follow. This is going in input to the AILerp
    public GameObject beeTarget;
    public Vector3 centerPosition;  //center position of the swarm
    public Vector3 velocity;

    public float swarmDamage;

    SphereCollider damageCollider;

    public bool isColliding;
    public bool die;
    public bool debugMode;
    public bool AStar = true;

    EnemyHealth health;
    FSM SwarmFSM;
    AILerp agent;
    float originY;





    // Use this for initialization
    void Start() {
        agent = GetComponent<AILerp>();
        agent.speed = maxVel;
        agent.target = target.transform;
        beeTarget = gameObject;
        originY = target.transform.position.y;
        isColliding = false;

        //FSM 

        FSMState chasing = new FSMState();
        FSMState attacking = new FSMState();
        FSMState dying = new FSMState();

        chasing.AddTransition(new FSMTransition(Colliding, attacking));
        chasing.AddTransition(new FSMTransition(GoigToDie, dying));

        attacking.AddStayAction(Attack);

        attacking.AddTransition(new FSMTransition(NotColliding, chasing));
        attacking.AddTransition(new FSMTransition(GoigToDie, dying));

        dying.AddStayAction(Die);

        SwarmFSM = new FSM(chasing);



        centerPosition = Vector3.zero;
        velocity = Vector3.zero;
        swarm = new List<GameObject>();
        damageCollider = GetComponent<SphereCollider>();

        //Spawn all the bees
        for (var i = 0; i < swarmSize; i++) {
            float localX = (Random.Range(-1, 1)) * boxDimension / 2;   //generate random from -1 to 1, then multiply per boxdimension/2
            float localY = (Random.Range(-1, 1)) * boxDimension / 2;
            float localZ = (Random.Range(-1, 1)) * boxDimension / 2;

            Vector3 position = new Vector3(
                transform.position.x + localX, transform.position.y + localY, transform.position.z + localZ
            );

            GameObject boid = Instantiate(beePrefab, position, transform.rotation) as GameObject;

            swarm.Add(boid);
            boid.GetComponent<BeeAI>().fakeParent = gameObject;     //add the swarm object ad fakeParent to the bees
            BeeAI beeAI = boid.GetComponent<BeeAI>();

        }
    }



    void Update() {

        if (AStar) {
            if ( beeTarget == target) {
                beeTarget = gameObject; 
                agent.canMove = true;
            }
        }
        else {
            if (beeTarget == gameObject) {
                beeTarget = target;
                agent.canMove = false;
            }
        }

        centerPosition = Vector3.zero;
        velocity = Vector3.zero;


        //Compute swarm center
        foreach (GameObject agent in swarm) {
            centerPosition += agent.transform.position;
            velocity += agent.transform.forward;
            BeeAI beeAI = agent.GetComponent<BeeAI>();

        }
        centerPosition /= swarmSize;
        velocity /= swarmSize;

        //we need the center to stay at certain height
        centerPosition.y = target.transform.position.y;

        //Collider match with center position 
        Vector3 colliderPos = transform.InverseTransformPoint(centerPosition);
        damageCollider.center = colliderPos;

        Debug.DrawLine(centerPosition, new Vector3(centerPosition.x, centerPosition.y + 5f, centerPosition.z), Color.red, 0, false);
        Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Color.cyan, 0, false);

        SwarmFSM.Update();
    }

    void LateUpdate() {
        transform.position = new Vector3(transform.position.x, originY, transform.position.z);

    }
    public List<GameObject> GetSwarm() {
        return swarm;
    }

    bool Colliding() {
        return isColliding;
    }

    bool NotColliding() {
        return !isColliding;
    }

    bool GoigToDie() {
        //return health.currentHealth <= 0;
        return die;
    }

    void Die() {
        DebugLine("Dying");
        foreach (GameObject agent in swarm) {
            Destroy(agent);
        }
        Destroy(gameObject);
    }
    void Attack() {
        DebugLine("Attack" + Random.value);
    }

    void OnTriggerEnter(Collider col) {
        Debug.Log("Collide");
        if (col.gameObject.tag == "Gladiator")
            isColliding = true;
    }

    void OnTriggerExit(Collider col) {
        if (col.gameObject.tag == "Gladiator") {
            isColliding = false;
        }
    }

    void DebugLine(string text) {
        if (debugMode)
            Debug.Log(text);
    }

    /*void OnDestoy() {
        target.GetComponent<GladiatorShooting>().RemoveTarget(transform);
    }*/

    /*void Destroy() {
        target.GetComponent<GladiatorShooting>().DestroyEnemy(gameObject);
    }*/





}
