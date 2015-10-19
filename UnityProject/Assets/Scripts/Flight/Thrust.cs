using UnityEngine;
using System.Collections;

[AddComponentMenu("Flight/Thrust")]
[RequireComponent(typeof(Rigidbody2D))]
public class Thrust : MonoBehaviour {

    public float MaxThrust;

    float thrust;

    public float CurrentThrust { get { return thrust; } }

    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}

    void Update () {
        thrust = Mathf.Clamp01(Input.GetAxis("Vertical")) * MaxThrust;
    }

	// Update is called once per frame
	void FixedUpdate () {
        rb.AddRelativeForce(thrust * Vector2.up);
	}
}
