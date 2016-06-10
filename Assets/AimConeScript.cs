using UnityEngine;
using System.Collections;

public class AimConeScript : MonoBehaviour
{
    public GameObject gladiator;
    GladiatorShooting gladiatorScript;


    void Start()
    {
        gladiatorScript = gladiator.GetComponent<GladiatorShooting>();
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy")
        {
            gladiatorScript.AddTarget(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Enemy")
        {
            gladiatorScript.RemoveTarget(col.gameObject);
        }
    }
}
