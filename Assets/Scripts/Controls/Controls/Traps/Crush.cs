using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class Crush : Trap {

    public Crush() {
        id = 1;
    }

    public int collisionDamage = 1;
    public float force = 1f;
    public float knockDuration = 0.125f;

    public float activeSpeedMultiplier = 5f;
    public float travelDistance = 3f;

    float excitedTicks = 0f;
    float excitedInterval = 0.35f;

    float activeBuffer = 0f; 
    float resetBuffer = 0f;
    float maxIntervalBuffer = 2f;

    

    /* --- OVERRIDE --- */
    public override void IdleAction() {

        for (int i = 0; i < vision.container.Count; i++) {
            if (vision.container[i].tag == playerTag) {

                state.direction = Compass.VectorToCardinalDirection(transform.position - vision.container[i].transform.position);
                if (state.direction != Direction.EMPTY) {
                    actionState = ActionState.EXCITED;
                }

                return;
            }
        }
    }

    public override void ExcitedAction() {

        excitedTicks += Time.deltaTime;
        if (excitedTicks >= excitedInterval) {
            actionState = ActionState.ACTIVE;
            excitedTicks = 0f;
            Charge();
        }

    }

    public override void ActiveAction() {
        //
        movementVector = Compass.DirectionToVector(state.direction);
        moveSpeed = state.baseSpeed * activeSpeedMultiplier;
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

        if (Vector2.Distance(transform.position, origin) < 0.05f || resetBuffer >= maxIntervalBuffer) {
            transform.position = origin;
            actionState = ActionState.IDLE;
            resetBuffer = 0f;
        }
    }

    /* --- METHODS --- */
    public void Charge() {
        state._renderer.PlayAnimation(state._renderer.currAnimation);
        state.body.isKinematic = false;
        state.hitbox.Reset();
    }

    public void Withdraw() {
        state.body.isKinematic = true;
        state.hitbox.Reset();
    }

    public override void Hit(Hitbox hitbox) {
        // do damage?
        if (hitbox.state.tag == playerTag  && actionState == ActionState.ACTIVE) {
            hitbox.state.Hurt(collisionDamage);
            Vector3 direction = movementVector; // hitbox.state.transform.position - transform.position;
            hitbox.state.Knock(force, direction, knockDuration);

        }

        StartCoroutine(IEWithdraw(0.05f));

    }

    private IEnumerator IEWithdraw(float delay) {
        yield return new WaitForSeconds(delay);

        actionState = ActionState.RESET;
        activeBuffer = 0f;
        Withdraw();

        yield return null;
    }

}
