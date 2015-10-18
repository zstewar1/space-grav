using UnityEngine;
using System.Collections;

public class Scratch : MonoBehaviour {

    public MainThrust engine;

    public Rotate maneuver;

    public float Thrust;

    public bool useGoal;
    public Vector3 goalDir;
    public Vector3 up = Vector3.up;

	// Update is called once per frame
	void Update () {
        engine.Thrust = Thrust;

        if (!useGoal) {
            maneuver.Maneuver = new Vector3(Input.GetAxis("Pitch"), Input.GetAxis("Yaw"), Input.GetAxis("Roll"));
        } else {
            maneuver.GoalRot = Quaternion.LookRotation(goalDir, up);
        }
	}
}
