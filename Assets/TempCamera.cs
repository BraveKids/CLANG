using UnityEngine;
using System.Collections;

public class TempCamera : MonoBehaviour {

    public GameObject target;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y+2f, target.transform.position.z - 5f);
        transform.LookAt(target.transform);
            
	}
}
