using UnityEngine;
using System.Collections;

public class Scratch : MonoBehaviour {

    public Transform Aimpoint;
    public GameObject Ship;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
        foreach (var t in Ship.GetComponentsInChildren<TurretAim>()) {
            if(Aimpoint) {
                t.AimActive = true;
                t.Aimpoint = Aimpoint.position;
            }
            else {
                t.AimActive = false;
            }
        }
	}
}
