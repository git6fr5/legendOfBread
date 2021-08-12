using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class Player : Controller {
    /* --- VARIABLES --- */
    public KeyCode attackKey = KeyCode.J;

    public Dictionary<KeyCode, Vector2> movementKeys = new Dictionary<KeyCode, Vector2>() {
        { KeyCode.W, Vector2.up },
        { KeyCode.A, -Vector2.right },
        { KeyCode.S, -Vector2.up },
        { KeyCode.D, Vector2.right }
    };
    public KeyCode lastPressedKey = KeyCode.W;


    /* --- OVERRIDE --- */
    public override void Think() {
        // reset movement 
        movementVector = Vector2.zero;

        // actions
        GetInput();
        FaceDirection();

    }

    /* --- METHODS --- */
    void GetInput() {

        if (Input.GetKeyDown(attackKey)) {
            attack = true;
            return;
        }
        else if (state.isAttacking) {
            return;
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

    void FaceDirection() {
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
