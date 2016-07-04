using UnityEngine;
using System.Collections;

public class BeeAI : MonoBehaviour {

    SwarmAI swarm;
    Rigidbody rigidbody;
    //we don't want to be child of the swarm because it rotate and we don't need it, but we do want to be connected with it
    //we also need this as target. The fake parent will follow the real target using a*, we'll follow the fake parent with flocking
    public GameObject fakeParent;       
    Vector3 velocity;

    // Use this for initialization
    void Start() {

        swarm = fakeParent.GetComponent<SwarmAI>();
        velocity = Vector3.zero;
        rigidbody = GetComponent<Rigidbody>();
        StartCoroutine(NextVel());
    }

    void Update() {
        Quaternion rotation = Quaternion.LookRotation(velocity);
        rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, rotation, 5f * Time.deltaTime));
        Debug.DrawLine(transform.position, new Vector3(transform.position.x + velocity.x, transform.position.y + velocity.y, transform.position.z + velocity.z), Color.green, 0, false);
    }

    //here we compute all the component of the flocking and update the velocity vector
    IEnumerator NextVel() {
        while (true) {
            Vector3 alignment = Alignment() * swarm.alignmentWeight;
            Vector3 cohesion = Cohesion() * swarm.cohesionWeight;
            Vector3 separation = Separation() * swarm.separationWeight;
            Vector3 chasee = swarm.gameObject.transform.position - transform.position;      //this is the "chase the target" component
            velocity = alignment * swarm.alignmentWeight + cohesion * swarm.cohesionWeight + separation * swarm.separationWeight;
            if (swarm.follow) velocity += (chasee * swarm.followWeight);

            //Since we want to reproduce an almost natural movement but we also want the swarm to be as one...

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



            //update velocity
            rigidbody.velocity = velocity;
            rigidbody.velocity.Normalize();


            //to obtain a random behaviour we don't update all the bee's velocity at same time
            yield return new WaitForSeconds(Random.Range(0f, .5f));
        }
    }



    Vector3 Alignment() {
        Vector3 alignment = Vector3.zero;
        foreach (GameObject agent in swarm.GetSwarm()) {
            alignment += agent.transform.GetComponent<Rigidbody>().velocity;
        }
        alignment.Normalize();      
        return alignment;
    }

    Vector3 Cohesion() {
        Vector3 cohesion;       
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

    void OnDestroy() {
        swarm.target.GetComponent<GladiatorShooting>().DestroyEnemy(gameObject);
    }


}
