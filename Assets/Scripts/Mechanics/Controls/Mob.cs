using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class Mob : Controller {

    public enum ActionState {
        IDLE,
        EXCITED,
        ACTIVE,
    }

    /* --- COMPONENTS --- */
    public string playerTag = "Player";
    public Vision vision;

    public Item item;

    /* --- VARIABLES --- */

    // the action state
    public ActionState actionState;

    // caches the original location of this trap
    public Vector3 origin;

    /* --- UNITY --- */
    void Awake() {
        origin = transform.position;
    }

    void Start() {
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
                break;
            case ActionState.EXCITED:
                ExcitedAction();
                break;
            case ActionState.ACTIVE:
                ActiveAction();
                break;
            default:
                break;
        }

        GetDirection();
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

    void GetDirection() {
        if (movementVector.x >= 0) {
            state.direction = Direction.RIGHT;
        }
        else if (movementVector.x < 0) {
            state.direction = Direction.LEFT;
        }
    }

    public override void Die() {
        DeathAction();
        Drop();
        gameObject.SetActive(false);
    }

    protected void Drop() {
        if (item != null) {
            GameObject itemObject = Instantiate(item.gameObject, transform.position, Quaternion.identity);
            itemObject.SetActive(true);
            if (dungeon != null) {
                dungeon.AddNewObject(itemObject);
            }
        }

    }

}
