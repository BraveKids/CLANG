using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;


    public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {

        public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
        public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

        //CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
        //CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input
        private Image bgImg;
        private Image joystickImg;
        private Vector3 inputVector;

        private void Start()
        {
            bgImg = GetComponent<Image>();
            joystickImg = transform.GetChild(0).GetComponent<Image>();
            //CreateVirtualAxes();

        }


        public float Horizontal()
        {
            if (inputVector.x != 0)
                return inputVector.x;
            else
                return Input.GetAxis("Horizontal");
        }

        public float Vertical()
        {
            if (inputVector.z != 0)
                return inputVector.z;
            else
                return Input.GetAxis("Vertical");

        }
    /*
        void UpdateVirtualAxes(Vector3 value)
        {


            m_HorizontalVirtualAxis.Update(inputVector.x);



            m_VerticalVirtualAxis.Update(inputVector.z);

        }*/


        /*
        void CreateVirtualAxes()
        {
            // set axes to use

            m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
            m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);

        }
        */

        public virtual void OnDrag(PointerEventData ped)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos))
            {
                pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
                pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);
                inputVector = new Vector3(pos.x * 2, 0, pos.y * 2);
                inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
                joystickImg.rectTransform.anchoredPosition = new Vector3(inputVector.x * (bgImg.rectTransform.sizeDelta.x / 3), inputVector.z * (bgImg.rectTransform.sizeDelta.y / 3));

                //UpdateVirtualAxes(inputVector);
            }
        }

        public virtual void OnPointerDown(PointerEventData ped)
        {

            OnDrag(ped);

        }

        public virtual void OnPointerUp(PointerEventData ped)
        {

            inputVector = Vector3.zero;
            //UpdateVirtualAxes(inputVector);
            joystickImg.rectTransform.anchoredPosition = Vector3.zero;
            //gameObject.SetActive(false);
            bgImg.enabled = false;
            joystickImg.enabled = false;
        }


    }


