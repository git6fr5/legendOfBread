using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : Container
{

    public Dungeon dungeon;
    [HideInInspector] public int[] exitID = new int[] { 0, 0 };

    public Exit() {
        containerTags = new string[] { "Hitbox" };
    }


    /* --- COMPONENTS --- */

    /* --- VARIABLES --- */

    /* --- OVERRIDE --- */
    public override void OnAdd(Collider2D collider) {
        print(exitID[0].ToString() + ", " + exitID[1].ToString());

        int[] newID = new int[] { dungeon.roomID[0] + exitID[0], dungeon.roomID[1] + exitID[1] };
        dungeon.LoadRoom(newID);

        collider.transform.parent.transform.position = collider.transform.parent.transform.position + new Vector3(-exitID[1] * 7.5f, exitID[0] * 7.5f, 0);
    }
}
