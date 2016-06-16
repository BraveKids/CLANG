using UnityEngine;
using System.Collections;

public class LevitateScript : MonoBehaviour {
    public Transform startPosition;
    public Transform endPosition;
    Transform target;
    bool done;
    InteractableObject objectScript;

    void Start()
    {
        done = false;
        target = startPosition;
        objectScript = GetComponentInParent<InteractableObject>();
    }

    void Update()
    {
        if (!objectScript.taken && !done)
        {
            if (transform.position == endPosition.position)
            {
                target = startPosition;
            }
            else if (transform.position == startPosition.position)
            {
                target = endPosition;
            }

            transform.position = Vector3.MoveTowards(transform.position, target.position, 0.3f * Time.deltaTime);
        }
        else
        {

            transform.position = startPosition.position;
            done = true;
            return;
        }
    }
	
}
