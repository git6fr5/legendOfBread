using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Container that handles being attacked
public class Hitbox : Container
{
    public Hitbox() {
        containerTags = new string[] {
            "Hitbox"
        };
    }

    public State state;

    /* --- COMPONENTS --- */

    /* --- VARIABLES --- */

    /* --- OVERRIDE --- */
    public override void OnAdd(Collider2D collider) {
        Hitbox otherHitbox = collider.gameObject.GetComponent<Hitbox>();
        state.controller.Hit(otherHitbox);
    }

}
