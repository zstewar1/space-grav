using UnityEngine;
using System.Collections;
using System;

[AddComponentMenu("Physics/Barnes-Hut Gravity Simulation")]
[ExecuteInEditMode]
public class BarnesHut : MonoBehaviour {

    public Octree<GravitationalMass> Octree = new Octree<GravitationalMass>(Vector3.zero, Vector3.one * 10);

    public const float Theta = 1f;

    public float SimulationSize = 10f;
    public Vector3 SimulationCenter = Vector3.zero;
    public bool allOctants;

    void OnEnable () {
        Octree.Reset();
        Octree.Origin = SimulationCenter + transform.position;
        Octree.HalfDimension = new Vector3(SimulationSize, SimulationSize, SimulationSize);
    }

#if UNITY_EDITOR
    void Update () {
        if (!Application.isPlaying) {
            OnEnable();
        }
    }
#endif

    void FixedUpdate () {
        Octree.Reset();
        foreach(var mass in GravitationalMass.Masses) {
            Octree.Insert(mass);
        }
        Octree.CalculateMassDistribution();
        foreach(var mass in GravitationalMass.Masses) {
            mass.addForce(Octree.CalculateForce(mass));
        }
    }

    public void OnDrawGizmos () {
        Octree.DrawGizmos(allOctants ? int.MaxValue : 1);
    }
}

public interface OctreePoint {
    Vector3 Position { get; }
    float Mass { get; }
}


public class Octree<T> where T : class, OctreePoint {
    public Vector3 Origin;
    public Vector3 HalfDimension;

    public Vector3 CenterOfMass;
    public float Mass;

    public Octree<T>[] Children;

    T Data;

    public Octree (Vector3 origin, Vector3 halfDimension) {
        Origin = origin;
        HalfDimension = halfDimension;
    }

    // Reset this node -- usually just for the root.
    public void Reset () {
        Children = null;
        Data = null;
        CenterOfMass = Vector3.zero;
        Mass = 0.0f;
    }

    /// <summary>
    /// Figure out which sub-octant the given point is in.
    /// 
    /// Assumes that the point is contained within this octant.
    /// </summary>
    /// <param name="point">The point</param>
    /// <returns>The index of the sub-octant containing the point</returns>
    int GetOctantPoint (Vector3 point) {
        int ret = 0;
        if (point.x >= Origin.x) ret |= 4;
        if (point.y >= Origin.y) ret |= 2;
        if (point.z >= Origin.z) ret |= 1;
        return ret;
    }

    public void Insert (T element) {
        // This element is a leaf if there are no children.
        if (Children == null) {
            if (Data == null) {
                // No data here? Just store it.
                Data = element;
            } else {
                // Initialize the children array.
                Children = new Octree<T>[8];
                // Create all of the child nodes.
                for (int i = 0; i < Children.Length; i++) {
                    var childOrigin = Origin;
                    childOrigin.x += HalfDimension.x * ((i & 4) != 0 ? 0.5f : -0.5f);
                    childOrigin.y += HalfDimension.y * ((i & 2) != 0 ? 0.5f : -0.5f);
                    childOrigin.z += HalfDimension.z * ((i & 1) != 0 ? 0.5f : -0.5f);
                    Children[i] = new Octree<T>(childOrigin, HalfDimension * 0.5f);
                }

                Children[GetOctantPoint(Data.Position)].Insert(Data);
                Data = null;
                Children[GetOctantPoint(element.Position)].Insert(element);
            }
        } else {
            Children[GetOctantPoint(element.Position)].Insert(element);
        }
    }

    // Find the center of mass of the octree and its total mass recursively over all chilren.
    public void CalculateMassDistribution () {
        if (Children == null) {
            if (Data != null) {
                Mass = Data.Mass;
                CenterOfMass = Data.Position;
            } // Else empty
        } else {
            Mass = 0;
            CenterOfMass = Vector3.zero;
            foreach (var child in Children) {
                child.CalculateMassDistribution();
                Mass += child.Mass;
                CenterOfMass += child.CenterOfMass * child.Mass;
            }
            CenterOfMass /= Mass;
        }
    }

    // Calculate the force on Targ
    public Vector3 CalculateForce(T targ) {
        var force = Vector3.zero;
        if (Children == null) {
            if (Data != null && Data != targ) {
                force = Data.Position - targ.Position;
                var dist = force.magnitude;
                dist *= dist * dist;
                force = force * GravitationalMass.G * Data.Mass * targ.Mass / dist;
            } // Else force stays zero.
        } else {
            var dir = CenterOfMass - targ.Position;
            var dist = dir.magnitude;
            var height = HalfDimension.x * 2;
            if (height/dist < BarnesHut.Theta) {
                force = dir * GravitationalMass.G * Mass * targ.Mass / (dist * dist * dist);
            } else {
                foreach(var child in Children) {
                    force += child.CalculateForce(targ);
                }
            }
        }
        return force;
    }

    public void DrawGizmos (int forceDrawDepth = 1) {
        if (Data != null || forceDrawDepth > 0) {
            Gizmos.DrawWireCube(Origin, HalfDimension * 2);
        }
        if (Children != null) {
            foreach (var child in Children) {
                child.DrawGizmos(forceDrawDepth - 1);
            }
        }
    }
}