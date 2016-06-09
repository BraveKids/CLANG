using UnityEngine;
using System.Collections;

public class PoisonCloudScript : MonoBehaviour {
    public bool poison;
    public float damage = 1f;
    GameObject player;
	// Use this for initialization
	void Start () {
        poison = false;
        player = GameElements.getGladiator();
        StartCoroutine(Poison());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerStay(Collider obj)
    {
        if(obj.tag == "Gladiator")
        {
            poison = true;
        }
    }

    void OnTriggerExit(Collider obj)
    {
        if(obj.tag == "Gladiator")
        {
            poison = false;
        }
    }


    IEnumerator Poison()
    {
        while (true)
        {
            if (poison)
            {
                player.GetComponent<GladiatorHealth>().Damage(damage);
                yield return  new WaitForSeconds(3);
            }
            else
            {
                yield return null;
            }
        }
    }
}
