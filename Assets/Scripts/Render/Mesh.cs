using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh : MonoBehaviour {

    /* --- COMPONENT --- */
    public Transform hull;

    /* --- VARIABLES --- */
    [HideInInspector] public float depth = 0f;

    /* --- Unity --- */
    void Update() {
        Depth();
        Render();
    }

    /* --- Methods --- */
    void Depth() {
        depth = -(transform.position.y + hull.position.y);
    }

    /* --- Static --- */

    // Compare the depth of the meshes.
    public static int Compare(Mesh meshA, Mesh meshB) {
        return meshA.depth.CompareTo(meshB.depth);
    }

}
