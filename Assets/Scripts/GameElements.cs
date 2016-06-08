using UnityEngine;
using System.Collections;

public class GameElements : MonoBehaviour {

    static GameObject currentCamera;
    static GameObject menuCanvas;
    static GameObject gladiatorCanvas;
    static GameObject strategistCanvas;
    static GameObject virtualJoystick;
    static GameObject strategist;
    static GameObject gladiator;
    static GladiatorHealth gladiatorHealth;
    static bool armorDropped;
    static bool medDropped;
    static bool weaponDropped;
    static int monster;
    static int intWeapon;

    // Use this for initialization
    void Start() {

        //strategist = null;
        //setCurrentCamera( Camera.main.gameObject);

        //menuCanvas = GameObject.FindGameObjectWithTag("MenuCanvas");
        gladiatorCanvas = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("GladiatorCanvas").gameObject;
        //strategistCanvas = GameObject.FindGameObjectWithTag("StrategistCanvas");
        //Debug.Log(strategistCanvas);
        virtualJoystick = gladiatorCanvas.transform.FindChild("VirtualJoypad/TapArea/BackgroundImage").gameObject;


    }

    // Update is called once per frame
    void Update() {

    }




    public static void setCurrentCamera(GameObject camera) {
        currentCamera = camera;
    }

    public static GameObject getCurrentCameraObj() {
        return currentCamera;
    }

    public static Camera getCurrentCamera() {
        return currentCamera.GetComponent<Camera>();
    }



    public static GameObject getGladiatorCanvas() {
        return gladiatorCanvas;
    }

    public static GameObject getStrategistCanvas() {
        return strategistCanvas;
    }

    public static GameObject getMenuCanvas() {
        return menuCanvas;
    }

    public static GameObject getVirtualJoystick() {
        return virtualJoystick;
    }


    public static GameObject getStrategist() {
        return strategist;
    }

    public static void setStrategist(GameObject strategistObj) {
        strategist = strategistObj;
    }

    public static GameObject getGladiator() {
        return gladiator;
    }

    public static void setGladiator(GameObject gladiatorObj) {
        gladiator = gladiatorObj;
    }


    public static float getGladiatorLife() {
        return gladiator.GetComponent<GladiatorHealth>().getLife();
    }

    public static float getGladiatorArmor() {
        return gladiator.GetComponent<GladiatorHealth>().getArmor();
    }

    public static float getMaxLife() {
        return gladiator.GetComponent<GladiatorHealth>().getMaxLife();
    }

    public static float getMaxArmor() {
        return gladiator.GetComponent<GladiatorHealth>().getMaxArmor();
    }

    public static void setGladiatorHealth(GladiatorHealth _gladiatorHealth) {
        gladiatorHealth = _gladiatorHealth;
    }

    public static bool getArmorDropped() {
        return armorDropped;
    }

    public static void setArmorDropped(bool dropped) {
        armorDropped = dropped;
    }

    public static bool getMedDropped() {
        return medDropped;
    }

    public static void setMedDropped(bool dropped) {
        medDropped = dropped;
    }

    public static void setIntegrity(int dropped) {
        intWeapon = dropped;
    }

    public static int getIntegrity() {
        return intWeapon;
    }

    public static int getMaxIntegrity() {
        //TODO
        return 10;
    }

    public static void setWeaponDropped(bool dropped) {
        weaponDropped = dropped;
    }

    public static void getWeaponDropped() {
        return weaponDropped;
    }


}
