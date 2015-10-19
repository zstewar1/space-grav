using UnityEngine;
using System.Collections;

[AddComponentMenu("Flight/Rotate")]
public class Rotate : MonoBehaviour {

    public float RotateSpeed;


	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 0, -RotateSpeed * Time.deltaTime * Input.GetAxis("Horizontal"));
	}
}
