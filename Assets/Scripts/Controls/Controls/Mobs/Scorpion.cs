using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class Scorpion : Mob {


    static int syncTicks = 512;

    public int attackDamage = 1;
    public float force = 1f;
    public float knockDuration = 0.15f;

    Vector2 targetPoint;
    Vector2 targetOffset = new Vector3(0.5f, 0.5f);

    public Scorpion() {
        id = 2;
    }

    /* --- OVERRIDE --- */
    public override void IdleAction() {

        // syncs the scorpions up
        if (GameRules.gameTicks % syncTicks == 0) {
            actionState = ActionState.ACTIVE;
        }

    }

    public override void ActiveAction() {

        // working under the assumption that the room object can be found
        if (room == null) { return; }

        // operating under the assumption that we'll always be able to find a room
        if (targetPoint == Vector2.zero) {
            targetPoint = GetAdjacentPosition();
        }

        // if we're colliding with another scorpion
        if (GameRules.gameTicks % syncTicks == 0) {
            Hitbox ourHitbox = state.hitbox;
            for (int i = 0; i < ourHitbox.container.Count; i++) {
                if (ourHitbox.container[i].state.GetComponent<Scorpion>() != null) {
                    targetPoint = GetAdjacentPosition();
                }
            }
        }

        // if we reached the target position
        if (Vector2.Distance(targetPoint + targetOffset, (Vector2)transform.position) < 0.05f) {
            transform.position = (Vector3)(targetPoint + targetOffset);
            targetPoint = Vector3.zero;
            actionState = ActionState.IDLE;
        }
        else {
            movementVector = targetPoint + targetOffset - (Vector2)transform.position;
        }

    }

    public override void Hit(Hitbox hitbox) {

        // do damage?
        if (hitbox.state.tag == playerTag) {
            hitbox.state.Hurt(attackDamage);
            Vector3 direction = hitbox.state.transform.position - transform.position;
            hitbox.state.Knock(force, direction, knockDuration);
        }

        targetPoint = GetAdjacentPosition();

    }

    Vector2 GetAdjacentPosition() {

        // get our point in the room
        int[] point = room.PointToGrid(transform.position);
        
        // getting some coordinates
        int y = point[0];
        int x = point[1];
        int i = Random.Range(-1, 2);
        int j = Random.Range(-1, 2);

        if (y + i  > vertBounds[1] || y + i < vertBounds[0]) {
            i = -i;
        }
        if (x + j > horBounds[1] || x + j < horBounds[0]) {
            j = -j;
        }
        return (Vector2)(Vector3)room.GridToTileMap(y + i, x + j);

    }



}
