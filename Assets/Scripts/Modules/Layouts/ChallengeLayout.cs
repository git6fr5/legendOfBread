using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class ChallengeLayout : Layout {

    // problems might occur if the sprites being put in
    // are not in the proper order w/ respect to their id's

    public void SetChallengeLayout(Select[] selections) {

        // therefore we order sprites by their id's here 
        int selectionMaxIndex = 0;
        for (int i = 0; i < selections.Length; i++) {
            if (selections[i].index > selectionMaxIndex) {
                selectionMaxIndex = selections[i].index;
            }
        }

        SpriteRenderer[] sprites = new SpriteRenderer[selectionMaxIndex]; // should be the max selections index
        tiles = new TileBase[selectionMaxIndex + 1]; ;

        foreach (Select selection in selections) { 

            //
            int index = selection.index;

            // create a tile from the sprite
            ChallengeTile newTile = ScriptableObject.CreateInstance<ChallengeTile>(); // Instantiate(nullTile);// create new tile
            newTile.newSprite = selection.GetComponent<SpriteRenderer>().sprite;

            // add that tile to the tile layout
            tiles[index] = newTile;

        }

    }

}
