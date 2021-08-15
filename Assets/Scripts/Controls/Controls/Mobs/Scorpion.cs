using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class Scorpion : Mob {

    Vector2 targetPoint;

    static int syncTicks = 512;

    public int attackDamage = 1;
    public float force = 1f;
    public float knockDuration = 0.15f;

    public int boundRight;
    public int boundLeft;
    public int boundUp;
    public int boundDown;

    public Room room;

    //float idleTicks;
    //float idleInterval = 3f;
    Vector3 targetPos = Vector3.zero;

    public float aggroSpeed = 1.5f;

    public Scorpion() {
        id = 2;
    }

    /* --- OVERRIDE --- */
    public override void IdleAction() {

        //for (int i = 0; i < vision.container.Count; i++) {
        //    if (vision.container[i].tag == playerTag) {
        //        actionState = ActionState.ACTIVE;
        //        return;
        //    }
        //}

        //idleTicks += Time.deltaTime;
        //if (idleTicks >= idleInterval) {
        //    actionState = ActionState.EXCITED;
        //    idleTicks = 0f;
        //    return;
        //}

        // syncs the scorpions up
        if (GameRules.gameTicks % syncTicks == 0) {
            actionState = ActionState.EXCITED;
        }

    }

    public override void ExcitedAction() {

        // working under the assumption that the room object can be found
        if (room == null) {
            GetRoomBounds();
        }

        // operating under the assumption that we'll always be able to find a room

        if (targetPos == Vector3.zero) {
            int[] gridPoint = room.PointToGrid(transform.position);

            int i = Random.Range(gridPoint[0] - 1, gridPoint[0] + 2);
            int j = Random.Range(gridPoint[1] - 1, gridPoint[1] + 2);

            if (i > boundUp || i < boundDown) {
                print("reset i");
                i = gridPoint[0];
            }
            if (j < boundLeft || j > boundRight) {
                print("reset j");
                j = gridPoint[1];
            }

            //int i = Random.Range(boundUp, boundDown);
            //int j = Random.Range(boundLeft, boundRight);

            targetPos = (Vector3)room.GridToTileMap(i, j);
            print(Log.ID(new int[] { i, j }));
            print(targetPos);
        }

        Vector3 pos = transform.position - new Vector3(0.5f, 0.5f, 0);

        // if we reached the target position
        if (Vector2.Distance(targetPos, pos) < 0.05f) {
            transform.position = targetPos + new Vector3(0.5f, 0.5f, 0);
            targetPos = Vector3.zero;
            actionState = ActionState.IDLE;
        }
        else {
            movementVector = targetPos - pos;
            if (movementVector.x > 0f) {
                state.direction = Direction.RIGHT;
            }
            else {
                state.direction = Direction.LEFT;
            }
        }

    }


    public override void ActiveAction() {

        //
        moveSpeed = state.baseSpeed * aggroSpeed;

        for (int i = 0; i < vision.container.Count; i++) {
            if (vision.container[i].tag == playerTag) {
                movementVector = vision.container[i].transform.position - transform.position;
                return;
            }
        }

        actionState = ActionState.IDLE;

        return;
    }

    public override void Hit(Hitbox hitbox) {
        // do damage?
        if (hitbox.state.tag == playerTag) {
            hitbox.state.Hurt(attackDamage);
            Vector3 direction = hitbox.state.transform.position - transform.position;
            hitbox.state.Knock(force, direction, knockDuration);
        }

        targetPos = Vector3.zero;

    }

    void GetRoomBounds() {
        room = GameObject.FindWithTag("Room").GetComponent<Room>();

        // room bounds
        int sizeHor = room.sizeHorizontal;
        int sizeVert = room.sizeVertical;
        int boundHor = room.borderHorizontal;
        int boundVert = room.borderVertical;

        // set the bounds (add 1 for padding because its with respect to the center of the object)
        boundRight = sizeHor - (boundHor / 2) - 2;
        boundLeft = (boundHor / 2) + 1;
        boundUp = sizeVert - (boundVert / 2) - 2;
        boundDown = (boundVert / 2) + 1;
    }

}
