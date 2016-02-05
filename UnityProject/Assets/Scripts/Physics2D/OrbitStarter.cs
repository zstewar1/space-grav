using UnityEngine;
using System.Collections;

[AddComponentMenu("Physics 2D/Orbit Starter")]
[RequireComponent(typeof(Rigidbody2D))]
[ExecuteInEditMode]
public class OrbitStarter : MonoBehaviour {

    public Transform Parent;

    public float Distance = 10;
    public float Angle = 0;

    public float PathVelocity = 10;
    public float TangentOffset = 0;

    void Start () {
        var rb = GetComponent<Rigidbody2D>();
        rb.position = CalculatePosition();
        rb.velocity = CalculateVelocity();
    }

#if UNITY_EDITOR
    void Update () {
        if(!Application.isPlaying) {
            transform.position = CalculatePosition();
        }
    }
#endif

    public float CalculateOffsetAngle () {
        var parentAngle = 0f;
        if (Parent) {
            var parentStarter = Parent.GetComponent<OrbitStarter>();
            if(parentStarter) {
                parentAngle = parentStarter.CalculateOffsetAngle();
            }
        }

        return parentAngle + Angle;
    }

    public Vector2 CalculateVelocity() {
        var angle = (CalculateOffsetAngle() + 90f + TangentOffset) * Mathf.Deg2Rad;
        var relvel = PathVelocity * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        return relvel + ParentVelocity();
    }

    public Vector2 ParentVelocity() {
        if (Parent) {
            var parentStarter = Parent.GetComponent<OrbitStarter>();
            if(parentStarter) {
                return parentStarter.CalculateVelocity();
            }
        }
        return Vector2.zero;
    }

    public Vector2 CalculatePosition() {
        var angle = CalculateOffsetAngle() * Mathf.Deg2Rad;
        var relpos = Distance * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        return relpos + ParentPosition();
    }

    Vector2 ParentPosition () {
        if (Parent) {
            var parentStarter = Parent.GetComponent<OrbitStarter>();
            if (parentStarter) {
                return parentStarter.CalculatePosition();
            } 
            return Parent.position;
        }
        return Vector2.zero;
    }

    public void OnDrawGizmosSelected () {
        Gizmos.color = Color.white;

        var pos = CalculatePosition();
        Gizmos.DrawLine(pos, ParentPosition());

        Gizmos.color = Color.red;

        Gizmos.DrawRay(pos, CalculateVelocity());

        Gizmos.color = Color.green;
        Gizmos.DrawRay(pos, CalculateVelocity() - ParentVelocity());

        Gizmos.color = Color.white;
    }
}
