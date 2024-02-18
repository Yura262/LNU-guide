using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.InputSystem;

public class PhoneRotation2 : MonoBehaviour
{
    void Update()
    {
        if (!AndroidRotationVector.current.enabled)
            InputSystem.EnableDevice(AndroidRotationVector.current);

        //get and apply rotation
        Quaternion q = AndroidRotationVector.current.attitude.value;
        q = ConvertToUnity(q);
        q = Quaternion.Euler(-90, 0, 0) * q;
        Vector3 v = q.eulerAngles;
        //Debug.Log(v);


        Vector3 v2 = new Vector3(-(float)v.x, -(float)v.y, (float)v.z);

        q = Quaternion.Euler(v2);

        transform.localRotation = q;
        //Debug.Log(AndroidRotationVector.current.noisy);

    }
    Quaternion ConvertToUnity(Quaternion input)
    {
        return new Quaternion(input.x, input.y, input.z, input.w);
    }
}