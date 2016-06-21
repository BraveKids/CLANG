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
    
    GameObject damageCollider;



    // Use this for initialization
    void Start() {
        centerPosition = Vector3.zero;
        velocity = Vector3.zero;
        boxCollider = GetComponent<Collider>();
        swarm = new List<GameObject>();
        damageCollider = transform.GetChild(0).gameObject;


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
        damageCollider.transform.position = centerPosition;
        Debug.DrawLine(centerPosition, new Vector3(centerPosition.x, centerPosition.y + 5f, centerPosition.z), Color.red, 0, false);
    }

    public List<GameObject> GetSwarm() {
        return swarm;
    }


}
