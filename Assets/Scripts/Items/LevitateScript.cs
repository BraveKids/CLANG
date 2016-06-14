using UnityEngine;
using System.Collections;

public class LevitateScript : MonoBehaviour {
    public Transform startPosition;
    public Transform endPosition;
    Transform target;
    InteractableObject objectScript;

    void Start()
    {
        target = startPosition;
        objectScript = GetComponentInParent<InteractableObject>();
    }

    void Update()
    {
        if (!objectScript.taken)
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
    }
	
}
