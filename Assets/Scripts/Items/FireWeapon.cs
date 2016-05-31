using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class FireWeapon : Weapon
{

    public Transform shootPosition;
    public float cooldown;
    public float overload;
    public GameObject bulletPrefab;
    public bool canFire;

    void Start()
    {

    }
    public void WaitCooldown()
    {

    }

    public float CheckOverload()
    {
        return overload;
    }

    public void IncreaseOverload(float increaseAmount)
    {
        overload += increaseAmount;
    }

    public void ResetOverload()
    {
        overload = 0.0f;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
