using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Physics 2D/Gravitational Mass")]
public class GravitationalMass : MonoBehaviour {

    static List<GravitationalMass> masses = new List<GravitationalMass>();

    public static IEnumerable<GravitationalMass> Masses {
        get { return masses; }
    }

    public const float G = 1000f;

    public float Mass;

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
