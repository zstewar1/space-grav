using UnityEngine;
using System.Collections;

[AddComponentMenu("Physics/Gravitational Force")]
public class GravitationalForce : MonoBehaviour {

    public float Mass;

    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	    foreach (var mass in GravitationalMass.Masses) {
            var forceNormal = mass.transform.position - transform.position;
            var distanceSquared = forceNormal.sqrMagnitude;
            forceNormal.Normalize();
            
            var force = GravitationalMass.G * Mass * mass.Mass / distanceSquared * forceNormal;

            rb.AddForce(force);
        }
	}
}
