using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwarmAI : MonoBehaviour {

    public GameObject beePrefab;
    public int swarmSize;
    private List<GameObject> swarm;
    private Collider boxCollider;
    public float alignmentWeight;
    public float cohesionWeight;

    public float separationWeight;
    public GameObject target;
    public Vector3 centerPosition;
    public Vector3 velocity;
    public float boxDimension;
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
            float totWeight = alignmentWeight + cohesionWeight + separationWeight;
            beeAI.SetAlignment(alignmentWeight / totWeight);
            beeAI.SetCohesion(cohesionWeight / totWeight);
            beeAI.SetSeparation(separationWeight / totWeight);
        }
    }

    // Update is called once per frame
    void Update() {
        centerPosition = Vector3.zero;
        velocity = Vector3.zero;
        foreach (GameObject agent in swarm) {
            centerPosition += agent.transform.position;
            velocity += agent.GetComponent<Rigidbody>().velocity;
            BeeAI beeAI = agent.GetComponent<BeeAI>();
            beeAI.SetAlignment(alignmentWeight);
            beeAI.SetCohesion(cohesionWeight);
            beeAI.SetSeparation(separationWeight);
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
