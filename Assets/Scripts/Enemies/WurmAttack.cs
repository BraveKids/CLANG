using UnityEngine;
using System.Collections;

public class WurmAttack : MonoBehaviour {
    GameObject gladiator;
    GladiatorHealth gladiatorHealth;
    public float damage;
    public bool alreadyAttack;
	// Use this for initialization
	void Start () {
        alreadyAttack = false;
        gladiator = GameElements.getGladiator();
        gladiatorHealth = gladiator.GetComponent<GladiatorHealth>();

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision hit)
    {

        if(hit.gameObject.tag == "Gladiator")
        {
            if(!alreadyAttack)
            gladiatorHealth.Damage(damage);
            alreadyAttack = true;
        }
    }
}
