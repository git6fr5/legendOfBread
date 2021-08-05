using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Layout : MonoBehaviour {

    public TileBase nullTile;
    public TileBase[] tiles;

    // input = LEFT_UP, UP, UP_RIGHT, LEFT_UP_RIGHT
    //          LEFT, CENTER, RIGHT, LEFT_RIGHT
    //          LEFT_DOWN, DOWN, RIGHT_DOWN, DOWN_LEFT_RIGHT
    //          DOWN_LEFT_UP, DOWN_UP, DOWN_UP_RIGHT, DOWN_LEFT_UP_RIGHT

    int[] inputOrder = new int[] {
        4+8, 8, 8+1, 4+8+1,
        4, 0, 1, 4+1,
        2+4, 2, 2+1, 2+4+1,
        2+4+8, 2+8, 2+8+1, 2+4+8+1
    };

    /* --- UNITY --- */
    // runs once on compilation

    // reorder the layouts to be compatible with the enum
    public void Organize() {
        List<TileBase> _tiles = new List<TileBase>();
        for (int i = 0; i < inputOrder.Length + 1; i++) {
            _tiles.Add(nullTile);
        }
        for (int i = 0; i < inputOrder.Length; i++) {
            _tiles[inputOrder[i] + 1] = tiles[i];
        }
        tiles = _tiles.ToArray();
    }

}
