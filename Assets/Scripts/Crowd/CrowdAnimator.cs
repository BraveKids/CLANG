using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CrowdAnimator : MonoBehaviour {
    Animator anim;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        float delay = Random.Range(0f, 2f);
        Invoke("Cheer", delay);
       
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Cheer()
    {
        anim.SetBool("Cheer", true);
    }
}


