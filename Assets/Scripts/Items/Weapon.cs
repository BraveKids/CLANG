using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Weapon : MonoBehaviour
{
    public int damage;
    public int integrity;
    public float rateOfAttack;
    // Use this for initialization


    void Update()
    {

    }

    float ChekRate()
    {
        return rateOfAttack;
    }


    int CheckIntegrity()
    {
        return integrity;
    }

    void DecreaseIntegrity(int decreaseAmount)
    {
        integrity -= decreaseAmount;
    }

    public int ReturnDamage()
    {
        return damage;
    }
}
