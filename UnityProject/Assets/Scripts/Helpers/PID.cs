using System;
using UnityEngine;

[Serializable]
public class PID {
    public float Kp;
    public float Ki;
    public float Kd;

    public float SetPoint;

    float integral;
    float previous;

    public void Reset() {
        integral = 0;
        previous = 0;
    }
    
    public float Step(float measured) {
        var error = SetPoint - measured;
        integral += error * Time.deltaTime;
        var derivative = (error - previous) / Time.deltaTime;
        var ret = Kp * error + Ki * integral + Kd * derivative;
        previous = error;
        return ret;
    }
}

[Serializable]
public class AnglePID {
    public float Kp;
    public float Ki;
    public float Kd;

    public float SetPoint;

    float integral;
    float previous;

    public void Reset () {
        integral = 0;
        previous = 0;
    }

    public float Step (float measured) {
        var error = Mathf.DeltaAngle(measured, SetPoint);
        integral += error * Time.deltaTime;
        var derivative = (error - previous) / Time.deltaTime;
        var ret = Kp * error + Ki * integral + Kd * derivative;
        previous = error;
        return ret;
    }
}