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
        if (col.gameObject.tag == "Enemy" || col.gameObject.tag == "Wurm" || col.gameObject.tag == "Tank")
        {
            /*if (col.gameObject.tag == "Tank")
            {
                col.gameObject.GetComponentInParent<TankAI>().isDamaged = true;
            }*/

            col.gameObject.GetComponentInParent<EnemyHealth>().Damage(m_Damage);
            
        }
       
        Destroy(gameObject,0.2f);

        

    }
}
