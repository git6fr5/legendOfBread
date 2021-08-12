using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Mob {

    Vector2 targetPoint;

    public int attackDamage = 1;
    public float force = 1f;
    public float knockDuration = 0.15f;

    public Slime() {
        id = 1;
    }

    /* --- OVERRIDE --- */
    public override void IdleAction() {
        print("Idle Action");
        for (int i = 0; i < vision.container.Count; i++) {
            if (vision.container[i].tag == playerTag) {
                actionState = ActionState.ACTIVE;
                return;
            }
        }
    }

    public override void ExcitedAction() {
        actionState = ActionState.ACTIVE;
    }

    public override void ActiveAction() {
        //
        for (int i = 0; i < vision.container.Count; i++) {
            if (vision.container[i].tag == playerTag) {
                movementVector = vision.container[i].transform.position - transform.position;
                return;
            }
        }

        actionState = ActionState.IDLE;
        return;
    }

    public override void DeathAction() {

    }

    public override void Hit(Hitbox hitbox) {
        // do damage?
        if (hitbox.state.tag == playerTag) {
            hitbox.state.Hurt(attackDamage);
            Vector3 direction = hitbox.state.transform.position - transform.position;
            hitbox.state.Knock(force, direction, knockDuration);
        }

    }

}
