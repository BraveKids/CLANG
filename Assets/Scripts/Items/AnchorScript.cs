using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class AnchorScript : MonoBehaviour
{
    public Transform anchorPosHand;
    public Transform anchorPosElbow;
    //GameObject gladiator;
    // Use this for initialization
    void Start()
    {
        gameObject.transform.parent = anchorPosHand;
        gameObject.transform.position = anchorPosHand.position;
        //gladiator = GameElements.getGladiator();
        //gladiator.GetComponent<GladiatorShooting>().handWeapon = this.gameObject;
        //anchorPosHand = gladiator.GetComponent<GladiatorShooting>().handPosition;
        //anchorPosElbow = gladiator.GetComponent<GladiatorShooting>().elbowPosition;

    }

    // Update is called once per frame
    void Update()
    {

        //gameObject.transform.position = new Vector3(anchorPosHand.position.x, anchorPosHand.position.y, anchorPosHand.position.z);
        gameObject.transform.rotation = new Quaternion(Quaternion.LookRotation(anchorPosHand.position - anchorPosElbow.position).x,
                                                       Quaternion.LookRotation(anchorPosHand.position - anchorPosElbow.position).y,
                                                       Quaternion.LookRotation(anchorPosHand.position - anchorPosElbow.position).z,
                                                       Quaternion.LookRotation(anchorPosHand.position - anchorPosElbow.position).w);
    }
}
