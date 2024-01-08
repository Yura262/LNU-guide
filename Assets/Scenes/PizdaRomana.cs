using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PizdaRomana : MonoBehaviour
{
    // Start is called before the first frame update

    // System constants
    float deltat = 0.001f; // sampling period in seconds (shown as 1 ms)
    static float gyroMeasError = 3.14159265358979f * (5.0f / 180.0f); // gyroscope measurement error in rad/s (shown as 5 deg/s)
    float beta = Mathf.Sqrt(3.0f / 4.0f) * gyroMeasError; // compute beta
                                                          // Global system variables
    float a_x, a_y, a_z; // accelerometer measurements
    float w_x, w_y, w_z; // gyroscope measurements in rad/s
    float SEq_1 = 1.0f, SEq_2 = 0.0f, SEq_3 = 0.0f, SEq_4 = 0.0f; // estimated orientation quaternion elements with initial conditions
    Quaternion filterUpdate(float w_x, float w_y, float w_z, float a_x, float a_y, float a_z)
    {
        // Local system variables
        float norm; // vector norm
        float SEqDot_omega_1, SEqDot_omega_2, SEqDot_omega_3, SEqDot_omega_4; // quaternion derrivative from gyroscopes elements
        float f_1, f_2, f_3; // objective function elements
        float J_11or24, J_12or23, J_13or22, J_14or21, J_32, J_33; // objective function Jacobian elements
        float SEqHatDot_1, SEqHatDot_2, SEqHatDot_3, SEqHatDot_4; // estimated direction of the gyroscope error
                                                                  // Axulirary variables to avoid reapeated calcualtions
        float halfSEq_1 = 0.5f * SEq_1;
        float halfSEq_2 = 0.5f * SEq_2;
        float halfSEq_3 = 0.5f * SEq_3;
        float halfSEq_4 = 0.5f * SEq_4;
        float twoSEq_1 = 2.0f * SEq_1;
        float twoSEq_2 = 2.0f * SEq_2;
        float twoSEq_3 = 2.0f * SEq_3;

        // Normalise the accelerometer measurement
        norm = Mathf.Sqrt(a_x * a_x + a_y * a_y + a_z * a_z);
        a_x /= norm;
        a_y /= norm;
        a_z /= norm;
        // Compute the objective function and Jacobian
        f_1 = twoSEq_2 * SEq_4 - twoSEq_1 * SEq_3 - a_x;
        f_2 = twoSEq_1 * SEq_2 + twoSEq_3 * SEq_4 - a_y;
        f_3 = 1.0f - twoSEq_2 * SEq_2 - twoSEq_3 * SEq_3 - a_z;
        J_11or24 = twoSEq_3; // J_11 negated in matrix multiplication
        J_12or23 = 2.0f * SEq_4;
        J_13or22 = twoSEq_1; // J_12 negated in matrix multiplication
        J_14or21 = twoSEq_2;
        J_32 = 2.0f * J_14or21; // negated in matrix multiplication
        J_33 = 2.0f * J_11or24; // negated in matrix multiplication
                                // Compute the gradient (matrix multiplication)
        SEqHatDot_1 = J_14or21 * f_2 - J_11or24 * f_1;
        SEqHatDot_2 = J_12or23 * f_1 + J_13or22 * f_2 - J_32 * f_3;
        SEqHatDot_3 = J_12or23 * f_2 - J_33 * f_3 - J_13or22 * f_1;
        SEqHatDot_4 = J_14or21 * f_1 + J_11or24 * f_2;
        // Normalise the gradient
        norm = Mathf.Sqrt(SEqHatDot_1 * SEqHatDot_1 + SEqHatDot_2 * SEqHatDot_2 + SEqHatDot_3 * SEqHatDot_3 + SEqHatDot_4 * SEqHatDot_4);
        SEqHatDot_1 /= norm;
        SEqHatDot_2 /= norm;
        SEqHatDot_3 /= norm;
        SEqHatDot_4 /= norm;
        // Compute the quaternion derrivative measured by gyroscopes
        SEqDot_omega_1 = -halfSEq_2 * w_x - halfSEq_3 * w_y - halfSEq_4 * w_z;
        SEqDot_omega_2 = halfSEq_1 * w_x + halfSEq_3 * w_z - halfSEq_4 * w_y;
        SEqDot_omega_3 = halfSEq_1 * w_y - halfSEq_2 * w_z + halfSEq_4 * w_x;
        SEqDot_omega_4 = halfSEq_1 * w_z + halfSEq_2 * w_y - halfSEq_3 * w_x;
        // Compute then integrate the estimated quaternion derrivative
        SEq_1 += (SEqDot_omega_1 - (beta * SEqHatDot_1)) * deltat;
        SEq_2 += (SEqDot_omega_2 - (beta * SEqHatDot_2)) * deltat;
        SEq_3 += (SEqDot_omega_3 - (beta * SEqHatDot_3)) * deltat;
        SEq_4 += (SEqDot_omega_4 - (beta * SEqHatDot_4)) * deltat;
        // Normalise quaternion
        norm = Mathf.Sqrt(SEq_1 * SEq_1 + SEq_2 * SEq_2 + SEq_3 * SEq_3 + SEq_4 * SEq_4);
        SEq_1 /= norm;
        SEq_2 /= norm;
        SEq_3 /= norm;
        SEq_4 /= norm;
        return new Quaternion(SEq_1, SEq_2, SEq_3, SEq_4);
    }

    public Quaternion attitude;
    public Vector3 acceleration;
    public Vector3 heading;
    public Vector3 RotationRate;
    public Quaternion EstimateOfOrientation_q_est = new Quaternion(1, 0, 0, 0);

    void Start()
    {
        Input.gyro.enabled = true;
        Input.compass.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        attitude = Input.gyro.attitude;
        RotationRate = Input.gyro.rotationRate;
        acceleration = Input.acceleration;
        heading = Input.compass.rawVector;
        Quaternion SEq = filterUpdate(RotationRate.x, RotationRate.y, RotationRate.z, acceleration.x, acceleration.y, acceleration.z);
        transform.rotation = SEq;
        Debug.Log(SEq);
    }
}
