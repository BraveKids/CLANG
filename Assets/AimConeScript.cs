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
        if(col.tag == "Wurm")
        {
            gladiatorScript.AddTarget(col.transform.parent.parent.parent.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Enemy")
        {
            gladiatorScript.RemoveTarget(col.gameObject);
        }
        if (col.tag == "Wurm")
        {
            gladiatorScript.RemoveTarget(col.transform.parent.parent.parent.gameObject);
        }
    }
}
