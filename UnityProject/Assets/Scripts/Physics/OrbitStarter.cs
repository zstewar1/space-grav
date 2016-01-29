using UnityEngine;
using System.Collections;

[AddComponentMenu("Physics/Orbit Starter")]
[RequireComponent(typeof(GravitationalMass))]
public class OrbitStarter : MonoBehaviour {

    public GravitationalMass Parent;

    public float SemimajorAxis;
    public float Eccentricity;

    public float Inclination;
    public float LongitudeOfAscendingNode;

    public float ArgumentOfPeriapsis;
    public float MeanAnomaly;

    public float VelocityBoostMultiplier = 1.0f;

    GravitationalMass mass;
    Rigidbody rb;

    float angularVelocity;
    Vector3 rotationalCenter;

    Quaternion ellipseToOrbit;

    void Awake() {
        mass = GetComponent<GravitationalMass>();
        rb = GetComponent<Rigidbody>();

        UpdateAngularVelocity();
        UpdateEllipseToOrbit();
    }

    void Start () {

        var pos = GetPosition();
        var nextPos = GetPositionAtTPlus(Time.fixedDeltaTime);

        rb.MovePosition(pos);
        rb.velocity = ((nextPos - pos) / Time.fixedDeltaTime) * VelocityBoostMultiplier;
    }

#if UNITY_EDITOR

    void FixedUpdate() {
        MeanAnomaly += angularVelocity * Time.fixedDeltaTime;
    }

#endif

#if UNITY_EDITOR
    void OnDrawGizmos () {
        if (Parent) {
            UpdateAngularVelocity();
            UpdateEllipseToOrbit();

            const float inc = Mathf.PI / 180f;

            var pos = GetParentPos();

            for (float theta = 0.0f; theta < 2f * Mathf.PI; theta += inc) {
                float old = theta - inc;
                Gizmos.DrawLine(
                    GetRelativePosition(EccentricToTrueAnomaly(theta)) + pos,
                    GetRelativePosition(EccentricToTrueAnomaly(old)) + pos);
            }

            var curr = GetPosition();
            if (!Application.isPlaying) {
                transform.position = curr;
            }
            Gizmos.DrawLine(pos, curr);

            var ascNode = GetRelativePosition(-ArgumentOfPeriapsis * Mathf.Deg2Rad) + pos;
            var descNode = GetRelativePosition(Mathf.PI - ArgumentOfPeriapsis * Mathf.Deg2Rad) + pos;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(ascNode, descNode);

            var apoapsis = GetRelativePosition(Mathf.PI) + pos;
            var periapsis = GetRelativePosition(0) + pos;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(apoapsis, periapsis);
        }
    }
#endif

    void UpdateAngularVelocity () {
        if (mass && Parent) {
            var µ = GravitationalMass.G * (Parent.Mass + mass.Mass);
            angularVelocity = Mathf.Sqrt(µ / (SemimajorAxis * SemimajorAxis * SemimajorAxis)) * Mathf.Rad2Deg;
        }
    }

    void UpdateEllipseToOrbit () {
        var argumentOfPeriapsis = Quaternion.Euler(0, -ArgumentOfPeriapsis, 0);
        var inclination = Quaternion.Euler(-Inclination, 0, 0);
        var longitudeOfAscendingNode = Quaternion.Euler(0, -LongitudeOfAscendingNode, 0);
        ellipseToOrbit = longitudeOfAscendingNode * inclination * argumentOfPeriapsis;
    }

    // Find position relative to center given trueAnomaly in radians.
    Vector3 GetRelativePosition (float trueAnomaly) {
        // Compute Radius
        var num = SemimajorAxis * (1 - Eccentricity * Eccentricity);
        var denom = 1 + Eccentricity * Mathf.Cos(trueAnomaly);
        var r = num / denom;

        var ellip = new Vector3(Mathf.Cos(trueAnomaly), 0, Mathf.Sin(trueAnomaly)) * r;
        return ellipseToOrbit * ellip;
    }

    float MeanToTrueAnomaly (float M) {
        return EccentricToTrueAnomaly(MeanToEccentricAnomaly(M));
    }

    float MeanToEccentricAnomaly (float M) {
        var E = M;

        float d;
        for (int i = 0; i < 100000; i++) {
            d = E - Eccentricity * Mathf.Sin(E) - M;
            if (Mathf.Abs(d) < 0.0001) break;

            var deltaE = d / (1f - Eccentricity * Mathf.Cos(E));
            E = E - deltaE;
        }

        return E;
    }

    float EccentricToTrueAnomaly (float E) {
        var num = Mathf.Sqrt(1-Eccentricity*Eccentricity) * Mathf.Sin(E);
        var denom = Mathf.Cos(E) - Eccentricity;
        return Mathf.Atan2(num, denom);
    }

    Vector3 GetPosition () {
        return GetRelativePosition(MeanToTrueAnomaly(MeanAnomaly * Mathf.Deg2Rad)) + GetParentPos();
    }

    Vector3 GetPositionAtTPlus (float t) {
        return GetRelativePosition(MeanToTrueAnomaly((MeanAnomaly + angularVelocity * t) * Mathf.Deg2Rad)) + GetParentPosAtTPlus(t);
    }

    Vector3 GetParentPos () {
        var parentOrbitStarter = Parent.GetComponent<OrbitStarter>();

        if (parentOrbitStarter) {
            return parentOrbitStarter.GetPosition();
        } else {
            return Parent.transform.position;
        }
    }

    Vector3 GetParentPosAtTPlus (float t) {
        var parentOrbitStarter = Parent.GetComponent<OrbitStarter>();

        if (parentOrbitStarter) {
            var res = parentOrbitStarter.GetPositionAtTPlus(t);
            return res;
        } else {
            return Parent.transform.position;
        }
    }
}
