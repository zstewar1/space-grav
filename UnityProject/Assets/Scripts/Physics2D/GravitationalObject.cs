using UnityEngine;
using System.Collections;

[AddComponentMenu("Physics 2D/Gravitational Object")]
[RequireComponent(typeof(Rigidbody2D))]
public class GravitationalObject : MonoBehaviour {

    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void FixedUpdate () {
        rb.AddForce(CalculateForce());
	}

    Vector2 CalculateForce () {
        return CalculateAccel() * rb.mass;
    }

    Vector2 CalculateAccel () {
        Vector2 accel = Vector2.zero;

        foreach (var gm in GravitationalMass.Masses) {
            accel += CalculateAccelPartial(transform.position, gm.transform.position, gm.Mass);
        }
        return accel * GravitationalMass.G;
    }

    public Vector2 CalculateAccelPartial(Vector2 pos, Vector2 other, float otherMass) {
        Vector2 accelVector = other - pos;
        var dist = accelVector.magnitude;

        accelVector *= otherMass / (dist*dist*dist);

        return accelVector;
    }

    // #if UNITY_EDITOR
    // void OnDrawGizmosSelected () {
    //     if (!rb) return;

    //     Gizmos.color = Color.red;
    //     Gizmos.DrawRay(transform.position, CalculateForce());

    //     Gizmos.color = Color.blue;

    //     Vector2 pos = rb.position;
    //     Vector2 vel = rb.velocity;
    //     float t = 0;
    //     for (int i = 0; i < 5000; i++) {
    //         Vector2 a = Vector2.zero;

    //         foreach (var gm in GravitationalMass.Masses) {
    //             Vector2 masspos;
    //             var ko = gm.GetComponent<KinematicOrbit>();
    //             if (ko) {
    //                 masspos = ko.PredictPosition(t);
    //             } else {
    //                 masspos = gm.transform.position;
    //             }

    //             a += CalculateAccelPartial(pos, masspos, gm.Mass);
    //         }

    //         a *= GravitationalMass.G;

    //         var dt = 1f / a.magnitude;

    //         vel += a * dt;
    //         var newPos = pos + vel * dt;
    //         Gizmos.DrawLine(pos, newPos);
    //         pos = newPos;
    //         t += dt;
    //     }
    // }
    // #endif
}
