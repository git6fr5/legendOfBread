using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class Spark : Trap
{

    public Spark() {
        id = 3;
    }

    public int collisionDamage = 1;
    public float force = 1f;
    public float knockDuration = 0.125f;

    float currTime;
    float maxTime = 7f;

    public Transform wallCheck;

    public int boundRight;
    public int boundLeft;
    public int boundUp;
    public int boundDown;

    public Room room;

    public override void IdleAction() {

        // working under the assumption that the room object can be found
        room = GameObject.FindWithTag("Room").GetComponent<Room>();

        int[] gridPoint = room.PointToGrid(transform.position);
        print(Log.ID(gridPoint));

        int sizeHor = room.sizeHorizontal;
        int sizeVert = room.sizeVertical;
        int boundHor = room.borderHorizontal;
        int boundVert = room.borderVertical;

        int minGrid = Mathf.Min(gridPoint[0], gridPoint[1]) - 1;
        int maxGrid = Mathf.Max(gridPoint[0], gridPoint[1]) + 1;

        // for square motion (and assuming room is a square)
        if (sizeHor - 1 - minGrid > maxGrid) { maxGrid = sizeHor - minGrid - 1; }
        if (sizeHor - maxGrid - 1 < minGrid) { minGrid = sizeHor - maxGrid - 1; }

        // set the "circle" radius
        //boundRight = sizeHor - (boundHor / 2) - 1;
        //boundLeft = (boundHor / 2);
        //boundUp = sizeVert - (boundVert / 2) - 1;
        //boundDown = (boundVert / 2);

        boundRight = maxGrid;
        boundLeft = minGrid;
        boundUp = maxGrid;
        boundDown = minGrid;

        // figure out the first direction to go in
        int midHor = sizeHor / 2;
        int midVert = sizeVert / 2;

        // if the distance to the horizontal mid point is smaller than the distance to the
        // vertical midpoint, then we want to move horizontally
        // and vice versa
        if (Mathf.Abs(midHor - gridPoint[1]) <= Mathf.Abs(midVert - gridPoint[0])) {
            // if moving horizontally, and in the bottom half of the room
            // we move left

            // i think positive y = down? it's getting confusing
            if (midVert - gridPoint[0] < 0) {
                state.direction = Direction.RIGHT;
            }
            else {
                state.direction = Direction.LEFT;
            }

        }
        else {
            if (midHor - gridPoint[1] < 0) {
                state.direction = Direction.DOWN;
            }
            else {
                state.direction = Direction.UP;
            }

        }

        movementVector = Compass.DirectionToVector(state.direction);
        wallCheck.localPosition = movementVector.normalized / 2f;


        actionState = ActionState.ACTIVE;
    }

    public override void ActiveAction() {

        // working under the assumption that the room object can be found
        bool changeDir = false;
        if (room != null) {
            int[] gridPoint = room.PointToGrid(wallCheck.position);
            if (gridPoint[1] >= boundRight || gridPoint[1] <= boundLeft || gridPoint[0] >= boundUp || gridPoint[0] <= boundDown) {
                changeDir = true;
            }
        }

        currTime += Time.deltaTime;
        if (changeDir || currTime >= maxTime) {
            int cardIndex = Compass.ConvertCardinalToIndex(state.direction);
            state.direction = Compass.ConvertIndexToCardinal(cardIndex+1);
            currTime = 0f;
        }

        state._renderer.PlayAnimation(state._renderer.currAnimation);
        movementVector = Compass.DirectionToVector(state.direction);
        wallCheck.localPosition = movementVector.normalized / 2f;

        origin = transform.position;

        // moveSpeed = state.baseSpeed / 10f;

    }

    public override void Hit(Hitbox hitbox) {
        // do damage?
        if (hitbox.state.tag == playerTag && actionState == ActionState.ACTIVE) {
            hitbox.state.Hurt(collisionDamage);
            Vector3 direction = new Vector3(movementVector.y, -movementVector.x, 0f); // hitbox.state.transform.position - transform.position;
            hitbox.state.Knock(force, direction, knockDuration);
            state.isDead = true;
            gameObject.SetActive(false);
        }
    }

}
