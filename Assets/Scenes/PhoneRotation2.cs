using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneRotation2 : MonoBehaviour
{
    //float Time.deltaTime = 0.02f; // sampling period in seconds (shown as 1 ms)
    //static float gyroMeasError = 3.14159265358979f * (5.0f / 180.0f); // gyroscope measurement error in rad/s (shown as 5 deg/s)
    //static float gyroMeasDrift = 3.14159265358979f * (0.2f / 180.0f); // gyroscope measurement error in rad/s/s (shown as 0.2f deg/s/s)
    //float beta = Mathf.Sqrt(3.0f / 4.0f) * gyroMeasError; // compute beta
    //float zeta = Mathf.Sqrt(3.0f / 4.0f) * gyroMeasDrift; // compute zeta
    //                                                      // Global system variables

    //Vector3 w; // accelerometer measurements
    //Vector3 a; // gyroscope measurements in rad/s
    //Vector3 m; // magnetometer measurements
    //Quaternion SEq = new Quaternion(1, 0, 0, 0);// estimated orientation quaternion elements with initial conditions
    //float b_x = 1, b_z = 0; // reference direction of flux in earth frame

    //Vector3 w_b = Vector3.zero; // estimate gyroscope biases error
    //                            // Function to compute one filter iteration
    //private void filterUpdate(Vector3 w, Vector3 a, Vector3 m)
    //{
    //    // local system variables
    //    //float norm; // vector norm
    //    Quaternion SEqDot_omega;// quaternion rate from gyroscopes elements
    //    float f_1, f_2, f_3, f_4, f_5, f_6; // objective function elements
    //    float J_11or24, J_12or23, J_13or22, J_14or21, J_32, J_33, // objective function Jacobian elements
    //    J_41, J_42, J_43, J_44, J_51, J_52, J_53, J_54, J_61, J_62, J_63, J_64; //
    //    Quaternion SEqHatDot;// estimated direction of the gyroscope error
    //    Vector3 w_err; // estimated direction of the gyroscope error (angular)
    //    Vector3 h;// computed flux in the earth frame

    //    // axulirary variables to avoid reapeated calcualtions
    //    float halfSEq_1 = 0.5f * SEq.x;
    //    float halfSEq_2 = 0.5f * SEq.y;
    //    float halfSEq_3 = 0.5f * SEq.z;
    //    float halfSEq_4 = 0.5f * SEq.w;
    //    float twoSEq_1 = 2.0f * SEq.x;
    //    float twoSEq_2 = 2.0f * SEq.y;
    //    float twoSEq_3 = 2.0f * SEq.z;
    //    float twoSEq_4 = 2.0f * SEq.w;
    //    float twob_x = 2.0f * b_x;
    //    float twob_z = 2.0f * b_z;
    //    float twob_xSEq_1 = 2.0f * b_x * SEq.x;
    //    float twob_xSEq_2 = 2.0f * b_x * SEq.y;
    //    float twob_xSEq_3 = 2.0f * b_x * SEq.z;
    //    float twob_xSEq_4 = 2.0f * b_x * SEq.w;
    //    float twob_zSEq_1 = 2.0f * b_z * SEq.x;
    //    float twob_zSEq_2 = 2.0f * b_z * SEq.y;
    //    float twob_zSEq_3 = 2.0f * b_z * SEq.z;
    //    float twob_zSEq_4 = 2.0f * b_z * SEq.w;
    //    float SEq_1SEq_2;
    //    float SEq_1SEq_3 = SEq.x * SEq.z;
    //    float SEq_1SEq_4;
    //    float SEq_2SEq_3;
    //    float SEq_2SEq_4 = SEq.y * SEq.w;
    //    float SEq_3SEq_4;
    //    float twom_x = 2.0f * m.x;
    //    float twom_y = 2.0f * m.y;
    //    float twom_z = 2.0f * m.z;
    //    // normalise the accelerometer measurement
    //    a.Normalize();
    //    // normalise the magnetometer measurement
    //    m.Normalize();
    //    // compute the objective function and Jacobian
    //    f_1 = twoSEq_2 * SEq.w - twoSEq_1 * SEq.z - a.x;
    //    f_2 = twoSEq_1 * SEq.y + twoSEq_3 * SEq.w - a.y;
    //    f_3 = 1.0f - twoSEq_2 * SEq.y - twoSEq_3 * SEq.z - a.z;
    //    f_4 = twob_x * (0.5f - SEq.z * SEq.z - SEq.w * SEq.w) + twob_z * (SEq_2SEq_4 - SEq_1SEq_3) - m.x;
    //    f_5 = twob_x * (SEq.y * SEq.z - SEq.x * SEq.w) + twob_z * (SEq.x * SEq.y + SEq.z * SEq.w) - m.y;
    //    f_6 = twob_x * (SEq_1SEq_3 + SEq_2SEq_4) + twob_z * (0.5f - SEq.y * SEq.y - SEq.z * SEq.z) - m.z;
    //    J_11or24 = twoSEq_3; // J_11 negated in matrix multiplication
    //    J_12or23 = 2.0f * SEq.w;
    //    J_13or22 = twoSEq_1; // J_12 negated in matrix multiplication
    //    J_14or21 = twoSEq_2;
    //    J_32 = 2.0f * J_14or21; // negated in matrix multiplication
    //    J_33 = 2.0f * J_11or24; // negated in matrix multiplication
    //    J_41 = twob_zSEq_3; // negated in matrix multiplication
    //    J_42 = twob_zSEq_4;
    //    J_43 = 2.0f * twob_xSEq_3 + twob_zSEq_1; // negated in matrix multiplication
    //    J_44 = 2.0f * twob_xSEq_4 - twob_zSEq_2; // negated in matrix multiplication
    //    J_51 = twob_xSEq_4 - twob_zSEq_2; // negated in matrix multiplication
    //    J_52 = twob_xSEq_3 + twob_zSEq_1;
    //    J_53 = twob_xSEq_2 + twob_zSEq_4;
    //    J_54 = twob_xSEq_1 - twob_zSEq_3; // negated in matrix multiplication
    //    J_61 = twob_xSEq_3;
    //    J_62 = twob_xSEq_4 - 2.0f * twob_zSEq_2;
    //    J_63 = twob_xSEq_1 - 2.0f * twob_zSEq_3;
    //    J_64 = twob_xSEq_2;
    //    // compute the gradient (matrix multiplication)
    //    SEqHatDot.x = J_14or21 * f_2 - J_11or24 * f_1 - J_41 * f_4 - J_51 * f_5 + J_61 * f_6;
    //    SEqHatDot.y = J_12or23 * f_1 + J_13or22 * f_2 - J_32 * f_3 + J_42 * f_4 + J_52 * f_5 + J_62 * f_6;
    //    SEqHatDot.z = J_12or23 * f_2 - J_33 * f_3 - J_13or22 * f_1 - J_43 * f_4 + J_53 * f_5 + J_63 * f_6;
    //    SEqHatDot.w = J_14or21 * f_1 + J_11or24 * f_2 - J_44 * f_4 - J_54 * f_5 + J_64 * f_6;
    //    // normalise the gradient to estimate direction of the gyroscope error
    //    SEqHatDot.Normalize();
    //    // compute angular estimated direction of the gyroscope error
    //    w_err.x = twoSEq_1 * SEqHatDot.y - twoSEq_2 * SEqHatDot.x - twoSEq_3 * SEqHatDot.w + twoSEq_4 * SEqHatDot.z;
    //    w_err.y = twoSEq_1 * SEqHatDot.z + twoSEq_2 * SEqHatDot.w - twoSEq_3 * SEqHatDot.x - twoSEq_4 * SEqHatDot.y;
    //    w_err.z = twoSEq_1 * SEqHatDot.w - twoSEq_2 * SEqHatDot.z + twoSEq_3 * SEqHatDot.y - twoSEq_4 * SEqHatDot.x;
    //    // compute and remove the gyroscope baises
    //    w_b.x += w_err.x * Time.deltaTime * zeta;
    //    w_b.y += w_err.y * Time.deltaTime * zeta;
    //    w_b.z += w_err.z * Time.deltaTime * zeta;
    //    w.x -= w_b.x;
    //    w.y -= w_b.y;
    //    w.z -= w_b.z;
    //    // compute the quaternion rate measured by gyroscopes
    //    SEqDot_omega.x = -halfSEq_2 * w.x - halfSEq_3 * w.y - halfSEq_4 * w.z;
    //    SEqDot_omega.y = halfSEq_1 * w.x + halfSEq_3 * w.z - halfSEq_4 * w.y;
    //    SEqDot_omega.z = halfSEq_1 * w.y - halfSEq_2 * w.z + halfSEq_4 * w.x;
    //    SEqDot_omega.w = halfSEq_1 * w.z + halfSEq_2 * w.y - halfSEq_3 * w.x;
    //    // compute then integrate the estimated quaternion rate
    //    SEq.x += (SEqDot_omega.x - (beta * SEqHatDot.x)) * Time.deltaTime;
    //    SEq.y += (SEqDot_omega.y - (beta * SEqHatDot.y)) * Time.deltaTime;
    //    SEq.z += (SEqDot_omega.z - (beta * SEqHatDot.z)) * Time.deltaTime;
    //    SEq.w += (SEqDot_omega.w - (beta * SEqHatDot.w)) * Time.deltaTime;
    //    // normalise quaternion
    //    SEq.Normalize();
    //    // compute flux in the earth frame
    //    SEq_1SEq_2 = SEq.x * SEq.y; // recompute axulirary variables
    //    SEq_1SEq_3 = SEq.x * SEq.z;
    //    SEq_1SEq_4 = SEq.x * SEq.w;
    //    SEq_3SEq_4 = SEq.z * SEq.w;
    //    SEq_2SEq_3 = SEq.y * SEq.z;
    //    SEq_2SEq_4 = SEq.y * SEq.w;
    //    h.x = twom_x * (0.5f - SEq.z * SEq.z - SEq.w * SEq.w) + twom_y * (SEq_2SEq_3 - SEq_1SEq_4) + twom_z * (SEq_2SEq_4 + SEq_1SEq_3);
    //    h.y = twom_x * (SEq_2SEq_3 + SEq_1SEq_4) + twom_y * (0.5f - SEq.y * SEq.y - SEq.w * SEq.w) + twom_z * (SEq_3SEq_4 - SEq_1SEq_2);
    //    h.z = twom_x * (SEq_2SEq_4 - SEq_1SEq_3) + twom_y * (SEq_3SEq_4 + SEq_1SEq_2) + twom_z * (0.5f - SEq.y * SEq.y - SEq.z * SEq.z);
    //    // normalise the flux vector to have only components in the x and z
    //    b_x = Mathf.Sqrt((h.x * h.x) + (h.y * h.y));
    //    b_z = h.z;
    //}
    Quaternion q = new Quaternion(1, 0, 0, 0);

    static float GyroMeasError = Mathf.PI * (10.0f / 180.0f);   // gyroscope measurement error in rads/s (start at 40 deg/s)
    static float GyroMeasDrift = Mathf.PI * (0.0f / 180.0f);   // gyroscope measurement drift in rad/s/s (start at 0.0 deg/s/s)
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
    public Quaternion EstimateOfOrientation_q_est = new Quaternion(1, 0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;
        Input.compass.enabled = true;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(Time.deltaTime);
        transform.rotation = q;
        attitude = Input.gyro.attitude;
        RotationRate = Input.gyro.rotationRateUnbiased;
        acceleration = Input.acceleration;
        heading = Input.compass.rawVector;
        //filterUpdate(RotationRate, acceleration, heading);
        MadgwickQuaternionUpdate(acceleration.x, acceleration.y, acceleration.z, RotationRate.x, RotationRate.y, RotationRate.z, heading.x, heading.y, heading.z);
        Debug.Log(q);
        //Debug.Log(attitude);
        //Debug.Log(RotationRate);
        //Debug.Log(acceleration);
        //Debug.Log(heading);
        Debug.Log(Time.deltaTime);
    }


}
