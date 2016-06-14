using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PoisonCloudScript : NetworkBehaviour {
    public bool poison;
    public float timeOfLife;
    public float timer = 0.0f;
    public float damage = 4f;
    GameObject player;
	// Use this for initialization
	void Start () {
        poison = false;
        player = GameElements.getGladiator();
        StartCoroutine(Poison());
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer >= timeOfLife)
        {
            Deactivate();
        }
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

    public void Deactivate()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
        GameElements.decreaseEnemy();
        //GameElements.getGladiator().GetComponent<GladiatorShooting>().DestroyEnemy(gameObject);
    }


    IEnumerator Poison()
    {
        while (true)
        {
            if (poison)
            {
                player.GetComponent<GladiatorHealth>().Damage(damage);
                yield return  new WaitForSeconds(2);
            }
            else
            {
                yield return null;
            }
        }
    }
}
