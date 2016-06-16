using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    public float m_Damage;
    // Use this for initialization
    void Start()
    {

    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Enemy" || col.gameObject.tag == "Wurm")
        {

            col.gameObject.GetComponentInParent<EnemyHealth>().Damage(m_Damage);
            
        }
        Destroy(gameObject,0.2f);

        

    }
}
