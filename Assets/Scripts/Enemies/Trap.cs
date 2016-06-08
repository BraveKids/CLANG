using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Trap : NetworkBehaviour
{
    //public Collider actionArea;
    public int damage;
    public float timeOfLife = 5f;
    public float timer = 0.0f;
    Animator anim;
    


    // Use this for initialization
    void Start()
    {
        timeOfLife = 5.0f;
        anim = GetComponent<Animator>();
        anim.SetBool("snap", false);
    }

    // Update is called once per frame
    void Update()
    {
        
        timer += Time.deltaTime;
        if(timer >= timeOfLife)
        {
            Deactivate();
        }
        
    }

    public void makeDamage(GameObject player)
    {
        player.GetComponent<GladiatorHealth>().Damage(damage);
        Invoke("Deactivate", 0.5f);
    }

    public void Deactivate()
    {
        //gameObject.SetActive(false);
        //Destroy(gameObject);
        GameElements.getGladiator().GetComponent<GladiatorShooting>().CmdDestroyEnemy(gameObject);
    }

 

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Gladiator")
        {
            anim.SetBool("snap", true);
            makeDamage(col.gameObject);

        }
    }

}