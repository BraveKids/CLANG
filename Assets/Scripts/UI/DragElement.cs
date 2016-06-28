using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;

//using UnityEditor;


public class DragElement : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public GameObject summonParticle;
    public GameObject prefabObject;
    float pulsePrice;
    public GameObject dragObject;
    public float heightFloat;
    Vector3 spawnPoint;
    GameObject strategist;
    StrategistPulse strategistPulse;
    StrategistSpawner strategistSpawner;
    private Plane plane = new Plane(Vector3.up, Vector3.zero);
    Camera strategistCamera;
    public float cooldownTimer;
    bool cooldown;
    public float timer;
    public Text pulseCostText;
    Vector3 worldPosition;
    private Color startColor;
    private Color notAvailableColor;
    private Image elementImage;
    public bool isWorldPositionCorrect = false;
    public Text popUpMessageText;

    public void Toggle()
    {
        enabled = !enabled;
    }

    void Start()
    {
        cooldown = false;
        strategist = GameElements.getStrategist();
        strategistCamera = strategist.GetComponent<StrategistSpawner>().strategistCamera;
        strategistPulse = strategist.GetComponent<StrategistPulse>();
        strategistSpawner = strategist.GetComponent<StrategistSpawner>();
        pulsePrice = prefabObject.GetComponent<PulsePrice>().pulsePrice;
        pulseCostText.text = string.Format("{0}", pulsePrice);
        elementImage = GetComponent<Image>();
        startColor = elementImage.color;
        notAvailableColor = elementImage.color;
        notAvailableColor.a = 0.5f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //dragObject = Instantiate(gameObject, eventData.position, Quaternion.identity) as GameObject;
    }

    void Update()
    {
        if (cooldown)
        {
            timer += Time.deltaTime;
            elementImage.fillAmount += 1.0f / cooldownTimer * Time.deltaTime;
            if (timer >= cooldownTimer)
            {
                timer = 0.0f;
                cooldown = false;
            }
        }
        if (strategistPulse.GetPulse() < pulsePrice)
            elementImage.color = notAvailableColor;
        else
            elementImage.color = startColor;
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        dragObject.transform.position = GetWorldPositionOnPlane(ped.position);
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        if (!cooldown)
        {
            if (strategistPulse.GetPulse() >= pulsePrice)
            {
                spawnPoint = ped.position;
                worldPosition = GetWorldPositionOnPlane(spawnPoint);
                if (Physics.Raycast(worldPosition, Vector3.up, 3.0f) != true)
                {
                    strategistPulse.SpawnPrice(pulsePrice);
                    strategistSpawner.Spawn(summonParticle, worldPosition);
                    Invoke("Summon", 0.8f);
                    elementImage.fillAmount = 0f;
                    cooldown = true;
                }
            }
            else
            {
                popUpMessageText.text = "YOU NEED " + pulsePrice + " PULSE POINTS TO SPWN THIS CARD";
                popUpMessageText.GetComponent<Animator>().SetTrigger("IsOpen");
            }
        }
        dragObject.transform.position = new Vector3(1000f, 1000f, 1000f);
    }
    void Summon()
    {
        if (!isWorldPositionCorrect)
            strategistSpawner.Spawn(prefabObject, worldPosition);
        else
            strategistSpawner.Spawn(prefabObject, new Vector3(worldPosition.x - 4f, worldPosition.y - 5.5f, worldPosition.z));
    }

    public Vector3 GetWorldPositionOnPlane(Vector3 pointerPosition)
    {
        float distance;

        Ray ray = strategistCamera.ScreenPointToRay(pointerPosition);

        //Camera.main.ScreenPointToRay(pointerPosition);
        if (plane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            //Just double check to ensure the y position is exactly zero
            hitPoint.y = heightFloat;
            return hitPoint;
        }
        return Vector3.zero;
    }
}
