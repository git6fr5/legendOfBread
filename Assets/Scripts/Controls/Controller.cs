using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

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
    
    public virtual void Hit(Hitbox hit) {
        //
    }

}
