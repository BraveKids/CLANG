using UnityEngine;
using System.Collections;

public class SwarmCamera : MonoBehaviour {
    GameObject swarm;
	// Use this for initialization
	void Start () {
        swarm = GameObject.Find("Swarm");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(swarm.transform.position.x, swarm.transform.position.y + 5f, swarm.transform.position.z - 5f);
	}
}
