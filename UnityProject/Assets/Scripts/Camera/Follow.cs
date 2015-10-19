using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera/Follow")]
public class Follow : MonoBehaviour {

    public Transform Target;
    public float LerpFactor = 0.9f;

	// Update is called once per frame
	void Update () {
        if (Target) {
            var pos = Vector3.Lerp(transform.position, Target.position, LerpFactor);
            pos.z = transform.position.z;
            transform.position = pos;
        }
	}
}
