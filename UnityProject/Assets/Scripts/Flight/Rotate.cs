using UnityEngine;
using System.Collections;

[AddComponentMenu("Flight/Rotate")]
public class Rotate : MonoBehaviour {

    public float RotateTorque;

    public PID Stabilizer;
    public bool AutoStabilize;

    public bool LockAngle;
    public AnglePID AngleLocker;

    public float ForwardAngle;

    Rigidbody2D rb;

	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update () {
        if (LockAngle) {
            rb.AddTorque(Mathf.Clamp(AngleLocker.Step(rb.rotation), -RotateTorque, RotateTorque));

            if (!Mathf.Approximately(0, Input.GetAxis("Horizontal")) || Input.GetButton("Stabilize")) {
                LockAngle = false;
            }
        }

        if(Input.GetButtonDown("Fire1")) {
            var clicked = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var diff = (Vector2)clicked - rb.position;

            Debug.DrawLine(clicked, rb.position);

            var angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - ForwardAngle;

            LockAngle = true;
            AngleLocker.SetPoint = angle;
        }

        if (!Mathf.Approximately(0, Input.GetAxis("Horizontal"))) {
            var torque = -Input.GetAxis("Horizontal") * RotateTorque;
            rb.AddTorque(torque);
        } else if (Input.GetButton("Stabilize") || (AutoStabilize && !LockAngle)) {
            rb.AddTorque(Mathf.Clamp(Stabilizer.Step(rb.angularVelocity), -RotateTorque, RotateTorque));
        }
	}
}
