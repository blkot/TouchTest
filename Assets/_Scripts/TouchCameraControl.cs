using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.UIElements;
using UnityEngine.UI;


public class TouchCameraControl : MonoBehaviour
{
    public static bool isTouchOverUI = false;





    [SerializeField]
    private float floatRot = 0.4f;


    [SerializeField]
    private float floatPan = 1f;


    [SerializeField]
    private float floatScale = 5f;

    float timeInterval = 0.1f;
    //float time_base;
    float time_pass;


    bool OneFinger = false;
    bool TwoFinger = false;



    private Vector3 originalPos;
    private Quaternion originalRot;


    public Range angleRange = new Range(0, 90);
    public Range distanceRange = new Range(1, 50);


    private int tapcount;

    //around center
    public Transform target;

    private Vector2 oldPos0;
    private Vector2 oldPos1;

    private bool m_isSingleFinger;

    private Vector3 targetPan;
    private Vector3 currentPan;

    private Vector3 targetPanTarget;
    private Vector3 currentPanTarget;

    private Vector2 targetAngles;
    private Vector2 currentAngles;

    private float targetDistance;
    private float currentCameraDistance;

    private bool getCurrentDA = true;


    private bool isMoving = false;

    //[HideInInspector] public EquipmentInteract equipmentInteract;
    //Damper(阻尼) for move and rotate
    [Range(0, 10)]
    private float damper = 2;

    #region DEBUG SEGMENT

    GUIStyle guistyle = new GUIStyle();
    enum LastTouchFunction
    {
        idle,
        tap,
        hold,
        onefingerMove,
        twofingerMove,
    }
    enum ManipulatorState
    {
        Idle = 0,
        Masked,

        Pan,
        Rotate,
        Scale,
    }
    LastTouchFunction lastTouch = 0;
    ManipulatorState mState = 0;

    private int functionExecution1 = 0;
    private int functionExecution2 = 0;



    #endregion




    void Start()
    {
        mState = 0;
        ///GUI Style
        guistyle.fontSize = 25;
        ///


        originalPos = this.transform.position;
        originalRot = this.transform.rotation;


        GameObject camTargetObj = GameObject.Find("Main Camera Target");
        if (camTargetObj == null)
            camTargetObj = new GameObject("Main Camera Target");
        camTargetObj.transform.position = new Vector3(-5f, 1.5f, -5f);
        target = camTargetObj.transform;

        currentAngles = targetAngles = transform.eulerAngles;
        currentPan = targetPan = transform.position;
        currentCameraDistance = targetDistance = Vector3.Distance(transform.position, target.position);
    }





    void FixedUpdate()
    {
        //Debug.Log(time_pass);
        //if (!EventSystem.current.IsPointerOverGameObject())

        if (!isTouchOverUI)
        {

            #region touchcount <= 0 DOESNT MATTER FOR NOW.
            //mState = ManipulatorState.Idle;
            if (Input.touchCount <= 0)
            {
                getCurrentDA = true;
                return;
            }
            else
            {
                //确保获取最新的Camera状态
                if (getCurrentDA)
                {
                    getCurrentDA = false;
                    currentAngles = targetAngles = transform.eulerAngles;
                    currentCameraDistance = targetDistance = Vector3.Distance(transform.position, target.position);
                    //if (Camera.main.orthographic)
                    //{
                    //    currentPan = targetPan = transform.position;
                    //    currentDistance = targetDistance = Camera.main.orthographicSize;
                    //}
                }
            }
            #endregion

            if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                if (Input.touchCount == 1 && !OneFinger)
                {
                    //OneFinger = true;
                    OneFingerFunc();
                }


                if (Input.touchCount > 1 && !TwoFinger)
                {

                    TwoFingerFunc();

                }

                //time_pass = 0;
            }


            else
            {
                mState = ManipulatorState.Masked;
            }
        }

    }









    void OneFingerFunc()
    {
        Touch touch = Input.GetTouch(0);
        tapcount = touch.tapCount;

        //Debug.Log("start---" + time_pass);

        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            ///counter for onefingerfunc;
            functionExecution1++;

            //Debug.Log()
            if (time_pass > timeInterval)
            {
                lastTouch = LastTouchFunction.onefingerMove;
                time_pass = 0;
                isMoving = false;
            }
            else
            {

                lastTouch = LastTouchFunction.tap;
                TapRayCast();
                mState = ManipulatorState.Idle;



            }


            //time_pass = 0;
        }

        if (touch.phase == TouchPhase.Moved)
        {
            if (time_pass > timeInterval)
            {
                isMoving = true;

                mState = ManipulatorState.Rotate;
                #region Rotate. LOCKED FOR NOW. NONEED TO MODIFY UNTIL BIG CHANGE.
                //rotate
                if (!Camera.main.orthographic)
                {
                    targetAngles.y += touch.deltaPosition.x * floatRot;
                    targetAngles.x -= touch.deltaPosition.y * floatRot;

                    targetAngles.x = Mathf.Clamp(targetAngles.x, angleRange.min, angleRange.max);

                    currentAngles = Vector2.Lerp(currentAngles, targetAngles, damper * Time.deltaTime);
                    //rotate of target
                    Quaternion rotation = Quaternion.Euler(currentAngles);
                    Vector3 newPosition = target.position + rotation * Vector3.back * currentCameraDistance;
                    transform.position = newPosition;
                    transform.rotation = rotation;

                }
                #endregion
                //Debug.Log(time_pass);
                //return;
            }

            time_pass += Time.deltaTime;
            //Debug.Log("Timer++ >>>" + time_pass);

            //mState = ManipulatorState.Pan;


            /////pan func
            //Vector3 panVector2 = touch.deltaPosition * floatPan;

            //Vector3 tempScreen = Camera.main.WorldToScreenPoint(target.position);
            //tempScreen -= panVector2;

            //Vector3 tempWorld = Camera.main.ScreenToWorldPoint(tempScreen);

            //Vector3 diff = tempWorld - target.position;


            //target.position = tempWorld;
            //this.transform.position += diff;



        }


        //Debug.Log("start---" + time_pass);



        m_isSingleFinger = true;
    }

    private void TapRayCast()
    {
        RaycastHit raycasthit;
        if (!isMoving && time_pass != 0 && Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(0).position), out raycasthit, 300, 1 << 9))
        {
            //equipmentInteract.TouchEquipTarget(raycasthit);
            Debug.Log("Raycasted out----");
        }

        time_pass = 0;
        isMoving = false;
        //throw new NotImplementedException();
    }

    void TwoFingerFunc()
    {



        if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
        {
            ///counter for twofingerfunc;
            functionExecution2++;
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);


            float touchDot = Vector3.Dot(touch0.deltaPosition, touch1.deltaPosition);
            //Touch touch = Input.GetTouch(0);

            if (touchDot > 0.5f)
            {
                mState = ManipulatorState.Pan;
                ///pan func
                Vector3 panVector2 = touch0.deltaPosition * floatPan;

                Vector3 tempScreen = Camera.main.WorldToScreenPoint(target.position);
                tempScreen -= panVector2;

                Vector3 tempWorld = Camera.main.ScreenToWorldPoint(tempScreen);

                Vector3 diff = tempWorld - target.position;


                target.position = tempWorld;
                this.transform.position += diff;


                //mState = ManipulatorState.Rotate;
                ////rotate
                //if (!Camera.main.orthographic)
                //{
                //    targetAngles.y += touch.deltaPosition.x * floatRot;
                //    targetAngles.x -= touch.deltaPosition.y * floatRot;

                //    targetAngles.x = Mathf.Clamp(targetAngles.x, angleRange.min, angleRange.max);

                //    currentAngles = Vector2.Lerp(currentAngles, targetAngles, damper * Time.deltaTime);
                //    //rotate of target
                //    Quaternion rotation = Quaternion.Euler(currentAngles);
                //    Vector3 newPosition = target.position + rotation * Vector3.back * currentCameraDistance;
                //    transform.position = newPosition;
                //    transform.rotation = rotation;

                //}
            }
            else
            {
                mState = ManipulatorState.Scale;
                //scale
                float currentTouchDistance = Vector3.Distance(touch0.position, touch1.position);
                float lastTouchDistance = Vector3.Distance(touch0.position - touch0.deltaPosition, touch1.position - touch1.deltaPosition);

                targetDistance -= (currentTouchDistance - lastTouchDistance) * Time.deltaTime * floatScale;


                targetDistance = Mathf.Clamp(targetDistance, distanceRange.min, distanceRange.max);
                currentCameraDistance = Mathf.Lerp(currentCameraDistance, targetDistance, damper * Time.deltaTime);

                if (Camera.main.orthographic)
                    Camera.main.orthographicSize = currentCameraDistance;
                else
                    transform.position = target.position - transform.forward * currentCameraDistance;



            }




            m_isSingleFinger = false;








        }
        if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended)
        {
            mState = ManipulatorState.Idle;
            lastTouch = LastTouchFunction.twofingerMove;
        }


    }

    //Reset to 0,0,0
    public void Recenter()
    {
        this.transform.position = originalPos;
        this.transform.rotation = originalRot;
        //this.transform.localScale = originalCamTransform.localScale;

        target.position = Vector3.zero;
    }
    void OnGUI()
    {

        //GUI.Label(new Rect(50, 120, 400, 30), "ManipulatorState = " + mState);
        GUI.Label(new Rect(50, 80, 400, 30), "Time-Pass :" + time_pass, guistyle);
        GUI.Label(new Rect(50, 120, 400, 30), "Manipulation Phase :" + mState, guistyle);
        GUI.Label(new Rect(50, 160, 400, 30), "LastTouchFunction :" + lastTouch, guistyle);

        GUI.Label(new Rect(50, 600, 400, 30), "OneFingerExecutions :" + functionExecution1, guistyle);
        GUI.Label(new Rect(50, 640, 400, 30), "TwoFingerExecutions :" + functionExecution2, guistyle);
        if (Input.touchCount == 1)
        {
            Touch touch0 = Input.GetTouch(0);
            GUI.Label(new Rect(50, 200, 400, 30), "EventIsPointerOverUI: " + EventSystem.current.IsPointerOverGameObject(touch0.fingerId), guistyle);
            GUI.Label(new Rect(50, 240, 400, 30), "EventIsPointerOverUI Selecting: " + EventSystem.current.currentSelectedGameObject, guistyle);
            GUI.Label(new Rect(50, 280, 400, 30), "Touch FingerId " + touch0.fingerId, guistyle);
            GUI.Label(new Rect(50, 320, 400, 30), "Touch Position : " + touch0.position, guistyle);
            GUI.Label(new Rect(50, 360, 400, 30), "Touch Phase " + touch0.phase, guistyle);

            //vague finger2
            GUI.Label(new Rect(50, 440, 400, 30), "Touch FingerId " + " NULL ", guistyle);
            GUI.Label(new Rect(50, 480, 400, 30), "Touch Position : " + " NULL ", guistyle);
            GUI.Label(new Rect(50, 520, 400, 30), "Touch Phase " + " NULL ", guistyle);
        }

        if (Input.touchCount > 1)
        {
            Touch touch0 = Input.GetTouch(0);
            GUI.Label(new Rect(50, 200, 400, 30), "EventIsPointerOverUI: " + EventSystem.current.IsPointerOverGameObject(touch0.fingerId), guistyle);
            GUI.Label(new Rect(50, 240, 400, 30), "EventIsPointerOverUI Selecting: " + EventSystem.current.currentSelectedGameObject, guistyle);
            GUI.Label(new Rect(50, 280, 400, 30), "Touch FingerId " + touch0.fingerId, guistyle);
            GUI.Label(new Rect(50, 320, 400, 30), "Touch Position : " + touch0.position, guistyle);
            GUI.Label(new Rect(50, 360, 400, 30), "Touch Phase " + touch0.phase, guistyle);

            Touch touch1 = Input.GetTouch(1);
            GUI.Label(new Rect(50, 440, 400, 30), "Touch FingerId " + touch1.fingerId, guistyle);
            GUI.Label(new Rect(50, 480, 400, 30), "Touch Position : " + touch1.position, guistyle);
            GUI.Label(new Rect(50, 520, 400, 30), "Touch Phase " + touch1.phase, guistyle);

        }



        //Touch touch1 = Input.GetTouch(1);



        //GUI.Label(new Rect(50, 160, 400, 30), "----touchinfo: " + touch0.phase);
        //GUI.Label(new Rect(50, 200, 400, 30), "----touchinfo: " + touch1.phase);
        //GUI.Label(new Rect(50, 240, 400, 30), "POS0 = " + touch0.position);
        //GUI.Label(new Rect(50, 280, 400, 30), "POS1 = " + touch1.position);
        //GUI.Label(new Rect(50, 320, 400, 30), "DISTANCE = " + Vector2.Distance(touch0.position, touch1.position));



    }
}





public struct Range
{
    public float min;
    public float max;

    public Range(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}