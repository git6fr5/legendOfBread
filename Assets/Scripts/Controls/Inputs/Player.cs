using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Controller
{
    /* --- VARIABLES --- */
    public Dictionary<KeyCode, Vector2> movementKeys = new Dictionary<KeyCode, Vector2>() {
        { KeyCode.W, Vector2.up },
        { KeyCode.A, -Vector2.right },
        { KeyCode.S, -Vector2.up },
        { KeyCode.D, Vector2.right }
    };
    public KeyCode lastPressedKey = KeyCode.W;


    /* --- OVERRIDE --- */
    public override void GetInput() {
        // move 
        movementVector = Vector2.zero;

        // get the last pressed key
        foreach (KeyValuePair<KeyCode, Vector2> movement in movementKeys) {
            if (Input.GetKeyDown(movement.Key)) {
                print("pressed a new key");
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
}
