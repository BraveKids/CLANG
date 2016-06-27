using UnityEngine;
using System.Collections;

public class BeeAI : MonoBehaviour {

    SwarmAI swarm;
    Rigidbody rigidbody;
   public GameObject fakeParent;
    Vector3 velocity;
   
    // Use this for initialization
    void Start() {

        swarm = fakeParent.GetComponent<SwarmAI>();
        velocity = Vector3.zero;
        rigidbody = GetComponent<Rigidbody>();
        //transform.localPosition = transform.position;
        StartCoroutine(NextVel());
    }

    // Update is called once per frame
    void Update() {
        //Invoke("NextVel", Random.Range(0f,5f));  
        //rigidbody.velocity = velocity;

        Quaternion rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0f, velocity.z));

        //rigidbody.MoveRotation(rotation);
        rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, rotation, 5f * Time.deltaTime));
        Debug.DrawLine(transform.position, new Vector3(transform.position.x + velocity.x, transform.position.y + velocity.y, transform.position.z + velocity.z), Color.green, 0, false);

    }

    IEnumerator NextVel() {
        while (true) {
            Vector3 alignment = Alignment() * swarm.alignmentWeight;
            Vector3 cohesion = Cohesion() * swarm.cohesionWeight;
            Vector3 separation = Separation() *swarm.separationWeight;
            Vector3 chasee = swarm.gameObject.transform.position - transform.position;
            velocity = alignment * swarm.alignmentWeight + cohesion * swarm.cohesionWeight + separation * swarm.separationWeight;
            if (swarm.follow) velocity += (chasee*swarm.followWeight);

            //limit max and min velocity
            if (velocity.x >= swarm.maxVel) velocity.x = swarm.maxVel;
            if (velocity.y >= swarm.maxVel) velocity.y = swarm.maxVel;
            if (velocity.z >= swarm.maxVel) velocity.z = swarm.maxVel;

            if (velocity.x <= -swarm.maxVel) velocity.x = -swarm.maxVel;
            if (velocity.y <= -swarm.maxVel) velocity.y = -swarm.maxVel;
            if (velocity.z <= -swarm.maxVel) velocity.z = -swarm.maxVel;

            //take the agents inside a box
            if (transform.position.x >= swarm.centerPosition.x + swarm.boxDimension / 2 && velocity.x >= 0) velocity.x = -0.1f;
            if (transform.position.x <= swarm.centerPosition.x - swarm.boxDimension / 2 && velocity.x <= 0) velocity.x = 0.1f;

            if (transform.position.y >= swarm.centerPosition.y + swarm.boxDimension / 2 && velocity.y >= 0) velocity.y = -0.1f;
            if (transform.position.y <= swarm.centerPosition.y - swarm.boxDimension / 2 && velocity.y <= 0) velocity.y = 0.1f;

            if (transform.position.z >= swarm.centerPosition.z + swarm.boxDimension / 2 && velocity.z >= 0) velocity.z = -0.1f;
            if (transform.position.z <= swarm.centerPosition.z - swarm.boxDimension / 2 && velocity.z <= 0) velocity.z = 0.1f;



            rigidbody.velocity = velocity;
            rigidbody.velocity.Normalize();
            


            yield return new WaitForSeconds(Random.Range(0f, .5f));
        }
    }

    

    Vector3 Alignment() {
        Vector3 alignment = Vector3.zero;
        foreach (GameObject agent in swarm.GetSwarm()) {
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

    
}
