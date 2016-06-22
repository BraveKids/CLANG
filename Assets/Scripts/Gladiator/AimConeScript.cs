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
        if (col.tag == "Enemy" || col.tag == "Tank")
        {
            gladiatorScript.AddTarget(col.gameObject.transform);
        }
        if(col.tag == "Wurm")
        {
            gladiatorScript.targets.Add(col.transform.parent.parent.parent);
            //gladiatorScript.AddTarget(col.transform.parent.parent.parent.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Enemy")
        {
            gladiatorScript.targets.Remove(col.gameObject.transform);
        }
        if (col.tag == "Wurm")
        {
            gladiatorScript.targets.Remove(col.transform.parent.parent.parent);
        }
    }
}
