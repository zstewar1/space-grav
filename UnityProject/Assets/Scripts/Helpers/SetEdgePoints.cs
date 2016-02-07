using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SetEdgePoints : MonoBehaviour {

    public EdgeCollider2D Edge;

    public Vector2[] Points;

	// Use this for initialization
	void Start () {
        if(Edge)
            Edge.points = Points;
	}

#if UNITY_EDITOR
    // Update is called once per frame
    void Update () {
	    if (!Application.isPlaying && Edge) {
            Edge.points = Points;
        }
	}
#endif
}
