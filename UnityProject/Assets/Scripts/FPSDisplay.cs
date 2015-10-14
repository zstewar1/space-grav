using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class FPSDisplay : MonoBehaviour {

    public Text display;

    float Δt;
    const float α = 0.1f;

	// Update is called once per frame
	void Update () {
        Δt = α * Time.deltaTime + (1-α) * Δt;
        float fps = 1.0f/Δt;

        if (display) {
            display.text = string.Format("{0}", fps);
        }
	}
}
