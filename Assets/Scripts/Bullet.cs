using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    public float m_Damage;
	// Use this for initialization
	void Start () {
	
	}
	
    void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.CompareTag("Enemy"))
        {
            obj.GetComponent<EnemyHealth>().Damage(m_Damage);
        }
        Destroy(this.gameObject);
    }

	// Update is called once per frame
	void Update () {
	
	}
}
