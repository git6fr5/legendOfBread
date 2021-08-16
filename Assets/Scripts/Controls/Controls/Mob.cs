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

    // the room its in
    protected Room room;
    protected int[] vertBounds = new int[2];
    protected int[] horBounds = new int[2];


    /* --- UNITY --- */
    void Awake() {
        origin = transform.position;
    }

    void Start() {
        room = GetRoom();
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

    protected Room GetRoom() {
        Room _room = GameObject.FindWithTag("Room").GetComponent<Room>();

        if (_room != null) {
            // set the bounds (add 1 for padding because its with respect to the center of the object)
            horBounds[1] = (int)(_room.sizeHorizontal - (_room.borderHorizontal / 2) - 2);
            horBounds[0] = (int)(_room.borderHorizontal / 2) + 1;
            vertBounds[1] = (int)(_room.sizeVertical - (_room.borderVertical / 2) - 2);
            vertBounds[0] = (int)(_room.borderVertical / 2) + 1;
        }

        return _room;
    }

    public override void Die() {
        DeathAction();
        Drop();
        gameObject.SetActive(false);
    }

    protected void Drop() {
        Dungeon dungeon = GameObject.FindWithTag("Dungeon").GetComponent<Dungeon>();

        if (item != null) {
            GameObject itemObject = Instantiate(item.gameObject, transform.position, Quaternion.identity);
            itemObject.SetActive(true);
            if (dungeon != null) {
                dungeon.AddNewObject(itemObject);
            }
        }

    }

}
