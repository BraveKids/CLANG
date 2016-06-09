using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
public class JoystickController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Image joystick;
    private Image bgImg;

    private void Start()
    {
        bgImg = GetComponent<Image>();
        joystick = transform.GetChild(0).GetComponent<Image>();
    }
    public virtual void OnPointerDown(PointerEventData ped)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            joystick.gameObject.GetComponent<Image>().enabled = true;
            joystick.gameObject.transform.GetChild(0).GetComponent<Image>().enabled = true;
            joystick.gameObject.SetActive(true);
            joystick.rectTransform.position = ped.position;
            
        }

    }

    public virtual void OnDrag(PointerEventData ped)
    {
        joystick.GetComponent<VirtualJoystick>().OnDrag(ped);
    }


    public virtual void OnPointerUp(PointerEventData ped)
    {

        joystick.GetComponent<VirtualJoystick>().OnPointerUp(ped);
    }

}
