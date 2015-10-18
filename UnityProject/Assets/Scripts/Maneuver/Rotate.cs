using UnityEngine;
using System.Collections;

[AddComponentMenu("Maneuver/Rotate")]
[RequireComponent(typeof(Rigidbody))]
public class Rotate : MonoBehaviour {

    enum RotMode {
        Maneuver,
        Direction,
    }

    public float RotateSpeed;

    public Vector3 Maneuver {
        get { return maneuver; }
        set {
            maneuver = value;
            rotMode = RotMode.Maneuver;
        }
    }

    public Quaternion GoalRot {
        get { return goalDir; }
        set {
            goalDir = value;
            rotMode = RotMode.Direction;
        }
    }

    Rigidbody rb;

    Vector3 maneuver;
    Quaternion goalDir;

    RotMode rotMode = RotMode.Maneuver;


    void Start () {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate () {
        switch (rotMode) {
        case RotMode.Maneuver:
            var rot = maneuver;
            rot.x *= RotateSpeed;
            rot.y *= RotateSpeed;
            rot.z *= RotateSpeed;

            var t = transform.TransformDirection(rot * Time.fixedDeltaTime);
            rb.MoveRotation(Quaternion.Euler(t) * transform.rotation);
            break;
        case RotMode.Direction:
            rb.MoveRotation(Quaternion.RotateTowards(
                                transform.rotation, goalDir, RotateSpeed * Time.fixedDeltaTime));
            break;
        }
    }
}
