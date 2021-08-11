using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : Hitbox {

    // the dungeon
    public Dungeon dungeon;
    [HideInInspector] public int[] exitID = new int[] { 0, 0 };

    // the player tag
    string playerTag = "Player";

    /* --- OVERRIDE --- */
    public override void OnAdd(Hitbox hitbox) {
        Log.Write(hitbox.state.name + " has triggered the exit to " + Log.ID(exitID), debugPrio, debugTag);

        // get the state
        State hitboxState = hitbox.state;
        if (hitboxState.tag == playerTag) {

            // move the player
            Vector3 currPosition = hitbox.state.transform.position;
            Vector3 deltaPosition = new Vector3(-exitID[1] * 7.5f, exitID[0] * 7.5f, 0);
            hitbox.state.transform.position = currPosition + deltaPosition;

            // load the new room
            int[] newID = new int[] { dungeon.roomID[0] + exitID[0], dungeon.roomID[1] + exitID[1] };
            dungeon.LoadRoom(newID);
        }        

    }
}
