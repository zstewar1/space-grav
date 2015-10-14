using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera/Camera Circle")]
public class CameraCircle : MonoBehaviour {

    public Transform Center;
    public float DegreesPerSecond = 1;
    public float Elevation = 20;
    public float Distance = 100;

    float rotationalPosition = 0;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
        if (Center) {
            rotationalPosition += Time.deltaTime * DegreesPerSecond * Mathf.Deg2Rad;
            transform.position = Center.position + Distance * new Vector3(
                Mathf.Sin(rotationalPosition) * Mathf.Cos(Elevation * Mathf.Deg2Rad),
                Mathf.Sin(Elevation * Mathf.Deg2Rad),
                Mathf.Cos(rotationalPosition) * Mathf.Cos(Elevation * Mathf.Deg2Rad));
            transform.LookAt(Center);
        }
	}
}
