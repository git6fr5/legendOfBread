using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class Mob : Controller {

    public enum ActionState {
        IDLE,
        EXCITED,
        ACTIVE,
        DEATH,
    }

    /* --- COMPONENTS --- */
    public string playerTag = "Player";
    public Vision vision;

    /* --- VARIABLES --- */
    
    // id
    [Space(5)][Header("ID")]   
    public int id = 0;

    // the action state
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
                break;
            case ActionState.EXCITED:
                ExcitedAction();
                break;
            case ActionState.ACTIVE:
                ActiveAction();
                break;
            case ActionState.DEATH:
                DeathAction();
                break;
            default:
                break;
        }

        FaceDirection();
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

    public virtual void DeathAction() {
        //
    }

    public override void See(State state, bool enteringVision) {
        //if (enteringVision && state.tag == playerTag) {
        //    actionState = ActionState.EXCITED;
        //}
        //else if (!enteringVision && state.tag == playerTag) {
        //    actionState = ActionState.IDLE;
        //}
    }

    public override void Hit(Hitbox hit) {
        // do damage?
    }

    void FaceDirection() {
        if (movementVector.x >= 0) {
            state.direction = Direction.RIGHT;
        }
        else if (movementVector.x < 0) {
            state.direction = Direction.LEFT;
        }
    }

}
