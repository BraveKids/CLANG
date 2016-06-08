using UnityEngine;
using System.Collections;

public class CollisionDetector : MonoBehaviour {
    public bool colliding;
	// Use this for initialization
	void Start () {
        colliding = false;
	}
	
	void OnTriggerEnter(Collider col)
    {
        colliding = true;
    }

    void OnTriggerExit(Collider col)
    {
        colliding = false;
    }
}
