using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Weapon : NetworkBehaviour
{
    public int damage;
    public int integrity;
    public int maxIntegrity;
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

    public int getIntegrity() {
        return integrity;
    }

    public int getMaxIntegrity() {
        return maxIntegrity;
    }
}
