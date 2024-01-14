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
        transform.localRotation = q;
        Debug.Log(AndroidRotationVector.current.noisy);

    }
    Quaternion ConvertToUnity(Quaternion input)
    {
        return new Quaternion(input.x, input.z, input.y, input.w);
    }
}