using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class Slime : Mob {

    Vector2 targetPoint;

    public int attackDamage = 1;
    public float force = 1f;
    public float knockDuration = 0.15f;

    // slime splitting mechanic
    public Slime parentSlime;
    public Slime childSlime;
    public bool isBig = true;
    public float growTime = 2f;
    public float growTicks = 0f;

    // motion controls
    float moduloTime = 0f;
    float period = 1f;

    public Slime() {
        id = 1;
    }

    /* --- OVERRIDE --- */

    public override void IdleAction() {

        // looks for a player to chase
        for (int i = 0; i < vision.container.Count; i++) {
            if (vision.container[i].tag == playerTag) {
                actionState = ActionState.ACTIVE;
                return;
            }
        }

    }

    public override void ActiveAction() {
        
        // if this slime is cactive then make it get older
        GetOlder();

        // adds a little sinusoidal motion to the slime
        moduloTime = (moduloTime + Time.deltaTime) % (period / (2 * Mathf.PI));
        moveSpeed = state.baseSpeed * Mathf.Pow(Mathf.Sin(moduloTime * Mathf.PI * 2 / period), 4);

        // finds a player to chase after
        for (int i = 0; i < vision.container.Count; i++) {
            if (vision.container[i].tag == playerTag) {
                movementVector = vision.container[i].transform.position - transform.position;
                return;
            }
        }

        // if no players were found
        actionState = ActionState.IDLE;

    }

    public override void Hit(Hitbox hitbox) {
        // do damage?
        if (hitbox.state.tag == playerTag) {
            hitbox.state.Hurt(attackDamage);
            Vector3 direction = hitbox.state.transform.position - transform.position;
            hitbox.state.Knock(force, direction, knockDuration);
        }

    }

    public override void DeathAction() {

        if (isBig) {

            Dungeon dungeon = GameObject.FindWithTag("Dungeon").GetComponent<Dungeon>();

            for (int i = 0; i < 2; i++) {
                GameObject newSlimeObject = Instantiate(childSlime.gameObject, transform.position + (Vector3)Random.insideUnitCircle * 0.5f, Quaternion.identity);
                if (dungeon != null) {
                    dungeon.AddNewObject(newSlimeObject);
                }
            }
        }

    }


    void GetOlder() {
        if (!isBig) {
            growTicks += Time.deltaTime;
            if (growTicks > growTime) {

                Dungeon dungeon = GameObject.FindWithTag("Dungeon").GetComponent<Dungeon>();

                GameObject newSlimeObject = Instantiate(parentSlime.gameObject, transform.position + (Vector3)Random.insideUnitCircle * 0.5f, Quaternion.identity);
               
                if (dungeon != null) {
                    dungeon.AddNewObject(newSlimeObject);
                }

                Destroy(gameObject);

            }
        }
    }

}
