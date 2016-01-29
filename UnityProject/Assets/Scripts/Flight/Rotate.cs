using UnityEngine;
using System.Collections;

[AddComponentMenu("Flight/Rotate")]
public class Rotate : MonoBehaviour {

    public float RotateTorque;

    public PID Stabilizer;

    public bool LockAngle;
    public AnglePID AngleLocker;

    Rigidbody2D rb;

	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update () {
        if (LockAngle) {
            rb.AddTorque(Mathf.Clamp(AngleLocker.Step(rb.rotation), -RotateTorque, RotateTorque));
        } else { 
            var torque = -Input.GetAxis("Horizontal") * RotateTorque;
            rb.AddTorque(torque);

            if (Input.GetButtonDown("Stabilize")) {
                Stabilizer.Reset();
            }
            if (Input.GetButton("Stabilize")) {
                rb.AddTorque(Mathf.Clamp(Stabilizer.Step(rb.angularVelocity), -RotateTorque, RotateTorque));
            }
        }
	}
}
