using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Container that handles being attacked
public class Hitbox : Container2D
{
    public Hitbox() {}

    /* --- COMPONENTS --- */

    /* --- VARIABLES --- */

    /* --- OVERRIDE --- */
    public override void OnAdd(Collider2D collider) {
    }

    /* --- METHODS --- */
    public void Hurt(int damage) {

    }

    public void Knockback(float magnitude, Vector2 direction) {

    }

}
