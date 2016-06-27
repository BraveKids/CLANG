using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
 
    Transform pointOfView;

    // Use this for initialization

    void Start () {
            pointOfView = GameObject.FindGameObjectWithTag("PointOfView").transform;
         
    
	}
	
	// Update is called once per frame
	void Update () {
      
        transform.LookAt(pointOfView);
	}
}
