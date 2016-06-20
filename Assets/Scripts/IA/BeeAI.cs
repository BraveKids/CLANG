using UnityEngine;
using System.Collections;

public class BeeAI : MonoBehaviour {

    SwarmAI swarm;
    Rigidbody rigidbody;
    float alignmentWeight;
    float cohesionWeight;

    float separationWeight;

    // Use this for initialization
    void Start() {
        rigidbody = GetComponent<Rigidbody>();
        swarm = GetComponentInParent<SwarmAI>();
        //transform.localPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        Vector3 alignment = Alignment() * alignmentWeight;
        Vector3 cohesion = Cohesion() * cohesionWeight;
        Vector3 separation = Separation() * separationWeight;
        Vector3 chasee = swarm.target.transform.position - transform.position;
        Vector3 velocity= alignment * alignmentWeight + cohesion * cohesionWeight + separation * separationWeight+chasee;
        rigidbody.velocity = velocity;
        rigidbody.velocity.Normalize();
        Quaternion rotation = Quaternion.LookRotation(velocity);

        rigidbody.MoveRotation(rotation);
        //rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation,rotation,5f*Time.deltaTime));
        Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z), Color.green, 0, false);
    }

    Vector3 Alignment() {
        Vector3 alignment = Vector3.zero;
        foreach(GameObject agent in swarm.GetSwarm()) {
            alignment += agent.transform.GetComponent<Rigidbody>().velocity;
        }
        alignment.Normalize();
        /*Vector3 alignment = swarm.velocity;
        alignment -= rigidbody.velocity;
        alignment /= swarm.swarmSize;
        alignment.Normalize();*/
        return alignment;
    }

    Vector3 Cohesion() {
        Vector3 cohesion;
        /*foreach (GameObject boid in swarm.GetSwarm()) {
            if (boid != gameObject) {
                cohesion += boid.transform.position;
            }
        }*/
        cohesion = new Vector3(swarm.centerPosition.x, swarm.centerPosition.y, swarm.centerPosition.z);

        cohesion -= transform.position;
        cohesion /= swarm.swarmSize;
        cohesion -= transform.position;
        cohesion.Normalize();
        return cohesion;
    }

    Vector3 Separation() {
        Vector3 separation = new Vector3();
        foreach (GameObject boid in swarm.GetSwarm()) {
            if (boid != gameObject) {
                separation += boid.transform.position - transform.position;
            }
        }

        separation /= swarm.swarmSize;
        
        separation *= -1;
        separation.Normalize();

        return separation;
    }

    public void SetAlignment(float alignmentW) {
        alignmentWeight = alignmentW;
    }

    public void SetSeparation(float separationW) {
        separationWeight = separationW;
    }

    public void SetCohesion(float cohesionW) {
        cohesionWeight = cohesionW;
    }
}
