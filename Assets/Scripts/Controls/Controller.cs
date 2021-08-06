using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = State.Direction;

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
    public Vector2 movementVector = Vector2.zero;

    /* --- UNITY --- */
    void Start() { 
    }
    
    void Update() {
        GetInput();
    }

    void FixedUpdate() {
        FaceDirection();
        Move();
    }
    
    /* --- VIRTUAL --- */
    public virtual void GetInput() {      
        // get the input
    }

    void Move() {
        // Apply the movement
        if (movementVector != Vector2.zero) {
            Vector2 deltaPosition = movementVector.normalized * GameRules.PixelsPerUnit * state.moveSpeed * Time.fixedDeltaTime;
            state.transform.position = state.transform.position + (Vector3)deltaPosition;
            state.isMoving = true;
        }
        else {
            state.isMoving = false;
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
