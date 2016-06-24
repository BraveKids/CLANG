using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

public class ButtonScript : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public string command;
    //public GameObject trigger;
    GameObject player;
    private Image bgImg;
    //public Color triggerColor;
    private void Start()
    {
        player = GameElements.getGladiator();
        bgImg = GetComponent<Image>();
  
        //trigger.SetActive(false);


    }
    public virtual void OnPointerDown(PointerEventData ped)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            //trigger.SetActive(true);
            Debug.Log("ButtonPressed");
            player.GetComponent<GladiatorShooting>().CommandInterpreter(command);

        }
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        //trigger.SetActive(false);
        
    }
    }
