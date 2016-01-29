using UnityEngine;
using System.Collections;

[AddComponentMenu("Physics 2D/Barnes-Hut Gravity Simulation")]
[ExecuteInEditMode]
public class BarnesHut : MonoBehaviour {

    public const float Theta = 1f;

    public Quadtree<GravitationalMass> Quadtree = new Quadtree<GravitationalMass>();

    public Vector2 SimulationCenter = Vector2.zero;
    public float SimulationSize = 10f;

    public bool allQuadrants = false;

    void OnEnable () {
        Quadtree.Reset();
        Quadtree.Origin = (Vector2)transform.position + SimulationCenter;
        Quadtree.HalfDimension = SimulationSize * 0.5f;
    }

#if UNITY_EDITOR
    void Update() {
        if(!Application.isPlaying) {
            OnEnable();
        }
    }
#endif

    void FixedUpdate () {
        Quadtree.Reset();
        foreach(var mass in GravitationalMass.Masses) {
            Quadtree.Insert(mass);
        }
        Quadtree.CalculateMassDistribution();
        foreach (var mass in GravitationalMass.Masses) {
            mass.AddForce(Quadtree.CalculateForce(mass));
        }
    }

    void OnDrawGizmos() {
        Quadtree.DrawGizmos(allQuadrants ? int.MaxValue : 1);
    }
}

public interface QuadtreePoint {
    Vector2 Position { get; }
    float Mass { get; }
}

public class Quadtree<T> where T : class, QuadtreePoint {

    public Vector2 Origin { get; set; }
    public float HalfDimension { get; set; }

    Vector2 centerOfMass;
    float mass;

    Quadtree<T>[] children;

    T data;

    public Quadtree (Vector2 origin, float halfDimension) {
        Origin = origin;
        HalfDimension = halfDimension;
    }

    public Quadtree () : this(Vector2.zero, 0f) { }

    public void Reset () {
        children = null;
        data = null;
        centerOfMass = Vector2.zero;
        mass = 0f;
    }

    int getPointQuadrant (Vector2 point) {
        int quad = 0;
        if (point.x >= Origin.x) quad |= 2;
        if (point.y >= Origin.y) quad |= 1;
        return quad;
    }

    public void Insert (T element) {
        if(children == null) {
            if(data == null) {
                data = element;
            } else {
                children = new Quadtree<T>[4];
                for(int i = 0; i < children.Length; i++) {
                    var childOrigin = Origin;
                    childOrigin.x += HalfDimension * ((i & 2) != 0 ? 0.5f : -0.5f);
                    childOrigin.y += HalfDimension * ((i & 1) != 0 ? 0.5f : -0.5f);
                    children[i] = new Quadtree<T>(childOrigin, HalfDimension * 0.5f);
                }

                children[getPointQuadrant(data.Position)].Insert(data);
                data = null;
                children[getPointQuadrant(element.Position)].Insert(element);
            }
        } else {
            children[getPointQuadrant(element.Position)].Insert(element);
        }
    }

    public void CalculateMassDistribution () {
        if (children == null) {
            if (data != null) {
                mass = data.Mass;
                centerOfMass = data.Position;
            } // Else empty, so ignore.
        } else {
            mass = 0;
            centerOfMass = Vector2.zero;
            foreach (var child in children) {
                child.CalculateMassDistribution();
                mass += child.mass;
                centerOfMass += child.centerOfMass * child.mass;
            }
            centerOfMass /= mass;
        }
    }

    public Vector2 CalculateForce (T targ) {
        var force = Vector2.zero;

        if(children == null) {
            if (data != null & data != targ) {
                force = data.Position - targ.Position;
                var dist = force.magnitude;
                dist *= dist * dist;
                force = force * GravitationalMass.G * data.Mass * targ.Mass / dist;
            } // else force stays zero
        } else {
            var dir = centerOfMass - targ.Position;
            var dist = dir.magnitude;
            var height = HalfDimension * 2;
            if (height / dist < BarnesHut.Theta) {
                force = dir * GravitationalMass.G * mass * targ.Mass / (dist * dist * dist);
            } else {
                foreach(var child in children) {
                    force += child.CalculateForce(targ);
                }
            }
        }

        return force;
    }

    public void DrawGizmos (int forceDrawDepth = 1) {
        if(data != null || forceDrawDepth > 0) {
            var size = HalfDimension * 2;
            Gizmos.DrawWireCube(Origin, new Vector3(size, size, size));
        }
        if (children != null) {
            foreach(var child in children) {
                child.DrawGizmos(forceDrawDepth - 1);
            }
        }
    }
}
