using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.PlayerLoop;

public class PinchDetection : MonoBehaviour
{

    private CameraControls controls;
    private Coroutine ZoomCoroutine;
    private Coroutine PanCoroutine;
    public CinemachineVirtualCamera VirtualCamera;
    CinemachineComposer Composer;
    CinemachineTransposer Transposer;
    public float distance = 0f;
    public GameObject FollowObj;
    bool pan = false;
    bool zoom = false;
    float zoomLevel = 1;//value 1 to 2
    float tiltLevel = 0;

    Vector2 touchstart;
    Vector2 touch1Start;
    Vector2 touch2Start;
    Vector2 touch1Prev;
    Vector2 touch2Prev;
    float StartTouchDistanceZoom;
    float TouchDistanceBetweenFingersZoom;
    float TiltDistancePrev;
    bool zoomRotateMode;
    bool tiltMode;
    bool modeSelected;
    //public bool Enabled = false;
    Vector2 prevFingerLineForAngle;
    float prevAngle;
    Vector3 currAngleVector;
    private void Awake()
    {
        controls = new CameraControls();
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    private void Start()
    {
        //ResetCamera();
        Composer = VirtualCamera.GetCinemachineComponent<CinemachineComposer>();
        Transposer = VirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        zoomLevel = Mathf.InverseLerp(80, 125, VirtualCamera.m_Lens.FieldOfView) + 1;
        controls.CameraControl.Zoom2fingerpressdetection.started += _ =>
        {
            zoom = true;
            touch1Start = controls.CameraControl.Zoom1finger.ReadValue<Vector2>();
            touch2Start = controls.CameraControl.Zoom2finger.ReadValue<Vector2>();
            touch1Prev = touch1Start;
            touch2Prev = touch2Start;
            StartTouchDistanceZoom = Vector2.Distance(touch1Start, touch2Start);
            TouchDistanceBetweenFingersZoom = StartTouchDistanceZoom;
            modeSelected = false;
            //touchstart = controls.CameraControl.Zoom2finger.ReadValue<Vector2>();
        };
        controls.CameraControl.Zoom2fingerpressdetection.canceled += _ =>
        {
            //zoom false?
            zoom = false;
            zoomRotateMode = false;
            tiltMode = false;
            modeSelected = false;
        };
        controls.CameraControl.Panpressdetection.started += _ => { pan = true; touchstart = controls.CameraControl.Pan.ReadValue<Vector2>(); };
        controls.CameraControl.Panpressdetection.canceled += _ =>
        {
            pan = false;
            zoom = false;
            zoomRotateMode = false;
            tiltMode = false;
            modeSelected = false;
        };
        controls.CameraControl.Zoom2fingerpressdetection.started += _ => {/* pan = false;*/ };
    }
    public void ResetCamera()
    {
        FollowObj.transform.eulerAngles = Vector3.zero;
        VirtualCamera.m_Lens.FieldOfView = 100;
        FollowObj.transform.position = Vector3.zero;
    }
    private void Update()
    {
        if (UI_Manager.instance.pickedbody.activeInHierarchy) return;
        //if (Input.GetMouseButtonDown(0))
        //{
        //    pan = true;
        //    touchstart = controls.CameraControl.Pan.ReadValue<Vector2>();
        //}
        //if (Input.GetMouseButtonUp(0))
        //    pan = false;

        if (pan)
        {
            if (!tiltMode)
            {
                //if (!zoom)
                //{

                Vector3 direction = touchstart - controls.CameraControl.Pan.ReadValue<Vector2>(); ;
                touchstart = controls.CameraControl.Pan.ReadValue<Vector2>();
                direction = direction * Time.deltaTime * zoomLevel;
                Vector3 moveDir = FollowObj.transform.forward * direction.y + transform.right * direction.x;
                FollowObj.transform.position += moveDir;// new Vector3(-direction.y, 0, direction.x);
                //}
                //if (zoomRotateMode)
                //{
                //    Vector2 currPos = (touch1Start + controls.CameraControl.Zoom1finger.ReadValue<Vector2>()) / 2;
                //    Vector3 direction = touch1Start - controls.CameraControl.Zoom1finger.ReadValue<Vector2>();
                //    touch1Start = controls.CameraControl.Zoom2finger.ReadValue<Vector2>();
                //    direction = direction * Time.deltaTime * zoomLevel;
                //    FollowObj.transform.position += new Vector3(-direction.y, 0, direction.x);
                //}

            }
        }


        if (Input.GetMouseButtonDown(2))
        {
            zoom = true;
            touch1Start = Vector2.zero;
            touch2Start = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
            pan = false;
        if (Input.GetMouseButtonUp(2))
            zoom = false;
        if (zoom && !modeSelected)
        {
            float distBetweenFingers = Vector2.Distance(controls.CameraControl.Zoom1finger.ReadValue<Vector2>(), controls.CameraControl.Zoom2finger.ReadValue<Vector2>());
            if (Mathf.Abs(StartTouchDistanceZoom - distBetweenFingers) > 1.75f)//eps
            {
                zoomRotateMode = true;
                modeSelected = true;
                touch2Start = controls.CameraControl.Zoom2finger.ReadValue<Vector2>();
                prevFingerLineForAngle = controls.CameraControl.Zoom1finger.ReadValue<Vector2>() - controls.CameraControl.Zoom2finger.ReadValue<Vector2>();
                prevAngle = 0;
            }
            else
            {
                float d1 = Vector2.Distance(touch1Start, controls.CameraControl.Zoom1finger.ReadValue<Vector2>());
                Vector2 travel1 = touch1Start - controls.CameraControl.Zoom1finger.ReadValue<Vector2>();
                float d2 = Vector2.Distance(touch2Start, controls.CameraControl.Zoom2finger.ReadValue<Vector2>());
                Vector2 travel2 = touch2Start - controls.CameraControl.Zoom2finger.ReadValue<Vector2>();
                float angle = Vector2.Angle(travel1, travel2);
                float generalAngle = Vector2.Angle(travel1 + travel2, Vector2.up);
                if (angle < 7 && generalAngle < 15 && Mathf.Abs(travel2.magnitude) > 3)//Mathf.Abs(d1 - d2) < 5f && Mathf.Abs(d1) > 0.5f)//eps
                {
                    tiltMode = true;
                    modeSelected = true;
                    touch1Start = new Vector2(touch1Start.x, 0);
                }
            }
        }
        if (zoomRotateMode)
        {
            //pan = false;//removebeforerelease + check
            Vector2 LineBetweenFingers = controls.CameraControl.Zoom1finger.ReadValue<Vector2>() - controls.CameraControl.Zoom2finger.ReadValue<Vector2>();
            float distBetweenFingers = LineBetweenFingers.magnitude;
            float difference = distBetweenFingers - TouchDistanceBetweenFingersZoom;
            TouchDistanceBetweenFingersZoom = distBetweenFingers;
            //touch2Prev = controls.CameraControl.Zoom2finger.ReadValue<Vector2>();
            zoomLevel -= difference * Time.deltaTime * 0.2f;

            zoomLevel = Mathf.Clamp(zoomLevel, 1, 2);
            VirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(80, 125, zoomLevel - 1);
            //Composer.m_TrackedObjectOffset.y = Mathf.Lerp(-0.5f, -10f, zoomLevel - 1);
            //rotation

            // get angular difference
            float angleNow = Mathf.Atan2(LineBetweenFingers.y, LineBetweenFingers.x) * Mathf.Rad2Deg;
            float angleThen = Mathf.Atan2(prevFingerLineForAngle.y, prevFingerLineForAngle.x) * Mathf.Rad2Deg;
            // wraparound failsafes (wraparound of Mathf.Atan2 is at +-180)
            if (angleNow > 90 && angleThen < -90)
                angleNow -= 360;
            if (angleNow < -90 && angleThen > 90)
                angleNow += 360;
            float angularDiff = angleNow - angleThen;
            float currAngle = Vector2.SignedAngle(LineBetweenFingers, prevFingerLineForAngle);
            Debug.Log(currAngle);
            float differenceAngle = angularDiff;// (prevAngle - currAngle) * Time.deltaTime * 100;
            Debug.Log(differenceAngle);
            prevAngle = currAngle;
            prevFingerLineForAngle = LineBetweenFingers;


            FollowObj.transform.eulerAngles += new Vector3(0, differenceAngle, 0);

        }
        if (tiltMode)
        {
            //float d1 = Vector2.Distance(touch1Start, controls.CameraControl.Zoom1finger.ReadValue<Vector2>());
            //float difference = d1 - TiltDistancePrev;
            //TiltDistancePrev = d1;
            //touch1Prev = touch1Start = new Vector2(controls.CameraControl.Zoom1finger.ReadValue<Vector2>().x, 0);

            float d2 = Vector2.Distance(touch2Start, controls.CameraControl.Zoom2finger.ReadValue<Vector2>());
            float difference = d2 - TiltDistancePrev;
            TiltDistancePrev = d2;
            touch2Prev = touch2Start = new Vector2(controls.CameraControl.Zoom2finger.ReadValue<Vector2>().x, 0);



            //float d2 = Vector2.Distance(touch2Prev, controls.CameraControl.Zoom2finger.ReadValue<Vector2>());
            tiltLevel -= (difference) * Time.deltaTime * 0.1f;

            tiltLevel = Mathf.Clamp(tiltLevel, 0, 1);
            Transposer.m_FollowOffset.z = Mathf.Lerp(-12, -0.05f, tiltLevel);
            Transposer.m_FollowOffset.y = Mathf.Lerp(11, 30, tiltLevel);
        }
    }

    //private void PanStart()
    //{

    //    if (VirtualCamera.m_LookAt != null)
    //        tempFollowObj = VirtualCamera.m_LookAt.gameObject;
    //    PanCoroutine = StartCoroutine(PanDetection());
    //    //Debug.Log("dssssssssssss");
    //}
    //private void PanEnd()
    //{
    //    //if (VirtualCamera.m_LookAt == null) { VirtualCamera.m_LookAt = tempFollowObj.transform; }
    //    StopCoroutine(PanCoroutine);
    //    //Debug.Log("qqqqqqqqqq");
    //}

    //IEnumerator PanDetection()
    //{
    //    VirtualCamera.m_LookAt = null;
    //    Vector2 prevScreenPos = controls.CameraControl.Pan.ReadValue<Vector2>();
    //    while (true)
    //    {

    //        Vector2 CurrScreenPOs = controls.CameraControl.Pan.ReadValue<Vector2>();
    //        //Debug.Log();
    //        //Debug.Log(CurrScreenPOs);
    //        //Debug.Log(Camera.main.ScreenToWorldPoint(CurrScreenPOs));
    //        //Vector3 PrevtouchPositionWorld = Camera.main.ScreenToWorldPoint(prevScreenPos);
    //        //Vector3 dir = PrevtouchPositionWorld - Camera.main.ScreenToWorldPoint(CurrScreenPOs);

    //        Vector2 direction = prevScreenPos - CurrScreenPOs;
    //        direction = direction * Time.deltaTime;
    //        //Debug.Log(direction);
    //        VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset += new Vector3(direction.x, 0, direction.y);
    //        prevScreenPos = CurrScreenPOs;
    //        yield return new WaitForEndOfFrame();
    //    }
    //}
    //private void ZoomStart()
    //{

    //    ZoomCoroutine = StartCoroutine(ZoomDetection());
    //}
    //private void ZoomEnd()
    //{
    //    StopCoroutine(ZoomCoroutine);
    //}

    //IEnumerator ZoomDetection()
    //{
    //    float prevDist = 0f;
    //    float dist = 0f;
    //    ////Vector2 prevPos1 = Vector2.zero;
    //    ////Vector2 prevPos2 = Vector2.zero;
    //    while (true)
    //    {
    //        ////Debug.Log(prevPos1);
    //        ////Debug.Log(prevPos2);
    //        dist = Vector2.Distance(controls.CameraControl.Zoom1finger.ReadValue<Vector2>(), controls.CameraControl.Zoom2finger.ReadValue<Vector2>());

    //        Vector2 curPos1 = controls.CameraControl.Zoom1finger.ReadValue<Vector2>();
    //        ////Vector2 curPos2 = controls.CameraControl.Zoom2finger.ReadValue<Vector2>();
    //        Debug.Log(curPos1);
    //        ////Debug.Log(curPos2);

    //        ////Vector2 deltaPos1 = prevPos1 - curPos1;
    //        ////Vector2 deltaPos2 = prevPos2 - curPos2;
    //        if (prevDist != 0)
    //            if (dist > prevDist)
    //            {
    //                VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y -= Mathf.Abs(dist - prevDist) / 10;
    //                //distance += 
    //                ////Debug.Log(deltaPos1);
    //                ////Debug.Log(deltaPos2);
    //            }
    //            else if (dist < prevDist)
    //            {
    //                VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y += Mathf.Abs(dist - prevDist) / 10;

    //                //distance -= Mathf.Abs(dist - prevDist) / 50;
    //                //
    //                ////Debug.Log(Vector2.Dot(deltaPos1, deltaPos2));

    //                ////prevPos1 = curPos1;
    //                ////prevPos2 = curPos2;


    //            }


    //        prevDist = dist;
    //        yield return new WaitForEndOfFrame();
    //    }
    //}
}
