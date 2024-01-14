using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using UnityEngine.InputSystem.Android;
using UnityEngine.InputSystem;
public class PizdaRomana : MonoBehaviour
{

    Quaternion q = new Quaternion(1, 0, 0, 0);

    static float GyroMeasError = Mathf.PI * (7.0f / 180.0f);   // gyroscope measurement error in rads/s (start at 40 deg/s)
    static float GyroMeasDrift = Mathf.PI * (0.1f / 180.0f);   // gyroscope measurement drift in rad/s/s (start at 0.0 deg/s/s)
    float beta = Mathf.Sqrt(3.0f / 4.0f) * GyroMeasError;   // compute beta
    float zeta = Mathf.Sqrt(3.0f / 4.0f) * GyroMeasDrift;   // compute zeta, the other free parameter in the Madgwick scheme usually set to a small or zero value

    void MadgwickQuaternionUpdate(float ax, float ay, float az, float gx, float gy, float gz, float mx, float my, float mz)
    {

        float q1 = q.x, q2 = q.y, q3 = q.z, q4 = q.w;   // short name local variable for readability
        float norm;
        float hx, hy, _2bx, _2bz;
        float s1, s2, s3, s4;
        float qDot1, qDot2, qDot3, qDot4;

        // Auxiliary variables to avoid repeated arithmetic
        float _2q1mx;
        float _2q1my;
        float _2q1mz;
        float _2q2mx;
        float _4bx;
        float _4bz;
        float _2q1 = 2.0f * q1;
        float _2q2 = 2.0f * q2;
        float _2q3 = 2.0f * q3;
        float _2q4 = 2.0f * q4;
        float _2q1q3 = 2.0f * q1 * q3;
        float _2q3q4 = 2.0f * q3 * q4;
        float q1q1 = q1 * q1;
        float q1q2 = q1 * q2;
        float q1q3 = q1 * q3;
        float q1q4 = q1 * q4;
        float q2q2 = q2 * q2;
        float q2q3 = q2 * q3;
        float q2q4 = q2 * q4;
        float q3q3 = q3 * q3;
        float q3q4 = q3 * q4;
        float q4q4 = q4 * q4;

        // Normalise accelerometer measurement
        norm = Mathf.Sqrt(ax * ax + ay * ay + az * az);
        if (norm == 0.0f) return; // handle NaN
        norm = 1.0f / norm;
        ax *= norm;
        ay *= norm;
        az *= norm;

        // Normalise magnetometer measurement
        norm = Mathf.Sqrt(mx * mx + my * my + mz * mz);
        if (norm == 0.0f) return; // handle NaN
        norm = 1.0f / norm;
        mx *= norm;
        my *= norm;
        mz *= norm;

        // Reference direction of Earth's magnetic field
        _2q1mx = 2.0f * q1 * mx;
        _2q1my = 2.0f * q1 * my;
        _2q1mz = 2.0f * q1 * mz;
        _2q2mx = 2.0f * q2 * mx;
        hx = mx * q1q1 - _2q1my * q4 + _2q1mz * q3 + mx * q2q2 + _2q2 * my * q3 + _2q2 * mz * q4 - mx * q3q3 - mx * q4q4;
        hy = _2q1mx * q4 + my * q1q1 - _2q1mz * q2 + _2q2mx * q3 - my * q2q2 + my * q3q3 + _2q3 * mz * q4 - my * q4q4;
        _2bx = Mathf.Sqrt(hx * hx + hy * hy);
        _2bz = -_2q1mx * q3 + _2q1my * q2 + mz * q1q1 + _2q2mx * q4 - mz * q2q2 + _2q3 * my * q4 - mz * q3q3 + mz * q4q4;
        _4bx = 2.0f * _2bx;
        _4bz = 2.0f * _2bz;

        // Gradient decent algorithm corrective step
        s1 = -_2q3 * (2.0f * q2q4 - _2q1q3 - ax) + _2q2 * (2.0f * q1q2 + _2q3q4 - ay) - _2bz * q3 * (_2bx * (0.5f - q3q3 - q4q4) + _2bz * (q2q4 - q1q3) - mx) + (-_2bx * q4 + _2bz * q2) * (_2bx * (q2q3 - q1q4) + _2bz * (q1q2 + q3q4) - my) + _2bx * q3 * (_2bx * (q1q3 + q2q4) + _2bz * (0.5f - q2q2 - q3q3) - mz);
        s2 = _2q4 * (2.0f * q2q4 - _2q1q3 - ax) + _2q1 * (2.0f * q1q2 + _2q3q4 - ay) - 4.0f * q2 * (1.0f - 2.0f * q2q2 - 2.0f * q3q3 - az) + _2bz * q4 * (_2bx * (0.5f - q3q3 - q4q4) + _2bz * (q2q4 - q1q3) - mx) + (_2bx * q3 + _2bz * q1) * (_2bx * (q2q3 - q1q4) + _2bz * (q1q2 + q3q4) - my) + (_2bx * q4 - _4bz * q2) * (_2bx * (q1q3 + q2q4) + _2bz * (0.5f - q2q2 - q3q3) - mz);
        s3 = -_2q1 * (2.0f * q2q4 - _2q1q3 - ax) + _2q4 * (2.0f * q1q2 + _2q3q4 - ay) - 4.0f * q3 * (1.0f - 2.0f * q2q2 - 2.0f * q3q3 - az) + (-_4bx * q3 - _2bz * q1) * (_2bx * (0.5f - q3q3 - q4q4) + _2bz * (q2q4 - q1q3) - mx) + (_2bx * q2 + _2bz * q4) * (_2bx * (q2q3 - q1q4) + _2bz * (q1q2 + q3q4) - my) + (_2bx * q1 - _4bz * q3) * (_2bx * (q1q3 + q2q4) + _2bz * (0.5f - q2q2 - q3q3) - mz);
        s4 = _2q2 * (2.0f * q2q4 - _2q1q3 - ax) + _2q3 * (2.0f * q1q2 + _2q3q4 - ay) + (-_4bx * q4 + _2bz * q2) * (_2bx * (0.5f - q3q3 - q4q4) + _2bz * (q2q4 - q1q3) - mx) + (-_2bx * q1 + _2bz * q3) * (_2bx * (q2q3 - q1q4) + _2bz * (q1q2 + q3q4) - my) + _2bx * q2 * (_2bx * (q1q3 + q2q4) + _2bz * (0.5f - q2q2 - q3q3) - mz);
        norm = Mathf.Sqrt(s1 * s1 + s2 * s2 + s3 * s3 + s4 * s4);    // normalise step magnitude
        norm = 1.0f / norm;
        s1 *= norm;
        s2 *= norm;
        s3 *= norm;
        s4 *= norm;

        qDot1 = 0.5f * (-q2 * gx - q3 * gy - q4 * gz) - beta * s1;
        qDot2 = 0.5f * (q1 * gx + q3 * gz - q4 * gy) - beta * s2;
        qDot3 = 0.5f * (q1 * gy - q2 * gz + q4 * gx) - beta * s3;
        qDot4 = 0.5f * (q1 * gz + q2 * gy - q3 * gx) - beta * s4;
        float deltat = Time.deltaTime;
        q1 += qDot1 * deltat;
        q2 += qDot2 * deltat;
        q3 += qDot3 * deltat;
        q4 += qDot4 * deltat;
        norm = Mathf.Sqrt(q1 * q1 + q2 * q2 + q3 * q3 + q4 * q4);
        norm = 1.0f / norm;
        q.x = q1 * norm;
        q.y = q2 * norm;
        q.z = q3 * norm;
        q.w = q4 * norm;
    }


    public Quaternion attitude;
    public Vector3 acceleration;
    public Vector3 heading;
    public Vector3 RotationRate;


    public Thread _updateThread;
    void Start()
    {
        Input.gyro.enabled = true;
        Input.compass.enabled = true;
        _updateThread = new Thread(SuperFastLoop);
        _updateThread.Start();

    }

    void FixedUpdate()
    {
        //InputSystem.EnableDevice(AndroidRotationVector.current);
        //Debug.Log(q);
        //q = AndroidRotationVector.current.attitude.value;

        RotationRate = Input.gyro.rotationRateUnbiased;
        acceleration = Input.acceleration;
        heading = Input.compass.rawVector;
        MadgwickQuaternionUpdate(acceleration.x, acceleration.y, acceleration.z, RotationRate.x, RotationRate.y, RotationRate.z, heading.x, heading.y, heading.z);
        transform.rotation = q;
        Debug.Log(q);
        //Debug.Log(attitude);
        //Debug.Log(RotationRate);
        //Debug.Log(acceleration);
        //Debug.Log(heading);
        Debug.Log(Time.deltaTime);
    }


    // These are used to display how many times per second the loop is being updated
    public int RegularUpdatesPerSecond;
    public int ThreadedUpdatedsPerSecond;


    private void OnDisable()
    {
        // Stop the thread when disabled, or it will keep running in the background
        _updateThread.Abort();

        // Waits for the Thread to stop
        _updateThread.Join();
    }


    private void SuperFastLoop()
    {
        // We can't use Time.time which is a Unity API, instead we'll use this
        var time = System.DateTime.UtcNow.Ticks;
        const int oneSecond = 10000000;
        var count = 0;

        // This begins our Update loop
        while (true)
        {

            if (System.DateTime.UtcNow.Ticks - time >= oneSecond)
            {
                ThreadedUpdatedsPerSecond = count;
                count = 0;
                time = System.DateTime.UtcNow.Ticks;
            }


            RotationRate = Input.gyro.rotationRateUnbiased;
            acceleration = Input.acceleration;
            heading = Input.compass.rawVector;
            MadgwickQuaternionUpdate(acceleration.x, acceleration.y, acceleration.z, RotationRate.x, RotationRate.y, RotationRate.z, heading.x, heading.y, heading.z);
            count++;
            Debug.Log(RotationRate);
            Debug.Log(acceleration);
            Debug.Log(heading);
            // This suspends the thread for 5 milliseconds, making this code execute 200 times per second
            //Thread.Sleep(5);
        }
    }
}
