using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {

    /* --- Components --- */
    [Space(5)][Header("Dungeon")] 
    public Dungeon dungeon;

    /* --- Variables --- */
    int[] id = new int[] { 0, 0 };
    string playerTag = "Player";

    /* --- Unity --- */

    // called when the attached collider intersects with another collider
    void OnTriggerEnter2D(Collider2D collider) {
        ScanExit(collider);
    }

    /* --- Methods --- */
    void ScanExit(Collider2D collider) {
        if (collider.GetComponent<Hitbox>() != null) {
            Hitbox hitbox = collider.GetComponent<Hitbox>();
            if (hitbox.state.tag == playerTag) {
                OnExit(hitbox);
            }
        }
    }

    // if colliding with a players hitbox, then exit
    void OnExit(Hitbox hitbox) {

        // move the player
        Vector3 currPosition = hitbox.state.transform.position;

        // slightly different values along the x and y axis because of the rectangular shape
        // of the players hitbox
        Vector3 deltaPosition = new Vector3(-id[1] * 8.35f, id[0] * 8.35f, 0);
        hitbox.state.transform.position = currPosition + deltaPosition;
        hitbox.state.controller.movementVector = Vector2.zero;

        // load the new room
        int[] newID = new int[] { dungeon.roomID[0] + id[0], dungeon.roomID[1] + id[1] };
        dungeon.LoadRoom(newID);

    }

}
