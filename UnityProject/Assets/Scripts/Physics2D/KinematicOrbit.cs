using UnityEngine;
using System.Collections;

[AddComponentMenu("Physics 2D/Kinematic Orbit")]
[RequireComponent(typeof(GravitationalMass))]
[RequireComponent(typeof(Rigidbody2D))]
public class KinematicOrbit : MonoBehaviour {

    public GravitationalMass Center;

    public float AngleOffset;
    public float SemimajorAxis;
    public float SemiminorAxis;

    float eccentricity;

    public float Angle;

    public bool InvertDirection;

    public float AngularVelocity { get { return angularVelocity; } }

    float angularVelocity;
    Vector2 rotationalCenter;

    GravitationalMass gm;
    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        gm = GetComponent<GravitationalMass>();

        UpdateEccentricity();

        UpdateRotationalCenter();

        UpdateAngularVelocity();
	}

	// Update is called once per frame
	void Update () {
        Angle += angularVelocity * Time.deltaTime;
        if (Angle > 360) Angle -= 360;
        else if (Angle < 0) Angle += 360;
    }

    void FixedUpdate () {
        rb.MovePosition(GetPosition());
	}

    void UpdateAngularVelocity () {
        var µ = GravitationalMass.G * (Center.Mass + gm.Mass);
        angularVelocity = Mathf.Sqrt(µ / Mathf.Pow(SemimajorAxis, 3)) * Mathf.Rad2Deg;

        if(InvertDirection) {
            angularVelocity *= -1;
        }
    }

    void UpdateRotationalCenter () {
        var d = 2 * Mathf.Sqrt(SemimajorAxis * SemimajorAxis - SemiminorAxis * SemiminorAxis);

        rotationalCenter = new Vector2(
            Mathf.Cos(AngleOffset * Mathf.Deg2Rad), Mathf.Sin(AngleOffset * Mathf.Deg2Rad)) * d;
    }

    void UpdateEccentricity () {
        if (SemimajorAxis < SemiminorAxis) {
            var tmp = SemimajorAxis;
            SemimajorAxis = SemiminorAxis;
            SemiminorAxis = tmp;
        }

        eccentricity = Mathf.Sqrt(
            1.0f - ((SemiminorAxis * SemiminorAxis) / (SemimajorAxis * SemimajorAxis)));
    }

    public static float GetRadius (float a, float e, float theta, float offset) {
        return (a * (1 - e * e)) / (1 - e * Mathf.Cos(theta + (Mathf.PI - offset)));
    }

    public static Vector2 GetPosition (float a, float e, float theta, float offset, Vector2 center) {
        var radius = GetRadius(a, e, theta, offset);
        return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * radius + center;
    }

    public Vector2 GetPosition () {
        return GetPosition(Angle * Mathf.Deg2Rad);
    }

    public Vector2 GetPosition (float angle) {
        return GetPosition(angle, Center.transform.position);
    }

    public Vector2 GetPosition (float angle, Vector2 center) {
        return GetPosition(
            SemimajorAxis, eccentricity, angle, AngleOffset * Mathf.Deg2Rad,
            center + rotationalCenter);
    }

    public Vector2 PredictPosition(float time) {
        var parent = Center.GetComponent<KinematicOrbit>();

        Vector2 center;
        if (parent) {
            center = parent.PredictPosition(time);
        } else {
            center = Center.transform.position;
        }

        var angle = Angle + AngularVelocity * time;

        return GetPosition(angle * Mathf.Deg2Rad, center);
    }

    #if UNITY_EDITOR
    void OnDrawGizmos () {
        UpdateEccentricity();
        UpdateRotationalCenter();

        const float inc = Mathf.PI / 180.0f;

        for (float theta = 0.0f; theta < 2 * Mathf.PI; theta += inc) {
            float old = theta - inc;

            Gizmos.DrawLine(GetPosition(old), GetPosition(theta));
        }

        var curr = GetPosition();
        Gizmos.DrawLine(Center.transform.position, curr);
        // Gizmos.DrawLine(rotationalCenter, curr);
    }
    #endif
}
