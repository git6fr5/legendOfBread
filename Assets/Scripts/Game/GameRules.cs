using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRules : MonoBehaviour {

    /* --- Presets --- */
    public static int PixelsPerUnit = 16;
    public static string Path = "Assets/Resources/";

    public static int gameTicks = 0;

    public static string RoomTag = "Room";
    public static string DungeonTag = "Dungeon";

    public static string PlayerTag = "Player";
    public static string HitboxTag = "Hitbox";
    public static string MeshTag = "Mesh";

    /* --- Unity --- */
    void Update() {
        Tick();
        MinimumSort();
    }

    /* --- Methods --- */
    void Tick() {
        gameTicks++;
    }

    public static int HashID(int seed, int[] id) {
        int primeFactorA = (int)Mathf.Pow(2, id[0]);
        int primeFactorB = (int)Mathf.Pow(3, id[1]);
        return Hash((seed + primeFactorA + primeFactorB) % 10);
    }

    public static int Hash(int val) {
        float fVal = (float)val;
        fVal = Mathf.Pow(3, fVal + 1) / Mathf.Pow(2, fVal + 1) * Mathf.Pow(7, fVal + 1) / Mathf.Pow(5, fVal + 1);
        fVal = (fVal % 1) + 1;
        val = (int)(fVal * 1e8);
        return val;
    }

    public static void MinimumSort() {
        // Declare the object array and the array of sorted characters
        GameObject[] unsortedObjects = GameObject.FindGameObjectsWithTag(meshTag);

        // assumes all the objects tagged with meshes have mesh components
        Mesh[] meshes = new Mesh[unsortedObjects.Length];
        for (int i = 0; i < unsortedObjects.Length; i++) {
            meshes[i] = unsortedObjects[i].GetComponent<Mesh>();
        }

        // the depth is understood as the position of the y axis
        Array.Sort<Mesh>(meshes, new Comparison<Mesh>((meshA, meshB) => Mesh.Compare(meshA, meshB)));
        for (int i = 0; i < meshes.Length; i++) {
            meshes[i]._renderer.spriteRenderer.sortingOrder = i;
        }
    }

}
