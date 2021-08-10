using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Sort : MonoBehaviour {

    /*--- COMPONENTS ---*/
    public static string meshTag = "Mesh";

    /*--- UNITY ---*/
    void Start() {
    }

    void Update() {
        MinimumSort();
    }

    /* --- METHODS --- */
    public static void MinimumSort() {
        print("Sorting");
        // Declare the object array and the array of sorted characters
        GameObject[] unsortedObjects = GameObject.FindGameObjectsWithTag(meshTag);
        
        // assumes all the objects tagged with meshes have mesh components
        Mesh[] meshes = new Mesh[unsortedObjects.Length];
        for (int i = 0; i < unsortedObjects.Length; i++) {
            meshes[i] = unsortedObjects[i].GetComponent<Mesh>();
        }

        // the depth is understood as the position of the y axis
        // sort these
        Array.Sort<Mesh>(meshes, new Comparison<Mesh>( (meshA, meshB) => Mesh.Compare(meshA, meshB) ) );
        for (int i = 0; i < meshes.Length; i++) {
            meshes[i]._renderer.spriteRenderer.sortingOrder = i;
        }
    }

}