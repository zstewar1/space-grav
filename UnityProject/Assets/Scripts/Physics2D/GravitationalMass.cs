using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Physics 2D/Gravitational Mass")]
[RequireComponent(typeof(Rigidbody2D))]
public class GravitationalMass : MonoBehaviour {

    public static readonly List<GravitationalMass> Masses = new List<GravitationalMass>();

    Rigidbody2D rb;

    public Vector2 Position {
        get { return rb.worldCenterOfMass; }
    }

    public float Mass {
        get { return rb.mass; }
    }

    public void AddForce(Vector2 force) {
        rb.AddForce(force);
    }

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

	// Use this for initialization
	void OnEnable () {
        //Debug.LogFormat("Adding Grav Mass {0}", gameObject.name);
        Masses.Add(this);
	}

	// Update is called once per frame
	void OnDisable () {
        //Debug.LogFormat("Removing Grav Mass {0}", gameObject.name);
        Masses.Remove(this);
	}
}
