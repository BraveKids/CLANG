using UnityEngine;
using System.Collections;

public class RandomMovement : MonoBehaviour {

    // Use this for initialization
    public float vel;
	void Start () {
        StartCoroutine(ChangeDir());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator ChangeDir() {
        while (true) {
            GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-vel, vel), 0f, Random.Range(-vel, vel));
            yield return new WaitForSeconds(Random.Range(0f, 4f));
        }
    }
}
