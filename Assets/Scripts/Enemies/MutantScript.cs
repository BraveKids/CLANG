using UnityEngine;
using System.Collections;


public class MutantScript : MonoBehaviour {
     AILerp agent;
	// Use this for initialization
	void Start () {
        agent = GetComponent<AILerp>();
        agent.target = GameElements.getGladiator().transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
