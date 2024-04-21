using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

public class PinchDetection : MonoBehaviour
{

    private CameraControls controls;
    private Coroutine ZoomCoroutine;
    private Coroutine PanCoroutine;
    public CinemachineVirtualCamera VirtualCamera;
    public float distance = 0f;
    private GameObject tempFollowObj;
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
        controls.CameraControl.Zoom2fingerpressdetection.started += _ => ZoomStart();
        controls.CameraControl.Zoom2fingerpressdetection.canceled += _ => ZoomEnd();
        controls.CameraControl.Panpressdetection.started += _ => PanStart();
        controls.CameraControl.Panpressdetection.canceled += _ => PanEnd();
        controls.CameraControl.Zoom2fingerpressdetection.started += _ => PanEnd();

    }
    //Vector3 touchstart;
    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        touchstart = Input.mousePosition;

    //    }
    //    if (Input.GetMouseButton(0))
    //    {

    //        Vector3 direction = touchstart - Input.mousePosition;
    //        direction = direction * Time.deltaTime;
    //        VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset += new Vector3(direction.x, 0, direction.y);
    //    }
    //}

    private void PanStart()
    {
        if (VirtualCamera.m_LookAt != null)
            tempFollowObj = VirtualCamera.m_LookAt.gameObject;
        PanCoroutine = StartCoroutine(PanDetection());
        //Debug.Log("dssssssssssss");
    }
    private void PanEnd()
    {
        //if (VirtualCamera.m_LookAt == null) { VirtualCamera.m_LookAt = tempFollowObj.transform; }
        StopCoroutine(PanCoroutine);
        //Debug.Log("qqqqqqqqqq");
    }

    IEnumerator PanDetection()
    {
        VirtualCamera.m_LookAt = null;
        Vector2 prevScreenPos = controls.CameraControl.Pan.ReadValue<Vector2>();
        while (true)
        {

            Vector2 CurrScreenPOs = controls.CameraControl.Pan.ReadValue<Vector2>();
            //Debug.Log();
            //Debug.Log(CurrScreenPOs);
            //Debug.Log(Camera.main.ScreenToWorldPoint(CurrScreenPOs));
            //Vector3 PrevtouchPositionWorld = Camera.main.ScreenToWorldPoint(prevScreenPos);
            //Vector3 dir = PrevtouchPositionWorld - Camera.main.ScreenToWorldPoint(CurrScreenPOs);

            Vector2 direction = prevScreenPos - CurrScreenPOs;
            direction = direction * Time.deltaTime;
            //Debug.Log(direction);
            VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset += new Vector3(direction.x, 0, direction.y);
            prevScreenPos = CurrScreenPOs;
            yield return new WaitForEndOfFrame();
        }
    }
    private void ZoomStart()
    {

        ZoomCoroutine = StartCoroutine(ZoomDetection());
    }
    private void ZoomEnd()
    {
        StopCoroutine(ZoomCoroutine);
    }


    IEnumerator ZoomDetection()
    {
        float prevDist = 0f;
        float dist = 0f;
        ////Vector2 prevPos1 = Vector2.zero;
        ////Vector2 prevPos2 = Vector2.zero;
        while (true)
        {
            ////Debug.Log(prevPos1);
            ////Debug.Log(prevPos2);
            dist = Vector2.Distance(controls.CameraControl.Zoom1finger.ReadValue<Vector2>(), controls.CameraControl.Zoom2finger.ReadValue<Vector2>());

            ////Vector2 curPos1 = controls.CameraControl.Zoom1finger.ReadValue<Vector2>();
            ////Vector2 curPos2 = controls.CameraControl.Zoom2finger.ReadValue<Vector2>();
            ////Debug.Log(curPos1);
            ////Debug.Log(curPos2);

            ////Vector2 deltaPos1 = prevPos1 - curPos1;
            ////Vector2 deltaPos2 = prevPos2 - curPos2;
            if (prevDist != 0)
                if (dist > prevDist)
                {
                    VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y -= Mathf.Abs(dist - prevDist) / 10;
                    //distance += 
                    ////Debug.Log(deltaPos1);
                    ////Debug.Log(deltaPos2);
                }
                else if (dist < prevDist)
                {
                    VirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y += Mathf.Abs(dist - prevDist) / 10;

                    //distance -= Mathf.Abs(dist - prevDist) / 50;
                    //
                    ////Debug.Log(Vector2.Dot(deltaPos1, deltaPos2));

                    ////prevPos1 = curPos1;
                    ////prevPos2 = curPos2;


                }


            prevDist = dist;
            yield return new WaitForEndOfFrame();
        }
    }
}
