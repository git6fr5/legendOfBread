using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Tiles = Room.Tiles;

public class Layout : MonoBehaviour {

    // the layout tiles
    [Space(5)][Header("Tiles")]
    public TileBase nullTile;
    public TileBase[] tiles;

    /* --- VARIABLES --- */
    // an array of indices pointing to where the 
    // input tile should be in the organized list

    Tiles[] inputOrder = new Tiles[] {
        // the outer corners
        Tiles.LEFT_UP, Tiles.UP, Tiles.UP_RIGHT, // left_top, 
        Tiles.LEFT, Tiles.CENTER, Tiles.RIGHT,
        Tiles.DOWN_LEFT, Tiles.DOWN, Tiles.DOWN_RIGHT,
    };

    /* --- METHODS --- */

    // reorder the layouts to be compatible with the directional enum
    public void SetOrder() {

        TileBase[] _tiles = new TileBase[(int)Tiles.tileCount];
        _tiles[0] = nullTile;

        for (int i = 0; i < inputOrder.Length; i++) {

            Tiles nextTile = inputOrder[i];
            int nextTileIndex = (int)nextTile;

            _tiles[nextTileIndex] = tiles[i];

        }

        tiles = _tiles;

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