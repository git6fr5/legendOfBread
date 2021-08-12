using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Mob {

    Vector2 targetPoint;

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

}
