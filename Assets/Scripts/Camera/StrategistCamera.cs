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
    public float distanceFromGround;
    GameObject leftBorder;
    Plane[] planes;
    public bool debugMode = false;



    //indicano i limiti dell'arena
    public float arenaBorderLR = 25f;
    public float arenaBorderUL = 15f;
    public float arenaBorderDL = 23f;
    float arenaBorderL;
    float arenaBorderR;
    float arenaBorderU;
    float arenaBorderD;

    public float maximumZoomSpeed;

    //distanza dall'origine della camera all'intersezione con l'arena
    float adiacente;

    //
    float cameraBorderL;
    float cameraBorderR;
    float cameraBorderU;
    float cameraBorderD;
    //altezza della camera
    float cameraHeight;
    //angolo di rotazione della camera, che corrisponde all'angolo formato dall'incrocio del piano con la distanzeView
    float cameraAngle;
    //distanza dall'origine della camera al punto di intersezione tra il piano e un'ipotetica retta che parte dall'origine. Fondamentalmente è il punto centrale della camera
    float distanceView;
    //apertura della camera in verticale/2
    float fieldOfViewMid;
    //angolo opposto al camera angle
    float oppositeCameraAngle;
    //angolo "di punta" della camera
    float needleAngle;
    //profondità della camera se fosse ruotata di 90 gradi, cioè visuale top view dell'arena. Utile per calcolare la larghezza della camera
    float camera90Dept;
    //angolo "di coda" della camera. Praticamente l'angolo tra il piano e
    float baseLeftAngle;
    //centro della camera sul piano
    Vector3 cameraCenterOnArena;

    float cameraHeightPlaneU;
    float cameraHeightPlaneD;

    void Start () {
        arena = GameObject.FindGameObjectWithTag("Arena");
        camera = gameObject.GetComponent<Camera>();

        //Calcolo angoli
        cameraAngle = transform.eulerAngles.x;
        fieldOfViewMid = camera.fieldOfView / 2;
        oppositeCameraAngle = 180 - cameraAngle;
        needleAngle = 180 - (oppositeCameraAngle + fieldOfViewMid);
        baseLeftAngle = 180 - (cameraAngle + fieldOfViewMid);

        minY = arena.transform.position.y + distanceFromGround;


        //calcolo tutto all'inizio, poi ogni volta che faccio zooming o panning in base alle esigenze

        ComputeAnything();





        //limiti dell'arena
        arenaBorderL = arena.transform.position.x - arenaBorderLR;
        arenaBorderR = arena.transform.position.x + arenaBorderLR;
        arenaBorderU = arena.transform.position.z + arenaBorderUL;
        arenaBorderD = arena.transform.position.z - arenaBorderDL;
        minY = arena.transform.position.y + distanceFromGround;

    }

    void Update () {

      

#if UNITY_EDITOR

        DebugLine();

#endif

        checkBorder();


        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                onUi = true;
                DebugTerm("Sei sulla UI");
            }
        }

        if (!onUi) {

            if (Input.touchCount == 2) {
                pressing = true;
                Zooming();
            } else if (Input.touchCount == 1) {
                pressing = true;
                Panning();
            }
        }

        if (Input.touchCount == 0 && (pressing || onUi)) {
            FingersUp();
        }

    }

    void Panning () {
        ComputeAnything();

        if (pointX == 0 && pointY == 0) { //first touch
            pointX = Input.GetTouch(0).position.x;
            pointY = Input.GetTouch(0).position.y;
            return;
        }

        float moveX = (pointX - Input.GetTouch(0).position.x) * panningSpeed;
        float moveZ = (pointY - Input.GetTouch(0).position.y) * panningSpeed;

        if (checkLeft() && moveX <= 0) {
            DebugTerm("You cannot pass!");
            moveX = 0;
        } else if (checkRight() && moveX >= 0) {
            DebugTerm("You cannot pass!");
            moveX = 0;
        }
        if (checkUp() && moveZ > 0) {
            DebugTerm("You cannot pass!");
            moveZ = 0;
        } else if (checkDown() && moveZ < 0) {
            DebugTerm("You cannot pass!");
            moveZ = 0; ;
        }

        transform.position = new Vector3(transform.position.x + moveX, transform.position.y, transform.position.z + moveZ);

        pointX = Input.GetTouch(0).position.x;
        pointY = Input.GetTouch(0).position.y;



    }

    void Zooming () {

        ComputeAnything();

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

        Debug.Log("Velocità di zoom: " + deltaMagnitudeDiff);
        if (deltaMagnitudeDiff < -maximumZoomSpeed) deltaMagnitudeDiff = -maximumZoomSpeed;
        if (deltaMagnitudeDiff > maximumZoomSpeed) deltaMagnitudeDiff = maximumZoomSpeed;



        if (transform.position.y < minY && deltaMagnitudeDiff <= 0) {
       
            return;
        }
       
        float moveX = 0;
        float moveY = 0;

        if (checkBorder() && deltaMagnitudeDiff > 0) {
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

    void FingersUp () {
        DebugTerm("Fingers UP");
        pressing = false;
        onUi = false;
        pointX = 0;
        pointY = 0;
    }

    void ComputeDistanceView () {
        cameraHeight = transform.position.y - arena.transform.position.y;
        float result = cameraHeight / Mathf.Sin(DegreeToRadian(cameraAngle));
        distanceView = result;
    }

    void Compute90CameraDept () {
        float cameraHeight = transform.position.y - arena.transform.position.y;
        camera90Dept = cameraHeight * Mathf.Sin(DegreeToRadian(fieldOfViewMid)) / Mathf.Sin(DegreeToRadian(cameraAngle));
    }




    private float RadianToDegree (float angle) {
        return angle * (180 / Mathf.PI);
    }

    private float DegreeToRadian (float angle) {
        return Mathf.PI * angle / 180;
    }

    void DebugLine () {
      
        adiacente = distanceView * Mathf.Cos(DegreeToRadian(cameraAngle));
        Debug.DrawLine(transform.position, new Vector3(transform.position.x, arena.transform.position.y, transform.position.z + adiacente), Color.green, 0, false);
        Debug.DrawLine(new Vector3(arena.transform.position.x, minY, arena.transform.position.z),
            new Vector3(arena.transform.position.x, minY + 5f, arena.transform.position.z), Color.red, 0, false);


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

    void ComputeAnything () {
        //Per trovare il punto centrale della camera
        ComputeDistanceView();
        adiacente = distanceView * Mathf.Cos(DegreeToRadian(cameraAngle));
        cameraCenterOnArena = new Vector3(transform.position.x, arena.transform.position.y, transform.position.z + adiacente);

        //larghezza della camera
        Compute90CameraDept();
        cameraBorderL = transform.position.x - (camera90Dept * camera.aspect);
        cameraBorderR = transform.position.x + (camera90Dept * camera.aspect);

        //profondità in avanti
        cameraHeightPlaneU = distanceView * Mathf.Sin(DegreeToRadian(fieldOfViewMid)) / Mathf.Sin(DegreeToRadian(needleAngle));
        cameraBorderU = cameraCenterOnArena.z + cameraHeightPlaneU;

        //profondità in indietro
        cameraHeightPlaneD = distanceView * Mathf.Sin(DegreeToRadian(fieldOfViewMid)) / Mathf.Sin(DegreeToRadian(baseLeftAngle));
        cameraBorderD = cameraCenterOnArena.z - cameraHeightPlaneD;
    }

    bool checkBorder () {
        if (cameraBorderL < arenaBorderL) {
            DebugTerm("Limite sinistro");
            return true;
        } else if (cameraBorderR > arenaBorderR) {
            DebugTerm("Limite destro");
            return true;
        } else if (cameraBorderU > arenaBorderU) {
            DebugTerm("Limite superiore");
            return true;
        } else if (cameraBorderD < arenaBorderD) {
            DebugTerm("Limite inferiore");
            return true;
        } else return false;
    }

    bool checkLeft () {
        return cameraBorderL <= arenaBorderL;
    }

    bool checkRight () {
        return cameraBorderR >= arenaBorderR;
    }

    bool checkUp () {
        return cameraBorderU >= arenaBorderU;
    }

    bool checkDown () {
        return cameraBorderD <= arenaBorderD;
    }

    void DebugTerm (string text) {
        if (debugMode) Debug.Log(text);
    }
}
