using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Controller {

    public enum ActionState {
        IDLE,
        EXCITED,
        ACTIVE,
        RESET,
    }

    /* --- COMPONENTS --- */
    public string playerTag = "Player";
    public Vision vision;

    /* --- VARIABLES --- */
    // action state
    public ActionState actionState;
    
    // caches the original location of this trap
    public Vector3 origin;


    /* --- UNITY --- */
    void Awake() {
        origin = transform.position;
    }

    /* --- METHODS --- */
    public override void Think() {

        // reset the movement
        movementVector = Vector2.zero;
        moveSpeed = state.baseSpeed;

        // take an action based on the state
        switch (actionState) {
            case ActionState.IDLE:
                IdleAction();
                return;
            case ActionState.EXCITED:
                ExcitedAction();
                return;
            case ActionState.ACTIVE:
                ActiveAction();
                return;
            case ActionState.RESET:
                ResetAction();
                return;
            default:
                return;
        }

    }

    public virtual void IdleAction() {
        //
    }

    public virtual void ExcitedAction() {
        //
    }

    public virtual void ActiveAction() {
        //
    }

    public virtual void ResetAction() {
        //
    }

}
