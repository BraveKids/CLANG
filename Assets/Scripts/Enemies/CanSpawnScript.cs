using UnityEngine;
using System.Collections;

public class CanSpawnScript : MonoBehaviour {
    public bool canSpawn;
	// Use this for initialization
	void Start () {
        canSpawn = true;
	}
	
    void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("ArenaBox")){
            canSpawn = false;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (!canSpawn)
        {
            canSpawn = true;
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
