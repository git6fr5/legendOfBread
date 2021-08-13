using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Priority = Log.Priority;

public class Exit : MonoBehaviour {

    /* --- DEBUG --- */
    protected Priority debugPrio = Priority.COLLISION;
    protected string debugTag = "[EXIT]: ";

    /* --- COMPONENTS --- */

    // the dungeon
    public Dungeon dungeon;

    /* --- VARIABLES --- */
    
    // id
    [HideInInspector] public int[] exitID = new int[] { 0, 0 };

    // the player tag
    string playerTag = "Player";

    /* --- UNITY --- */
    void OnTriggerEnter2D(Collider2D collider) {
        Add(collider);
    }

    /* --- METHODS --- */
    void Add(Collider2D collider) {

        // add the item if it is in the container and has the correct tag
        if (collider.tag == tag && collider.GetComponent<Hitbox>() != null) {
            Hitbox hitbox = collider.GetComponent<Hitbox>();
            if (hitbox.state.tag == playerTag) {
                OnExit(hitbox);
            }
        }

    }

    public void OnExit(Hitbox hitbox) {
        Log.Write(hitbox.state.name + " has triggered the exit to " + Log.ID(exitID), debugPrio, debugTag);

        // get the state
        State hitboxState = hitbox.state;
        if (hitboxState.tag == playerTag) {

            // move the player
            Vector3 currPosition = hitbox.state.transform.position;

            // slightly different values along the x and y axis because of the rectangular shape
            // of the players hitbox
            Vector3 deltaPosition = new Vector3(-exitID[1] * 8.05f, exitID[0] * 7.95f, 0);
            hitbox.state.transform.position = currPosition + deltaPosition;

            // load the new room
            int[] newID = new int[] { dungeon.roomID[0] + exitID[0], dungeon.roomID[1] + exitID[1] };
            dungeon.LoadRoom(newID);
        }        

    }

}
