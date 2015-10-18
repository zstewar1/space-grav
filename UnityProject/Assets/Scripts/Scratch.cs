using UnityEngine;
using System.Collections;

public class Scratch : MonoBehaviour {

    public GameObject Ship;

    public Vector3 Aimpoint;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
        Aimpoint = new Vector3(
            Mathf.Cos(Time.time * 0.5f) * Mathf.Sin(Time.time * 0.15f),
            Mathf.Sin(Time.time * 0.5f) * Mathf.Sin(Time.time * 0.15f),
            Mathf.Cos(Time.time * 0.15f)) * 100f;

        foreach (var t in Ship.GetComponentsInChildren<TurretAim>()) {
            Debug.Log(Aimpoint);
            t.AimActive = true;
            t.Aimpoint = Aimpoint;
        }
	}

    void OnDrawGizmos () {
        Gizmos.DrawLine(Vector3.zero, Aimpoint);
    }
}
