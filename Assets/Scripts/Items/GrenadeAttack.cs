using UnityEngine;
using System.Collections;

public class GrenadeAttack : MonoBehaviour
{

    public float m_Damage;
    // Use this for initialization
    void Start()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy" || col.gameObject.tag == "Wurm")
        {

            col.gameObject.GetComponentInParent<EnemyHealth>().Damage(m_Damage);

        }
    }
}
