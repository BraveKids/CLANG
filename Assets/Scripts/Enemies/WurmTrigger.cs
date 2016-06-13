using UnityEngine;
using System.Collections;

public class WurmTrigger : MonoBehaviour {
    bool triggered;
    public WormIA IAScript;
    // Use this for initialization
    void Start() {
        triggered = false;
    }

    // Update is called once per frame
    void Update() {

    }


    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject == IAScript.target )
        {
            triggered = true;
        }
    }


    void OnTriggerExit(Collider col)
    {
        if (col.gameObject == IAScript.target)
        {
            triggered = false;
        }
    }
    public bool getTriggered()
    {
        return triggered;
    }
}
