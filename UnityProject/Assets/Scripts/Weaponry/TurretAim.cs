using UnityEngine;
using System.Collections;

[AddComponentMenu("Weaponry/Turret Aim")]
public class TurretAim : MonoBehaviour {

    public Transform TraverseBone;
    public Transform ElevateBone;

    public bool LimitTraverse;
    public Vector2 TraverseLimit;
    public Vector2 ElevateLimit = new Vector2(-15, 85);

    public float TraverseRate = 30;
    public float ElevateRate = 20;

    public bool AimActive;
    public Vector3 Aimpoint;

    float traverse;
    float elevation;

    void Start () {
        traverse = 0;
        elevation = 0;
    }

	// Update is called once per frame
	void Update () {
        if (AimActive) {
            Vector3 TraverseTargetVec = Aimpoint - TraverseBone.position;
            TraverseTargetVec.Normalize();
            TraverseTargetVec = TraverseBone.parent.InverseTransformDirection(TraverseTargetVec);

            float goalTraverse = Mathf.Atan2(TraverseTargetVec.x, TraverseTargetVec.z) * Mathf.Rad2Deg;

            if (LimitTraverse) {
                #if UNITY_EDITOR
                if (TraverseLimit.x - TraverseLimit.y > 360) {
                    Debug.LogError("Traverse limits must be within 360 degrees of each other");
                }
                if (TraverseLimit.x > TraverseLimit.y) {
                    Debug.LogError("Lower traverse limit must be less than upper traverse limit");
                }
                if (TraverseLimit.x < -360 || TraverseLimit.x > 360 || TraverseLimit.y < -360 || TraverseLimit.y > 360) {
                    Debug.LogError("Traverse limits must be in the range [-360, 360]");
                }
                #endif

                float center = 0.5f * (TraverseLimit.x + TraverseLimit.y);

                if (goalTraverse < center - 180) {
                    goalTraverse += 360;
                } else if (goalTraverse > center + 180) {
                    goalTraverse += 360;
                }

                if (goalTraverse < TraverseLimit.x) {
                    goalTraverse = TraverseLimit.x;
                } else if (goalTraverse > TraverseLimit.y) {
                    goalTraverse = TraverseLimit.y;
                }

                traverse = Mathf.MoveTowards(traverse, goalTraverse, TraverseRate * Time.deltaTime);

            } else {
                traverse = Mathf.MoveTowardsAngle(traverse, goalTraverse, TraverseRate * Time.deltaTime);
            }

            TraverseBone.localRotation = Quaternion.AngleAxis(traverse, Vector3.up);

            Vector3 ElevateTargetVec = Aimpoint - ElevateBone.position;
            ElevateTargetVec.Normalize();
            ElevateTargetVec = ElevateBone.parent.InverseTransformDirection(ElevateTargetVec);

            float goalElevate = Mathf.Atan2(ElevateTargetVec.y, ElevateTargetVec.z) * Mathf.Rad2Deg;

            elevation = Mathf.Clamp(Mathf.MoveTowards(elevation, goalElevate, ElevateRate * Time.deltaTime), ElevateLimit.x, ElevateLimit.y);
            ElevateBone.localRotation = Quaternion.AngleAxis(elevation, Vector3.left);
        } else {
            if (traverse != 0) {
                if (LimitTraverse) {
                    traverse = Mathf.MoveTowards(traverse, 0, TraverseRate * Time.deltaTime);
                } else {
                    traverse = Mathf.MoveTowardsAngle(traverse, 0, TraverseRate * Time.deltaTime);
                }
                TraverseBone.localRotation = Quaternion.AngleAxis(traverse, Vector3.up);
            }

            if (elevation != 0) {
                elevation = Mathf.MoveTowards(elevation, 0, ElevateRate * Time.deltaTime);
                ElevateBone.localRotation = Quaternion.AngleAxis(elevation, Vector3.left);
            }
        }
    }
}
