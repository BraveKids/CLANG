using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class StrategistCamera : MonoBehaviour {


    public float perspectiveZoomSpeed = 0.15f;        // The rate of change of the field of view in perspective mode.
    public float panningSpeed = 0.025f;
    Camera camera;
    float pointX;
    float pointY;
    bool onUi = false;
    bool pressing = true;
    GameObject arena;
    float minY;
    float maxY;
    public float distanceFromGround;
    public float distanceToGround;
    GameObject leftBorder;
    Plane[] planes;

    float cameraHeight;
    float cameraAngle;
    float distanceView;
    float fieldOfViewMid;

    //indicano i limiti dell'arena
    public float arenaBorderLR = 30f;
    public float arenaBorderUD = 25f;
    float arenaBorderL;
    float arenaBorderR;
    float arenaBorderU;
    float arenaBorderD;

    //distanza dall'origine della camera all'intersezione con l'arena
    float adiacente;

    //
    float cameraBorderL;
    float cameraBorderR;
    float cameraBorderU;
    float cameraBorderD;




    void Start() {
        arena = GameObject.FindGameObjectWithTag("Arena");
        camera = gameObject.GetComponent<Camera>();
        minY = arena.transform.position.y + distanceFromGround;
        maxY = minY + distanceToGround;
        //leftBorder = arena.transform.FindChild("leftBorder").gameObject;
        cameraAngle = transform.eulerAngles.x;
        distanceView = CalculateDistanceFromGround();
        fieldOfViewMid = camera.fieldOfView / 2;

        arenaBorderL = arena.transform.position.x - arenaBorderLR;
        arenaBorderR = arena.transform.position.x + arenaBorderLR;
        arenaBorderU = arena.transform.position.z + arenaBorderUD;
        arenaBorderD = arena.transform.position.z - arenaBorderUD;


    }

    void Update() {

        distanceView = CalculateDistanceFromGround();

        float cameraBorder = (distanceView / Mathf.Sin(DegreeToRadian(90 - fieldOfViewMid)));
        cameraBorderL = transform.position.x - cameraBorder;
        cameraBorderR = transform.position.x + cameraBorder;

        //we have to consider the aspect ratio
        float aspectRatio = camera.aspect;

        adiacente = distanceView * Mathf.Cos(DegreeToRadian(cameraAngle));
        cameraBorderU = transform.position.z + adiacente + (cameraBorder / aspectRatio);
        cameraBorderD = transform.position.z + adiacente - (cameraBorder / aspectRatio);
        /*
        #if UNITY_EDITOR

        arenaBorderL = arena.transform.position.x - arenaBorderLR;
        arenaBorderR = arena.transform.position.x + arenaBorderLR;
        arenaBorderU = arena.transform.position.z + arenaBorderUD;
        arenaBorderD = arena.transform.position.z - arenaBorderUD;
        minY = arena.transform.position.y + distanceFromGround;
        maxY = minY + distanceToGround;
        DebugLine();

        #endif
        */
        checkBorder();


        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                onUi = true;
                //Debug.Log("Sei sulla UI");
            }
        }

        if (!onUi) {

            if (Input.touchCount == 2) {
                pressing = true;
                Zooming();
            }
            else if (Input.touchCount == 1) {
                pressing = true;
                Panning();
            }
        }

        if (Input.touchCount == 0 && (pressing || onUi)) {
            FingersUp();
        }

    }

    void Panning() {
        if (pointX == 0 && pointY == 0) { //first touch
            pointX = Input.GetTouch(0).position.x;
            pointY = Input.GetTouch(0).position.y;
            return;
        }

        float moveX = (pointX - Input.GetTouch(0).position.x) * panningSpeed;
        float moveZ = (pointY - Input.GetTouch(0).position.y) * panningSpeed;

        if (checkLeft() && moveX <= 0) {
            //Debug.Log("You cannot pass!");
            moveX = 0;
        }
        else if (checkRight() && moveX >= 0) {
            //Debug.Log("You cannot pass!");
            moveX = 0;
        }
        if (checkUp() && moveZ > 0) {
            //Debug.Log("You cannot pass!");
            moveZ = 0;
        }
        else if (checkDown() && moveZ < 0) {
            //Debug.Log("You cannot pass!");
            moveZ = 0; ;
        }

        transform.position = new Vector3(transform.position.x + moveX, transform.position.y, transform.position.z + moveZ);

        pointX = Input.GetTouch(0).position.x;
        pointY = Input.GetTouch(0).position.y;

    }

    void Zooming() {
        pointX = 0;
        pointY = 0;

        // Store both touches.
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // Find the position in the previous frame of each touch.
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // Find the difference in the distances between each frame.
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;



        // Otherwise change the field of view based on the change in distance between the touches.
        //camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

        // Clamp the field of view to make sure it's between 0 and 180.
        //camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 0.1f, 179.9f);

        if (transform.position.y < minY && deltaMagnitudeDiff <= 0) {
            //Debug.Log("min limit");
            return;
        }
        /*
        else if (transform.position.y > maxY && deltaMagnitudeDiff > 0) {
            Debug.Log("max limit");
        }*/
        float moveX = 0;
        float moveY = 0;

        if (checkBorder() && deltaMagnitudeDiff > 0) {
            //Debug.Log("Border touched while zooming");
            if (checkLeft() && !checkRight()) {
                moveX = 1;
            }
            if (checkRight() && !checkLeft()) {
                moveX = -1;
            }
            if (checkUp() && !checkDown()) {
                moveY = -1;
            }
            if (checkDown() && !checkUp()) {
                moveY = 1;
            }
            if ((checkLeft() && checkRight()) || (checkDown() && checkUp())) {
                return;
            }
        }

        transform.Translate(new Vector3(moveX * deltaMagnitudeDiff * perspectiveZoomSpeed, moveY * deltaMagnitudeDiff * perspectiveZoomSpeed, -1 * deltaMagnitudeDiff * perspectiveZoomSpeed), transform);


    }

    void FingersUp() {
        //Debug.Log("Fingers UP");
        pressing = false;
        onUi = false;
        pointX = 0;
        pointY = 0;
    }

    float CalculateDistanceFromGround() {
        cameraHeight = transform.position.y - arena.transform.position.y;
        float result = cameraHeight / Mathf.Sin(DegreeToRadian(cameraAngle));
        return result;
    }

    private float RadianToDegree(float angle) {
        return angle * (180 / Mathf.PI);
    }

    private float DegreeToRadian(float angle) {
        return Mathf.PI * angle / 180;
    }

    void DebugLine() {
        //Debug.DrawLine(transform.position,)
        adiacente = distanceView * Mathf.Cos(DegreeToRadian(cameraAngle));
        Debug.DrawLine(transform.position, new Vector3(transform.position.x, arena.transform.position.y, transform.position.z + adiacente), Color.green, 0, false);
        Debug.DrawLine(new Vector3(arena.transform.position.x, minY, arena.transform.position.z),
            new Vector3(arena.transform.position.x, maxY, arena.transform.position.z), Color.red, 0, false);


        //Camera border
        Vector3 borderLeftPoint = new Vector3(cameraBorderL, arena.transform.position.y, transform.position.z + adiacente);
        Debug.DrawLine(new Vector3(transform.position.x, arena.transform.position.y, transform.position.z + adiacente), borderLeftPoint, Color.cyan, 0, false);
        Vector3 borderRightPoint = new Vector3(cameraBorderR, arena.transform.position.y, transform.position.z + adiacente);
        Debug.DrawLine(new Vector3(transform.position.x, arena.transform.position.y, transform.position.z + adiacente), borderRightPoint, Color.cyan, 0, false);

        Vector3 borderUpPoint = new Vector3(transform.position.x, arena.transform.position.y, cameraBorderU);
        Debug.DrawLine(new Vector3(transform.position.x, arena.transform.position.y, transform.position.z + adiacente), borderUpPoint, Color.cyan, 0, false);
        Vector3 borderDownPoint = new Vector3(transform.position.x, arena.transform.position.y, cameraBorderD);
        Debug.DrawLine(new Vector3(transform.position.x, arena.transform.position.y, transform.position.z + adiacente), borderDownPoint, Color.cyan, 0, false);



        //Arena border
        Debug.DrawLine(new Vector3(arenaBorderL, arena.transform.position.y, arena.transform.position.z - 10f), new Vector3(arenaBorderL, arena.transform.position.y, arena.transform.position.z + 10f), Color.white, 0, false);
        Debug.DrawLine(new Vector3(arenaBorderR, arena.transform.position.y, arena.transform.position.z - 10f), new Vector3(arenaBorderR, arena.transform.position.y, arena.transform.position.z + 10f), Color.white, 0, false);
        Debug.DrawLine(new Vector3(arena.transform.position.x - 10f, arena.transform.position.y, arenaBorderU), new Vector3(arena.transform.position.x + 10f, arena.transform.position.y, arenaBorderU), Color.white, 0, false);
        Debug.DrawLine(new Vector3(arena.transform.position.x - 10f, arena.transform.position.y, arenaBorderD), new Vector3(arena.transform.position.x + 10f, arena.transform.position.y, arenaBorderD), Color.white, 0, false);

    }

    bool checkBorder() {
        if (cameraBorderL < arenaBorderL) {
            //Debug.Log("Limite sinistro");
            return true;
        }
        else if (cameraBorderR > arenaBorderR) {
            //Debug.Log("Limite destro");
            return true;
        }
        else if (cameraBorderU > arenaBorderU) {
            //Debug.Log("Limite superiore");
            return true;
        }
        else if (cameraBorderD < arenaBorderD) {
            //Debug.Log("Limite inferiore");
            return true;
        }
        else return false;
    }

    bool checkLeft() {
        return cameraBorderL <= arenaBorderL;
    }

    bool checkRight() {
        return cameraBorderR >= arenaBorderR;
    }

    bool checkUp() {
        return cameraBorderU >= arenaBorderU;
    }

    bool checkDown() {
        return cameraBorderD <= arenaBorderD;
    }
}
