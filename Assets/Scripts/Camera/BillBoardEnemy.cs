using UnityEngine;
using System.Collections;

public class BillBoardEnemy : MonoBehaviour {
    Transform pointOfView;
    GameObject targetCamera;
    GameObject gladiatorCamera;
    GameObject strategistCamera;
    // Use this for initialization

    void Start()
    {
        pointOfView = GameObject.FindGameObjectWithTag("PointOfView").transform;
        gladiatorCamera = GameObject.FindGameObjectWithTag("GladiatorCamera");

        if (gladiatorCamera == null)
        {
            strategistCamera = GameElements.getStrategist().transform.FindChild("Camera").gameObject;
            targetCamera = strategistCamera;
        }
        else
        {
            targetCamera = gladiatorCamera;
        }


    }

    // Update is called once per frame
    void Update()
    {

        transform.LookAt(pointOfView);
    }
}

