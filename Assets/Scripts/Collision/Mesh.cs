using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh : MonoBehaviour {

    /* --- COMPONENT --- */
    public Renderer2D _renderer;
    public CapsuleCollider2D hull;

    /* --- VARIABLES --- */
    [HideInInspector] public float depth = 0f;

    void Update() {
        Depth();
    }

    void Depth() {
        depth = -(transform.position.y + hull.offset.y);
    }

    public static int Compare(Mesh meshA, Mesh meshB) {
        // Compare x and y in reverse order.
        return meshA.depth.CompareTo(meshB.depth);
    }

}
