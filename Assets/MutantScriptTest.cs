using UnityEngine;
using System.Collections;

public class MutantScriptTest : MonoBehaviour {
    Animator m_animator;
	// Use this for initialization
	void Start () {
        m_animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("MeleeAttackTrigger"))
        {
            //m_animator.SetTrigger("Damage");
            gameObject.transform.FindChild("Model").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
        }
    }
    

    }

