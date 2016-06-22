using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;

//using UnityEditor;


public class DragElementWurm : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public GameObject summonParticle;
    public GameObject prefabObject;
    float pulsePrice;
    public GameObject dragObject;
    public float heightFloat;
    Vector3 spawnPoint;
    GameObject strategist;
    StrategistPulse strategistPulse;
    private Plane plane = new Plane(Vector3.up, Vector3.zero);
    Camera strategistCamera;
    public float cooldownTimer;
    bool cooldown;
    public float timer;
    public Text pulseCostText;
    Vector3 worldPosition;
    Vector3 worldCorrectPosition;

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
        pulsePrice = prefabObject.GetComponent<PulsePrice>().pulsePrice;
        pulseCostText.text = string.Format("{0}", pulsePrice);

        //gameObject.GetComponent<RawImage>().texture = AssetPreview.GetAssetPreview(prefabObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //dragObject = Instantiate(gameObject, eventData.position, Quaternion.identity) as GameObject;
    }

    void Update()
    {
        if (cooldown)
        {
            GetComponent<Image>().color = Color.blue;
            timer += Time.deltaTime;
            if (timer >= cooldownTimer)
            {
                GetComponent<Image>().color = Color.white;
                timer = 0.0f;
                cooldown = false;
            }
        }
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
                    
                    strategist.GetComponent<StrategistSpawner>().Spawn(summonParticle, worldPosition);
                    Invoke("Summon", 0.8f);
                    
                    cooldown = true;
                }
                else
                {

                }
            }
        }
        dragObject.transform.position = new Vector3(1000f, 1000f, 1000f);
    }

    void Summon()
    {
        worldCorrectPosition = new Vector3(worldPosition.x - 4f, worldPosition.y - 5.5f, worldPosition.z);
        strategist.GetComponent<StrategistSpawner>().Spawn(prefabObject, worldCorrectPosition);
    }
    /*
    [Command]
    public void CmdSpawn(Vector3 position)
    {
        var enemy = (GameObject)Instantiate(prefabObject, GetWorldPositionOnPlane(position), Quaternion.identity);
        NetworkServer.Spawn(enemy);
    }
    */
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
