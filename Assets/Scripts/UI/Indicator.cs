using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour {
    public Camera indicatorCamera;
    public Canvas canvas;
    public Image offScreenSpritePrefab;

    public bool displayOffscreen = true;

    private Image offSprite;
    private Vector3 offScreen = new Vector3(-100, -100, 0);
    Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) * .5f;

    private Rect centerRect;
    // Use this for initialization
    void Start () {
        GameObject cameraObj = GameObject.FindGameObjectWithTag("GladiatorCamera");
        if (cameraObj)
            indicatorCamera = cameraObj.GetComponent<Camera>();

        canvas = GameElements.getGladiatorCanvas().GetComponent<Canvas>();
        offSprite = Instantiate(offScreenSpritePrefab, offScreen, Quaternion.Euler(new Vector3(0, 0, 0))) as Image;
        offSprite.rectTransform.SetParent(canvas.transform, false);
        centerRect.width = Screen.width / 2;
        centerRect.height = Screen.height / 2;
        centerRect.position = new Vector2(screenCenter.x - centerRect.width / 2, screenCenter.y - centerRect.height / 2);
    }

    // Update is called once per frame
    void Update () {
        if (indicatorCamera == null) {
            this.enabled = false;
            OnDisable();
            return;
        }

        PlaceIndicators();
    }


    void PlaceIndicators () {
        Vector3 screenpos = indicatorCamera.WorldToScreenPoint(this.transform.position);

        //if onscreen
        if (screenpos.z > 0 && screenpos.x < Screen.width && screenpos.x > 0 && screenpos.y < Screen.height && screenpos.y > 0) {
            offSprite.rectTransform.position = offScreen;//get rid of the arrow indicator            
        } else {
            if (displayOffscreen) {
                PlaceOffscreen(screenpos);
            }
        }
    }

    void PlaceOffscreen (Vector3 screenpos) {
        float x = screenpos.x;
        float y = screenpos.y;
        float offset = 20;
        float angle = 0;
        //Color c = Color.magenta;

        if (screenpos.z < 0) {
            screenpos = -screenpos;
        }

        if (screenpos.x > Screen.width) //right
        {
            //c = Color.green;
            angle = -90;
            x = Screen.width - offset;
        }
        if (screenpos.x < 0) {
            //c = Color.blue;
            angle = 90;
            x = offset;
        }

        if (screenpos.y > Screen.height) {
            angle = -180;
            y = Screen.height - offset;
        }
        if (screenpos.y < 0) {
            angle = 180;
            y = offset;
        }

        Vector3 offscreenPos = new Vector3(x, y, 0);
        offSprite.rectTransform.position = offscreenPos;

        offSprite.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        //offSprite.color = c;
    }

    void OnDisable () {
        //OnDisable called
        if (offSprite) {
            Destroy(offSprite);
        }
    }
}

