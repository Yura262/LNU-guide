using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using UnityEngine.InputSystem.Android;
using UnityEngine.InputSystem;
using System;
using JetBrains.Annotations;

public class InertialNavigation : MonoBehaviour
{
    public static event Action<InputDevice, InputDeviceChange> onDeviceChange;
    public Thread _updateThread;
    public Vector3 v;
    Rigidbody rb;
    Vector3 aAverage = Vector3.zero;
    int numberOfValuesToAverage = 3;
    void Start()
    {

        //_updateThread = new Thread(SuperFastLoop);
        //_updateThread.Start();
        Input.compass.enabled = true;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    HighPassFilterVector hPF = new HighPassFilterVector(50f, 50);
    private void FixedUpdate()
    {
        if (!AndroidRotationVector.current.enabled)
            InputSystem.EnableDevice(AndroidRotationVector.current);
        if (!AndroidLinearAccelerationSensor.current.enabled)
            InputSystem.EnableDevice(AndroidLinearAccelerationSensor.current);
        //Debug.Log(AndroidLinearAccelerationSensor.current.acceleration.value);

        //get and apply rotation
        Quaternion q = AndroidRotationVector.current.attitude.value;
        q = ConvertToUnity(q);
        transform.localRotation = q;

        //////convert sensor coordinates to global
        Vector3 aInput = ConvertToUnity(AndroidLinearAccelerationSensor.current.acceleration.ReadValue());
        Quaternion aInputq = new Quaternion(0, aInput.x, aInput.y, aInput.z);
        Quaternion aGlobalq = q * aInputq;//* reverseQuaternion(q);
        Vector3 aGlobal = new Vector3(aGlobalq.y, aGlobalq.z, aGlobalq.w);
        Debug.Log(aGlobal);
        Debug.Log(ConvertToUnity(Input.acceleration));
        ////kind of moving average thing
        aAverage -= aAverage / numberOfValuesToAverage;
        aAverage += aGlobal / numberOfValuesToAverage;
        Debug.Log(aAverage);

        ////something that looks like high-pass filter, idk
        Vector3 aProcessed = hPF.update(aGlobal);
        Debug.Log(aProcessed);

        //////apply accelereation 
        v = rb.velocity;
        v += aProcessed * Time.deltaTime;
        rb.velocity = v;


    }

    class HighPassFilter
    {
        float tau;
        float alpha;
        float prev_output;
        float prev_input;
        public HighPassFilter(float cutoff_freq, int sampling_rate)
        {
            tau = 1 / (2 * MathF.PI * cutoff_freq);
            alpha = tau / (tau + 1 / sampling_rate);
            prev_output = 0.0f;
            prev_input = 0.0f;
        }
        public float update(float new_input)
        {
            float output = alpha * (prev_output + new_input - prev_input);
            prev_input = new_input;
            prev_output = output;
            return output;
        }

    }
    class HighPassFilterVector
    {
        HighPassFilter x;
        HighPassFilter y;
        HighPassFilter z;

        public HighPassFilterVector(float cutoff_freq, int sampling_rate)
        {
            x = new HighPassFilter(cutoff_freq, sampling_rate);
            y = new HighPassFilter(cutoff_freq, sampling_rate);
            z = new HighPassFilter(cutoff_freq, sampling_rate);
        }
        public Vector3 update(Vector3 new_input)
        {
            float x_ = x.update(new_input.x);
            float y_ = y.update(new_input.y);
            float z_ = z.update(new_input.z);

            return new Vector3(x_, y_, z_);
        }
    }
    Vector3 ConvertToUnity(Vector3 inp)
    {
        return new Vector3(inp.x, inp.z, inp.y);
    }
    Quaternion ConvertToUnity(Quaternion input)
    {
        return new Quaternion(input.x, input.z, input.y, input.w);
    }
    Quaternion reverseQuaternion(Quaternion input)
    {
        return new Quaternion(input.x, -input.y, -input.z, -input.w);
    }

    //// These are used to display how many times per second the loop is being updated
    //public int RegularUpdatesPerSecond;
    //public int ThreadedUpdatedsPerSecond;


    //private void OnDisable()
    //{
    //    // Stop the thread when disabled, or it will keep running in the background
    //    _updateThread.Abort();

    //    // Waits for the Thread to stop
    //    _updateThread.Join();
    //}


    //private void SuperFastLoop()
    //{
    //    // We can't use Time.time which is a Unity API, instead we'll use this
    //    var time = System.DateTime.UtcNow.Ticks;
    //    const int oneSecond = 10000000;
    //    var count = 0;

    //    // This begins our Update loop
    //    while (true)
    //    {

    //        if (System.DateTime.UtcNow.Ticks - time >= oneSecond)
    //        {
    //            ThreadedUpdatedsPerSecond = count;
    //            count = 0;
    //            time = System.DateTime.UtcNow.Ticks;
    //        }



    //        // This suspends the thread for 5 milliseconds, making this code execute 200 times per second
    //        //Thread.Sleep(5);
    //    }
    //}


}
