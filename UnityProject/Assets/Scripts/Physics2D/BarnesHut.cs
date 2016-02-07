using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

[AddComponentMenu("Physics 2D/Barnes-Hut Gravity Simulation")]
[ExecuteInEditMode]
public class BarnesHut : MonoBehaviour {

    [DllImport("physrust")]
    public static unsafe extern Int32 barnes_hut_calculate_forces (BarnesHutC* bh, QuadtreePointImpl* points, Vector2* outp, UIntPtr npoints);

    /// <summary>
    /// Representation of the rust BarnesHut struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BarnesHutC {
        public Vector2 center;
        public float size;
        public float theta;
        public float g;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct QuadtreePointImpl {
        public Vector2 position;
        public float mass;
    }

    BarnesHutC underlying;

    public float Theta = 1.0f;
    public float G = 1.0f;

    public Vector2 Center = Vector2.zero;
    public float Size = 10f;
    
    void Start () {
        underlying.center = Center;
        underlying.size = Size;
        underlying.theta = Theta;
        underlying.g = G;
    }

#if UNITY_EDITOR
    void Update () {
        if (!Application.isPlaying) {
            underlying.center = Center;
            underlying.size = Size;
            underlying.theta = Theta;
            underlying.g = G;
        }
    }
#endif

    void FixedUpdate () {

        QuadtreePointImpl[] points = new QuadtreePointImpl[GravitationalMass.Masses.Count];
        Vector2[] outputs = new Vector2[GravitationalMass.Masses.Count];

        for(int i = 0; i < GravitationalMass.Masses.Count; i++) {
            var m = GravitationalMass.Masses[i];
            points[i].position = m.Position;
            points[i].mass = m.Mass;
        }

        //for (int i = 0; i < points.Length; i++) {
        //    Debug.LogFormat("p{3}x: {0}, p{3}y: {1}, p{3}m: {2}", points[i].position.x, points[i].position.y, points[i].mass, i);
        //}
        
        for(int i = 0; i < outputs.Length; i++) {
            outputs[i] = Vector2.zero;
        }

        unsafe
        {
            fixed (QuadtreePointImpl* pts = points)
            {
                fixed(Vector2* outs = outputs)
                {
                    fixed(BarnesHutC* bh = &underlying)
                    {
                        barnes_hut_calculate_forces(bh, pts, outs, (UIntPtr)GravitationalMass.Masses.Count);

                    }
                }
            }
        }

        for(int i = 0; i < GravitationalMass.Masses.Count; i++) {
            GravitationalMass.Masses[i].AddForce(outputs[i]);
        }
    }
}
