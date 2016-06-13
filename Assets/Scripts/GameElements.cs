using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
public class GameElements : NetworkBehaviour
{

    static GameObject currentCamera;
    static GameObject menuCanvas;
    static GameObject gladiatorCanvas;
    static GameObject strategistCanvas;
    static GameObject virtualJoystick;
    static GameObject strategist;
    static GameObject gladiator;
    static GameObject arena;
    static GladiatorHealth gladiatorHealth;
    static bool armorDropped = false;
    static bool medDropped = false;
    static bool weaponDropped = false;
    static int enemy = 0;
    static int intWeapon;
    public List<GameObject> targets;


    // Use this for initialization
    void Start()
    {
        enemy = 0;
        targets = new List<GameObject>();
        arena = GameObject.FindGameObjectWithTag("Arena");
        //strategist = null;
        //setCurrentCamera( Camera.main.gameObject);

        //menuCanvas = GameObject.FindGameObjectWithTag("MenuCanvas");
        gladiatorCanvas = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("GladiatorCanvas").gameObject;
        //strategistCanvas = GameObject.FindGameObjectWithTag("StrategistCanvas");
        //Debug.Log(strategistCanvas);
        virtualJoystick = gladiatorCanvas.transform.FindChild("VirtualJoypad/TapArea/BackgroundImage").gameObject;


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddTarget(GameObject target)
    {
        if (!targets.Contains(target))
        {
            targets.Add(target);
        }
    }

    public void RemoveTarget(GameObject target)
    {
        targets.Remove(target);
    }


    public static GameObject getArena()
    {
        return arena;
    }

    public static void setArena(GameObject _arena)
    {
        arena = _arena;
    }

    public static void setCurrentCamera(GameObject camera)
    {
        currentCamera = camera;
    }

    public static GameObject getCurrentCameraObj()
    {
        return currentCamera;
    }

    public static Camera getCurrentCamera()
    {
        return currentCamera.GetComponent<Camera>();
    }



    public static GameObject getGladiatorCanvas()
    {
        return gladiatorCanvas;
    }

    public static GameObject getStrategistCanvas()
    {
        return strategistCanvas;
    }

    public static GameObject getMenuCanvas()
    {
        return menuCanvas;
    }

    public static GameObject getVirtualJoystick()
    {
        return virtualJoystick;
    }


    public static GameObject getStrategist()
    {
        return strategist;
    }

    public static void setStrategist(GameObject strategistObj)
    {
        strategist = strategistObj;
    }

    public static GameObject getGladiator()
    {
        return gladiator;
    }

    public static void setGladiator(GameObject gladiatorObj)
    {
        gladiator = gladiatorObj;
    }


    public static float getGladiatorLife()
    {
        return gladiator.GetComponent<GladiatorHealth>().getLife();
    }

    public static float getGladiatorArmor()
    {
        return gladiator.GetComponent<GladiatorHealth>().getArmor();
    }

    public static float getMaxLife()
    {
        return gladiator.GetComponent<GladiatorHealth>().getMaxLife();
    }

    public static float getMaxArmor()
    {
        return gladiator.GetComponent<GladiatorHealth>().getMaxArmor();
    }

    public static void setGladiatorHealth(GladiatorHealth _gladiatorHealth)
    {
        gladiatorHealth = _gladiatorHealth;
    }

    public static bool getArmorDropped()
    {
        return armorDropped;
    }

    public static void setArmorDropped(bool dropped)
    {
        armorDropped = dropped;
    }

    public static bool getMedDropped()
    {
        return medDropped;
    }

    public static void setMedDropped(bool dropped)
    {
        medDropped = dropped;
    }

    public static int getIntegrity()
    {
        if (gladiator.GetComponent<GladiatorShooting>().fireWeapon != null)
        {
            return gladiator.GetComponent<GladiatorShooting>().fireWeapon.GetComponent<FireWeapon>().getIntegrity(); ;
        }
        else
            return 0;
    }

    public static int getMaxIntegrity()
    {
        if (gladiator.GetComponent<GladiatorShooting>().fireWeapon != null)
        {
            return gladiator.GetComponent<GladiatorShooting>().fireWeapon.GetComponent<FireWeapon>().getMaxIntegrity();
        }
        else
            return 0;
    }

    public static void setWeaponDropped(bool dropped)
    {
        weaponDropped = dropped;
    }

    public static bool getWeaponDropped()
    {
        return weaponDropped;
    }

    public static void decreaseEnemy()
    {
        enemy--;
    }

    public static void increaseEnemy()
    {
        enemy++;
    }

    public static int getEnemyCount()
    {
        Debug.Log(enemy);
        return enemy;
    }


}
