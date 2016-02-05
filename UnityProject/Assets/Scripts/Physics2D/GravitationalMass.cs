using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Physics 2D/Gravitational Mass")]
[RequireComponent(typeof(Rigidbody2D))]
public class GravitationalMass : MonoBehaviour, QuadtreePoint {

    static List<GravitationalMass> masses = new List<GravitationalMass>();

    public static IEnumerable<GravitationalMass> Masses {
        get { return masses; }
    }

    public const float G = 1000f;

    public float Mass { get {
            return rb.mass;
        }
    }
    public Vector2 Position { get { return rb.worldCenterOfMass; } }

    public void AddForce(Vector2 force) {
        rb.AddForce(force);
    }

    Rigidbody2D rb;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

	// Use this for initialization
	void OnEnable () {
        Debug.LogFormat("Adding Grav Mass {0}", gameObject.name);
        masses.Add(this);
	}

	// Update is called once per frame
	void OnDisable () {
        Debug.LogFormat("Removing Grav Mass {0}", gameObject.name);
        masses.Remove(this);
	}
}
