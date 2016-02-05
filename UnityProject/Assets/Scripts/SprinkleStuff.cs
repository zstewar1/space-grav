using UnityEngine;

[System.Serializable]
public class StuffToSprinkle {
    public int NumberOfThings;
    public OrbitStarter Thing;

    public float MinRadius, MaxRadius;
}

public class SprinkleStuff : MonoBehaviour {

    public StuffToSprinkle[] StuffToSprinkle;

	// Use this for initialization
	void Start () {
	    foreach(var thing in StuffToSprinkle) {
            for(int i = 0; i < thing.NumberOfThings; i++) {
                var newThing = Instantiate<OrbitStarter>(thing.Thing);

                var radius = Random.Range(thing.MinRadius, thing.MaxRadius);
                var angle = Random.Range(0, 360);

                newThing.Distance = radius;
                newThing.Angle = angle;
                newThing.transform.position = newThing.CalculatePosition();
            }
        }
	}
}
