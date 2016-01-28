using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Physics/Gravitational Mass")]
public class GravitationalMass : MonoBehaviour {

    static List<GravitationalMass> masses = new List<GravitationalMass>();

    public static IEnumerable<GravitationalMass> Masses {
        get { return masses; }
    }

    public const float G = 500f;

    public float Mass;

    void OnEnable () {
        Debug.LogFormat("Adding Grav Mass {0}", gameObject.name);
        masses.Add(this);
    }

    void OnDisable () {
        Debug.LogFormat("Removing Grav Mass {0}", gameObject.name);
        masses.Remove(this);
    }

}
