using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class Player : Controller {

    /* --- ENUMS --- */
    public enum ActionState {
        IDLE,
        MOVING,
        ATTACKING,
        DEAD,
    }

    public ActionState actionState;

    /* --- VARIABLES --- */
    public KeyCode attackKey = KeyCode.J;

    public Dictionary<KeyCode, Vector2> movementKeys = new Dictionary<KeyCode, Vector2>() {
        { KeyCode.W, Vector2.up },
        { KeyCode.A, -Vector2.right },
        { KeyCode.S, -Vector2.up },
        { KeyCode.D, Vector2.right }
    };
    public KeyCode lastPressedKey = KeyCode.W;

    [Range(0.05f, 1f)] public float attackMoveSlowMultiplier = 0.5f;

    /* --- OVERRIDE --- */
    public override void Think() {
        // reset movement 
        movementVector = Vector2.zero;
        moveSpeed = state.baseSpeed;

        // actions
        GetAttackInput();
        GetMoveInput();
        GetDirection();
    }


    /* --- METHODS --- */
    void GetAttackInput() {
        if (Input.GetKeyDown(attackKey) && !state.isAttacking) {
            attack = true;
            return;
        }
    }

    void GetMoveInput() {

        // get the speed of the player
        if (state.isAttacking) {
            moveSpeed = state.baseSpeed * attackMoveSlowMultiplier;
        }

        // get the last pressed key
        foreach (KeyValuePair<KeyCode, Vector2> movement in movementKeys) {
            if (Input.GetKeyDown(movement.Key)) {
                lastPressedKey = movement.Key;
            }
        }

        // prioritize the last pressed key
        if (Input.GetKey(lastPressedKey)) {
            movementVector = movementKeys[lastPressedKey];
            return;
        }

        // check through the other keys
        foreach (KeyValuePair<KeyCode, Vector2> movement in movementKeys) {
            if (Input.GetKey(movement.Key)) {
                movementVector = movement.Value;
                return;
            }
        }
    }

    void GetDirection() {
        if (state.isAttacking) { return; }

        if (movementVector.x == 1) {
            state.direction = Direction.RIGHT;
        }
        else if (movementVector.y == 1) {
            state.direction = Direction.UP;
        }
        else if (movementVector.x == -1) {
            state.direction = Direction.LEFT;
        }
        else if (movementVector.y == -1) {
            state.direction = Direction.DOWN;
        }
    }
}
