using UnityEngine;
using System.Collections;

public class WrenchScript : MonoBehaviour {
    public float m_Damage;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

     void OnTriggerEnter(Collider obj)
    {
        if (obj.CompareTag("Enemy"))
        {
            obj.GetComponent<EnemyHealth>().Damage(m_Damage);
            //gameObject.SetActive(false);
        }
    }
}
