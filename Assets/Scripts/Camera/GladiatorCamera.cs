using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class GladiatorCamera : MonoBehaviour {
    Quaternion initRot;
    public int cameraType;
    public float cameraDistance;
    public float cameraAngle = 45;
    GameObject player;
    float playerX;
    float playerY;
    float playerZ;
    float cameraX;
    float cameraY;
    float cameraZ;
    GameObject menuCamera;
    // Use this for initialization
    void Start() {
        initRot = transform.rotation;
        player = GameObject.FindGameObjectWithTag("Gladiator");

    }



    // Update is called once per frame
    void Update() {

        playerX = player.transform.position.x;
        playerY = player.transform.position.y;
        playerZ = player.transform.position.z;
        cameraX = transform.position.x;
        cameraY = transform.position.y;
        cameraZ = transform.position.z;

        cameraX = playerX;


        //Move on start after set them
        float sin = Mathf.Sin(cameraAngle * (Mathf.PI / 180));
        float cos = Mathf.Cos(cameraAngle * (Mathf.PI / 180));
        cameraZ = playerZ - cameraDistance * cos;
        cameraY = playerY + cameraDistance * sin;


        transform.position = new Vector3(cameraX, cameraY, cameraZ);
        transform.LookAt(player.transform);
        transform.rotation = initRot;


    }
}
