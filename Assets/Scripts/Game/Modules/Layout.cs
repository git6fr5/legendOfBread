using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Layout : MonoBehaviour {

    /* --- Enums --- */
    // The ordered layout of the tiles.
    public enum Tiles {
        EMPTY,
        CENTER,
        RIGHT,
        UP, UP_RIGHT,
        LEFT, LEFT_RIGHT, LEFT_UP, LEFT_UP_RIGHT,
        DOWN, DOWN_RIGHT, DOWN_UP, DOWN_UP_RIGHT, DOWN_LEFT, DOWN_LEFT_RIGHT, DOWN_LEFT_UP,
        DOWN_LEFT_UP_RIGHT,
        tileCount
    };

    /* --- Components --- */
    // The given tiles
    [Space(5)][Header("Tiles")]
    public TileBase nullTile;
    public TileBase[] tiles;

    /* --- Variables --- */
    // The organization format.
    Tiles[] inputOrder = new Tiles[] {
        Tiles.LEFT_UP, Tiles.UP, Tiles.UP_RIGHT,
        Tiles.LEFT, Tiles.CENTER, Tiles.RIGHT,
        Tiles.DOWN_LEFT, Tiles.DOWN, Tiles.DOWN_RIGHT,
    };

    /* --- Methods --- */
    // Reorder the layouts to be compatible with the directional enum.
    public void SetOrder() {
        TileBase[] tempTiles = new TileBase[(int)Tiles.tileCount];
        tempTiles[0] = nullTile;
        for (int i = 0; i < inputOrder.Length; i++) {
            // Put the tile currently at "i", at the correct index
            int nextTileIndex = (int)inputOrder[i];
            tempTiles[nextTileIndex] = tiles[i];
        }
        tiles = tempTiles;

    }

    //// set the tiles to a given set of tiles
    //public void SetLayoutFromBrushes(Select[] selections, int maxID) {
    //    tiles = new TileBase[maxID + 1];
    //    foreach (Select selection in selections) {
    //        // create a tile from the selections sprite
    //        SpriteTile newTile = ScriptableObject.CreateInstance<SpriteTile>(); // Instantiate(nullTile);// create new tile
    //        newTile.newSprite = selection.GetComponent<SpriteRenderer>().sprite;
    //        // add that tile to the tile layout
    //        int index = selection.index;
    //        tiles[index] = newTile;
    //    }

    //}
}