using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwarmAI : MonoBehaviour {

    public GameObject beePrefab;
    public float boxDimension = 2;
    public int swarmSize = 5;

    public float maxVel = 3f;
    private List<GameObject> swarm;
    private Collider boxCollider;
    public float alignmentWeight = 1.5f;
    public float cohesionWeight = 2.5f;
    public float separationWeight = 2;
    public float followWeight = 5f;
    public bool follow = true;
    public GameObject target;
    public Vector3 centerPosition;
    public Vector3 velocity;
    public float swarmDamage;
    public float speed;
    
    SphereCollider damageCollider;

    public bool collided;
    public bool debugMode;

    EnemyHealth health;
    FSM SwarmFSM;
    AILerp agent;




    // Use this for initialization
    void Start() {
        agent = GetComponent<AILerp>();
        //agent.target = target.transform;
        //agent.canMove = true;
        //agent.speed = speed;

        collided = false;

        FSMState chasing = new FSMState();
        FSMState attacking = new FSMState();
        FSMState dying = new FSMState();

        chasing.AddTransition(new FSMTransition(Colliding, attacking));
        chasing.AddTransition(new FSMTransition(GoigToDie, dying));


        attacking.AddTransition(new FSMTransition(NotColliding, chasing));
        attacking.AddTransition(new FSMTransition(GoigToDie, dying));
        attacking.AddStayAction(Attack);

        SwarmFSM = new FSM(chasing);

        dying.AddStayAction(Die);


        centerPosition = Vector3.zero;
        velocity = Vector3.zero;
        boxCollider = GetComponent<Collider>();
        swarm = new List<GameObject>();
        //damageCollider = transform.GetChild(0).gameObject;
        damageCollider = GetComponent<SphereCollider>();


        for (var i = 0; i < swarmSize; i++) {
            float localX = (Random.Range(-1, 1)) * boxDimension / 2;   //generate random from -1 to 1, then multiply per boxdimension/2
            float localY = (Random.Range(-1, 1)) * boxDimension / 2;
            float localZ = (Random.Range(-1, 1)) * boxDimension / 2;

            Vector3 position = new Vector3(
                transform.position.x + localX, transform.position.y + localY, transform.position.z + localZ
            );

            GameObject boid = Instantiate(beePrefab, position, transform.rotation) as GameObject;

            boid.transform.SetParent(transform);
            //boid.transform.position= position;

            swarm.Add(boid);

            BeeAI beeAI = boid.GetComponent<BeeAI>();
           
        }
    }

    // Update is called once per frame
    void Update() {
        centerPosition = Vector3.zero;
        velocity = Vector3.zero;
        foreach (GameObject agent in swarm) {
            centerPosition += agent.transform.position;
            velocity += agent.transform.forward;
            BeeAI beeAI = agent.GetComponent<BeeAI>();
  
        }
        centerPosition /= swarmSize;
        velocity /= swarmSize;
        //position.Normalize();
        centerPosition.y = transform.position.y;
        //damageCollider.transform.position = centerPosition;
        Vector3 colliderPos = transform.InverseTransformPoint(centerPosition);
        damageCollider.center = colliderPos;
        centerPosition = transform.position;
  
        Debug.DrawLine(centerPosition, new Vector3(centerPosition.x, centerPosition.y + 5f, centerPosition.z), Color.red, 0, false);

        /*transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.transform.position.x,
                                                                                transform.position.y,
                                                                                target.transform.position.z)
                                                                                , speed * Time.deltaTime);*/
        //SwarmFSM.Update();
    }

    public List<GameObject> GetSwarm() {
        return swarm;
    }

    bool Colliding() {
        return collided;
    }

    bool NotColliding() {
        return !collided;
    }

    bool GoigToDie() {
        return health.currentHealth <= 0;
    }

    void Die() {
        DebugLine("Dying");
    }
    void Attack() {
        DebugLine("Attack");
        //TODO infliggi il danno al gladiatore
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Gladiator")
            collided = true;
    }
    
    void OnTriggerExit(Collider col) {
        if (col.gameObject.tag == "Gladiator") {
            collided = false;
        }
    }

    void DebugLine(string text) {
        if (debugMode)
            Debug.Log(text);
    }




}
