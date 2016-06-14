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

    void OnTriggerEnter(Collider obj)
    {
        if (obj.tag == "Enemy" || obj.tag == "Wurm")
        {
            
            obj.GetComponentInParent<EnemyHealth>().Damage(m_Damage);
            
        }
        Destroy(gameObject);

        

    }
}
