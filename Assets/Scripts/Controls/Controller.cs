using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gathers the inputs and affects the state of the character based on the inputs
public class Controller : MonoBehaviour
{
    /* --- DEBUG --- */
    public bool doDebug = false;
    protected string debugTag = "[Controller]: ";

    /* --- COMPONENTS --- */
    public State state;

    /* --- VARIABLES --- */
    // horizontal
    public int horizontalMove = 0;
    public int verticalMove = 0;

    /* --- UNITY --- */
    void Start() { 
    }

    void FixedUpdate() {
        GetInput();
        FaceDirection();
        Move();
    }
    
    /* --- VIRTUAL --- */
    public virtual void GetInput() {      
        // get the input
    }

    void Move() {
        // Apply the movement
        if (horizontalMove != 0 || verticalMove != 0) {
            Vector2 deltaPosition = (new Vector2(horizontalMove, verticalMove).normalized) * GameRules.PixelsPerUnit * state.moveSpeed * Time.fixedDeltaTime;
            state.transform.position = state.transform.position + (Vector3)deltaPosition;
            state.isMoving = true;
        }
        else {
            state.isMoving = false;
        }
    }

    void FaceDirection() {
        if (horizontalMove == 1) {
            state.direction = State.Direction.RIGHT;
        }
        else if (horizontalMove == -1) {
            state.direction = State.Direction.LEFT;
        }
        else if (verticalMove == 1) {
            state.direction = State.Direction.UP;
        }
        else if (verticalMove == -1) {
            state.direction = State.Direction.DOWN;
        }
    }

}
