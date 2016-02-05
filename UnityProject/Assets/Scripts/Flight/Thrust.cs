using UnityEngine;
using System.Collections;

[AddComponentMenu("Flight/Thrust")]
[RequireComponent(typeof(Rigidbody2D))]
public class Thrust : MonoBehaviour {

    public float MaxThrust;

    Rigidbody2D rb;

	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody2D>();
	}

    void Update () {
        var thrust = Mathf.Clamp01(Input.GetAxis("Vertical")) * MaxThrust;
        rb.AddRelativeForce(thrust * Vector2.up);
    }
}
