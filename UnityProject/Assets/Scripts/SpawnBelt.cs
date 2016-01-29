using UnityEngine;
using System.Collections;

public class SpawnBelt : MonoBehaviour {

    public int NumberToSpawn;

    OrbitStarter os;

    void Awake () {
        os = GetComponent<OrbitStarter>();
    }

	// Use this for initialization
	void Start () {
        this.enabled = false;
        float step = 360f / (NumberToSpawn + 1);
        float MA = os.MeanAnomaly;
	    for(int i = 0; i < NumberToSpawn; i++) {
            MA += step;

            var spawned = Instantiate<OrbitStarter>(os);
            spawned.MeanAnomaly = MA;
        }
	}
}
