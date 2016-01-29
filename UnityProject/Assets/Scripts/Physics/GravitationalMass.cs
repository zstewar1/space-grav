using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Physics/Gravitational Mass")]
[RequireComponent(typeof(Rigidbody))]
public class GravitationalMass : MonoBehaviour, OctreePoint {

    static List<GravitationalMass> masses = new List<GravitationalMass>();

    public static IEnumerable<GravitationalMass> Masses {
        get { return masses; }
    }

    public const float G = 50000f;

    public float mass = 1;

    private Rigidbody rb;

    public Vector3 Position { get { return rb.worldCenterOfMass; } }
    public float Mass { get { return mass; } }

    public void addForce(Vector3 force) {
        rb.AddForce(force);
    }

    // Use this for initialization
    void Awake () {
        rb = GetComponent<Rigidbody>();
        rb.mass = Mass;
    }

    void OnEnable () {
        Debug.LogFormat("Adding Grav Mass {0}", gameObject.name);
        masses.Add(this);
    }

    void OnDisable () {
        Debug.LogFormat("Removing Grav Mass {0}", gameObject.name);
        masses.Remove(this);
    }

}
