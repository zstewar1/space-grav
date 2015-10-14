using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AttachPrefabList {
    public Transform Prefab;
    public List<Transform> AttachPoints;
}

[AddComponentMenu("Attachment/Basic Attach")]
public class BasicAttach : MonoBehaviour {

    public List<AttachPrefabList> PrefabAttachments;

	// Use this for initialization
	void Start () {
        foreach(var prefatt in PrefabAttachments) {
            if (prefatt != null && prefatt.Prefab) {
                foreach(var attpnt in prefatt.AttachPoints) {
                    if (attpnt) {
                        var createdTrans = Instantiate<Transform>(prefatt.Prefab);
                        createdTrans.parent = attpnt;
                        createdTrans.localPosition = Vector3.zero;
                        createdTrans.localRotation = Quaternion.identity;
                    }
                }
            }
        }
    }
}
