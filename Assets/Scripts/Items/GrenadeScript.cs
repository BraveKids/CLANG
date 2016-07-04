using UnityEngine;
using System.Collections;

public class GrenadeScript : MonoBehaviour {
    public GameObject explosion;
    AudioSource audioBoom;
    public GameObject attackTrigger;
    public bool boom;
	// Use this for initialization
	void Start () {
        boom = false;
        attackTrigger.SetActive(false);
        audioBoom = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "ArenaBox" || col.gameObject.tag == "Enemy" || col.gameObject.tag == "Tank" || col.gameObject.tag == "Wurm")
        {
            audioBoom.PlayOneShot(audioBoom.clip);
            attackTrigger.SetActive(true);
            boom = true;
            explosion.SetActive(true);

            Invoke("Destroy", 0.6f);
        }

    }

    void Destroy()
    {
        Destroy(gameObject);
    }


}
