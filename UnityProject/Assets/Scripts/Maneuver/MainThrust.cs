using UnityEngine;
using System.Collections;

[AddComponentMenu("Maneuver/Main Thrust")]
[RequireComponent(typeof(Rigidbody))]
public class MainThrust : MonoBehaviour {

    public float MaxThrust;

    public float Thrust {
        get { return thrust; }
        set { thrust = Mathf.Clamp(value, 0, MaxThrust); }
    }

    Rigidbody rb;

    float thrust;

    void Start () {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate () {
        rb.AddRelativeForce(Vector3.forward * thrust);
    }
}
