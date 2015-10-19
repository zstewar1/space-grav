using UnityEngine;
using System.Collections;

[AddComponentMenu("Physics 2D/Integrator")]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(GravitationalObject))]
[RequireComponent(typeof(Rigidbody2D))]
public class Integrator : MonoBehaviour {

    GravitationalObject go;
    LineRenderer lr;
    Rigidbody2D rb;

    public int Steps = 1000;
    public float TargetDeltaV = 1.0f;

    public float IntegrateDelay = 1.0f;
    public int IntegrateMaxSteps = 1000;

	void Start () {
        go = GetComponent<GravitationalObject>();
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();

        lr.SetVertexCount(Steps + 1);
        lr.useWorldSpace = true;

        StartCoroutine(RunIntegrator());
	}

    IEnumerator RunIntegrator () {
        for(;;) {
            yield return StartCoroutine(Integrate());
            yield return new WaitForSeconds(IntegrateDelay);
        }
    }

	// Update is called once per frame
	IEnumerator Integrate () {
        Vector2 pos = rb.position;
        Vector2 vel = rb.velocity;
        float t = 0;

        lr.SetPosition(0, pos);

        for (int i = 1; i <= Steps; i++) {
            if (i % IntegrateMaxSteps == 0) {
                yield return null;
            }

            Vector2 a = Vector2.zero;

            foreach (var gm in GravitationalMass.Masses) {
                Vector2 masspos;
                var ko = gm.GetComponent<KinematicOrbit>();
                if (ko) {
                    masspos = ko.PredictPosition(t);
                } else {
                    masspos = gm.transform.position;
                }

                a += go.CalculateAccelPartial(pos, masspos, gm.Mass);
            }

            a *= GravitationalMass.G;

            var dt = TargetDeltaV / a.magnitude;

            vel += a * dt;
            pos += vel * dt;
            lr.SetPosition(i, pos);
            t += dt;
        }
	}
}
