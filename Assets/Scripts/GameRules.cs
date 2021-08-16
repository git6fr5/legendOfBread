using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Priority = Log.Priority;

public class GameRules : MonoBehaviour {

    public static string debugTag = "[GameRules]: ";
    public static Priority debugPrio = Priority.LOW;

    public static int PixelsPerUnit = 16;
    public static string Path = "Assets/Resources/";

    public static int gameTicks;

    public static string meshTag = "Mesh";


    void Update() {
        print("game tick");
        gameTicks++;
        MinimumSort();
    }

    public static int HashID(int seed, int[] id) {
        int _1 = (int)Mathf.Pow(2, id[0]);
        int _2 = (int)Mathf.Pow(3, id[1]);
        return Hash((seed + _1 + _2) % 10);
    }

    public static int Hash(int val) {
        Log.WriteValue("Input seed: ", val, debugPrio, debugTag);
        float _val = (float)val;
        _val = Mathf.Pow(3, _val + 1) / Mathf.Pow(2, _val + 1) * Mathf.Pow(7, _val + 1) / Mathf.Pow(5, _val + 1); // * Mathf.Pow(3.9f, -_val);
        _val = (_val % 1) + 1;
        val = (int)(_val * 1e8);
        Log.WriteValue("Output seed: ", val, debugPrio, debugTag);
        return val;
    }

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
        Array.Sort<Mesh>(meshes, new Comparison<Mesh>((meshA, meshB) => Mesh.Compare(meshA, meshB)));
        for (int i = 0; i < meshes.Length; i++) {
            meshes[i]._renderer.spriteRenderer.sortingOrder = i;
        }
    }
}
