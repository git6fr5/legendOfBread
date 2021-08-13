using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Layout : MonoBehaviour {

    // the layout tiles
    [Space(5)][Header("Tiles")]
    public TileBase nullTile;
    public TileBase[] tiles;

    /* --- VARIABLES --- */
    // an array of indices pointing to where the 
    // input tile should be in the organized list
    int[] directionalOrder = new int[] {
        4+8, 8, 8+1, 4+8+1,
        4, 0, 1, 4+1,
        2+4, 2, 2+1, 2+4+1,
        2+4+8, 2+8, 2+8+1, 2+4+8+1
    };

    /* --- METHODS --- */

    // reorder the layouts to be compatible with the directional enum
    public void SetDirectionalOrder() {
        List<TileBase> _tiles = new List<TileBase>();
        for (int i = 0; i < directionalOrder.Length + 1; i++) {
            _tiles.Add(nullTile);
        }
        for (int i = 0; i < directionalOrder.Length; i++) {
            _tiles[directionalOrder[i] + 1] = tiles[i];
        }
        tiles = _tiles.ToArray();
    }

    // set the tiles to a given set of tiles
    public void SetLayoutFromBrushes(Select[] selections, int maxID) {

        tiles = new TileBase[maxID + 1]; ;

        foreach (Select selection in selections) {

            //
            int index = selection.index;

            // create a tile from the sprite
            SpriteTile newTile = ScriptableObject.CreateInstance<SpriteTile>(); // Instantiate(nullTile);// create new tile
            newTile.newSprite = selection.GetComponent<SpriteRenderer>().sprite;

            // add that tile to the tile layout
            tiles[index] = newTile;
        }

    }
}