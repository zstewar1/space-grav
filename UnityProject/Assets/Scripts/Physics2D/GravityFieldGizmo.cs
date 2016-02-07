using UnityEngine;
using System.Collections;

[AddComponentMenu("Physics 2D/Gravity Field Gizmo")]
public class GravityFieldGizmo : MonoBehaviour {

    public float StartX = -10f, EndX = 10f, XStep = 1f;
    public float StartY = -10f, EndY = 10f, YStep = 1f;

    public float FieldScale = 1f;
    public float LimitLength = 0;

    #if UNITY_EDITOR
    void OnDrawGizmos () {
        Gizmos.color = Color.green;

        for (float x = StartX; x <= EndX; x += XStep) {
            for (float y = StartY; y <= EndY; y += YStep) {

                var pos = new Vector2(x, y);

                var fieldLine = Vector2.zero;

                foreach (var gm in GravitationalMass.Masses) {
                    var coll = gm.GetComponent<Collider2D>();

                    if (coll && coll.OverlapPoint(pos)) continue;

                    Vector2 fieldVector = (Vector2)gm.transform.position - pos;
                    var d = fieldVector.magnitude;

                    fieldVector *= gm.GetComponent<Rigidbody2D>().mass / (d*d*d);
                    fieldLine += fieldVector;
                }

                fieldLine *= FieldScale;

                if (LimitLength > 0) {
                    fieldLine = Vector2.ClampMagnitude(fieldLine, LimitLength);
                }

                Gizmos.DrawRay(pos, fieldLine);
            }
        }
    }
    #endif
}
