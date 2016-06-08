using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {
    public float damage;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider obj)
    {
        if(obj.tag == "Gladiator")
        {
            obj.GetComponent<GladiatorHealth>().Damage(damage);
        }
    }
}
