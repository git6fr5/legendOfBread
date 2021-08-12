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
    public float rotationSpeed = 0f;
    public bool attack = false;

    /* --- UNITY --- */  
    void Update() {
        Think();
    }

    void FixedUpdate() {
        Attack();
        Move();
        Rotate();
    }
    
    /* --- VIRTUAL --- */
    public virtual void Think() {      
        // get the input
    }

    void Attack() {
        if (attack) {
            state.isAttacking = true;
            attack = false;
        }
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

    void Rotate() {
        // this should just be a normalizable direction to rotate in
        // and the speed should come from the state
        if (rotationSpeed != 0f) {
            float deltaRotation = rotationSpeed * Time.fixedDeltaTime;
            state.transform.eulerAngles = state.transform.eulerAngles + Vector3.forward * deltaRotation;
        }
    }
    
    public virtual void Hit(Hitbox hit) {
        //
    }

    public virtual void See(State state, bool enteringVision) {
        //
    }

}
