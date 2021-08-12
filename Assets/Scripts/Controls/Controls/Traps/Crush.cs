using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class Crush : Trap {

    public Crush() {
        id = 1;
    }

    float idleTicks = 0f;
    float idleInterval = 1f;

    float activeBuffer = 0f; 
    float resetBuffer = 0f;
    float maxIntervalBuffer = 3f;

    float activeSpeedMultiplier = 2f;
    float travelDistance = 3f;

    /* --- OVERRIDE --- */
    public override void IdleAction() {
        idleTicks += Time.deltaTime;
        if (idleTicks >= idleInterval) {
            actionState = ActionState.EXCITED;
            idleTicks = 0f;
        }
    }

    public override void ExcitedAction() {
        //
        actionState = ActionState.ACTIVE;
        Charge();
    }

    public override void ActiveAction() {
        //
        movementVector = Compass.DirectionToVector(state.direction);
        activeBuffer += Time.deltaTime;

        if (Vector2.Distance(transform.position, origin) >= travelDistance || activeBuffer >= maxIntervalBuffer) {
            actionState = ActionState.RESET;
            activeBuffer = 0f;
            Withdraw();
        }
    }

    public override void ResetAction() {

        movementVector = origin - transform.position;
        resetBuffer += Time.deltaTime;

        if (Vector2.Distance(transform.position, origin) < 0.01f || resetBuffer >= maxIntervalBuffer) {
            transform.position = origin;
            actionState = ActionState.IDLE;
            resetBuffer = 0f;
        }
    }

    /* --- METHODS --- */
    public void Charge() {
        state._renderer.PlayAnimation(state._renderer.currAnimation);
        state.moveSpeed *= activeSpeedMultiplier;
    }

    public void Withdraw() {
        state.moveSpeed = state.moveSpeed / activeSpeedMultiplier;
    }

}
