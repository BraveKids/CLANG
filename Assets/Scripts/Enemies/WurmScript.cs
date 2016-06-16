using UnityEngine;
using System.Collections;

public class WurmScript : MonoBehaviour {
    Animator anim;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        anim.SetBool("Attack", true);
        Invoke("Hide", 1f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Hide()
    {
        anim.SetBool("Attack", false);
    }
}
