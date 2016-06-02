using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;

//using UnityEditor;


public class DragElement : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {
    public GameObject prefabObject;
    float pulsePrice;
    public GameObject dragObject;
    public float heightFloat;
    Vector3 spawnPoint;
    GameObject strategist;
    StrategistPulse strategistPulse;
    private Plane plane = new Plane(Vector3.up, Vector3.zero);
    Camera strategistCamera;
    public void Toggle() {
        enabled = !enabled;
    }

    void Start () {
        strategist = GameElements.getStrategist();
        strategistCamera = strategist.GetComponent<StrategistSpawner>().strategistCamera;
        strategistPulse = strategist.GetComponent<StrategistPulse>();
        pulsePrice = prefabObject.GetComponent<PulsePrice>().pulsePrice;
        //gameObject.GetComponent<RawImage>().texture = AssetPreview.GetAssetPreview(prefabObject);
    }

    public void OnBeginDrag (PointerEventData eventData) {
        //dragObject = Instantiate(gameObject, eventData.position, Quaternion.identity) as GameObject;
    }

    /*void Update() {
        dragObject.transform.position = GetWorldPositionOnPlane(Input.mousePosition);
    }*/

    public virtual void OnDrag (PointerEventData ped) {
        dragObject.transform.position = GetWorldPositionOnPlane(ped.position);
    }

    public virtual void OnPointerDown (PointerEventData ped) {
        OnDrag(ped);
    }

    public virtual void OnPointerUp (PointerEventData ped) {
        if (strategistPulse.GetPulse() >= pulsePrice)
        {
            strategistPulse.SpawnPrice(pulsePrice);
            spawnPoint = ped.position;
            strategist.GetComponent<StrategistSpawner>().Spawn(prefabObject, GetWorldPositionOnPlane(spawnPoint));
            
        }
        dragObject.transform.position = new Vector3(1000f, 1000f, 1000f);
    }
    /*
    [Command]
    public void CmdSpawn(Vector3 position)
    {
        var enemy = (GameObject)Instantiate(prefabObject, GetWorldPositionOnPlane(position), Quaternion.identity);
        NetworkServer.Spawn(enemy);
    }
    */
    public Vector3 GetWorldPositionOnPlane (Vector3 pointerPosition) {
        float distance;
       
        Ray ray = strategistCamera.ScreenPointToRay(pointerPosition); 
            //Camera.main.ScreenPointToRay(pointerPosition);
        if (plane.Raycast(ray, out distance)) {
            Vector3 hitPoint = ray.GetPoint(distance);
            //Just double check to ensure the y position is exactly zero
            hitPoint.y = heightFloat;
            return hitPoint;
        }
        return Vector3.zero;
    }
}
