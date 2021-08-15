using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : Trap {

    public Spinner() {
        id = 2;
    }

    public int collisionDamage = 1;
    public float force = 0.25f;
    public float knockDuration = 0.125f;

    public int burnDamage = 1;
    public int burnTicks = 5;

    public Renderer2D[] fireballs;

    public override void IdleAction() {
        for (int i = 0; i < fireballs.Length; i++) {
            fireballs[i].PlayAnimation(fireballs[i].currAnimation);
        }
        actionState = ActionState.ACTIVE;
    }

    public override void Hit(Hitbox hitbox) {
        // do damage?
        if (hitbox.state.tag == playerTag) {
            hitbox.state.Hurt(collisionDamage);
            Vector3 direction = hitbox.state.transform.position - transform.position;
            hitbox.state.Knock(force, direction, knockDuration);
            hitbox.state.Burn(burnDamage, burnTicks);
        }

    }

}
