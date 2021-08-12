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

    /* --- VARIABLES --- */

    // id
    [Space(5)][Header("ID")]
    public int id = 0;
    
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

    public override void Hit(Hitbox hit) {
        // do damage?
    }

}
