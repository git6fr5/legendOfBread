using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class Slime : Mob {

    Vector2 targetPoint;

    public int attackDamage = 1;
    public float force = 1f;
    public float knockDuration = 0.15f;

    public bool isBig = true;
    public float growTime = 2f;
    public float growTicks = 0f;

    float moduloTime = 0f;
    float period = 1f;

    public Slime parentSlime;
    public Slime childSlime;


    public Slime() {
        id = 1;
    }

    /* --- OVERRIDE --- */
    public override void IdleAction() {
        print("Idle Action");
        for (int i = 0; i < vision.container.Count; i++) {
            if (vision.container[i].tag == playerTag) {
                actionState = ActionState.ACTIVE;
                return;
            }
        }

        Grow();
    }

    public override void ExcitedAction() {
        actionState = ActionState.ACTIVE;
        Grow();
    }

    public override void ActiveAction() {
        //
        moduloTime = (moduloTime + Time.deltaTime) % (period / (2 * Mathf.PI));
        moveSpeed = state.baseSpeed * Mathf.Pow(Mathf.Sin(moduloTime * Mathf.PI * 2 / period), 4);

        for (int i = 0; i < vision.container.Count; i++) {
            if (vision.container[i].tag == playerTag) {
                movementVector = vision.container[i].transform.position - transform.position;
                Grow();
                return;
            }
        }

        actionState = ActionState.IDLE;
        Grow();
        return;
    }

    void Grow() {
        if (!isBig) {
            growTicks += Time.deltaTime;
            if (growTicks > growTime) {
                Die();
                Instantiate(parentSlime.gameObject, transform.position, Quaternion.identity, transform);
            }
        }
    }

    public override void Hit(Hitbox hitbox) {
        // do damage?
        if (hitbox.state.tag == playerTag) {
            hitbox.state.Hurt(attackDamage);
            Vector3 direction = hitbox.state.transform.position - transform.position;
            hitbox.state.Knock(force, direction, knockDuration);
        }

    }

    public override void Die() {

        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        if (isBig) {
            // need to add this to the list of cached room objects!!!
            state.isDead = false;
            // because we can't just turn off the gameObject
            // since leaving and returning will just turn it back on
            for (int i = 0; i < 2; i++) {
                Instantiate(childSlime.gameObject, transform.position + (Vector3)Random.insideUnitCircle * 0.5f, Quaternion.identity, transform);
            }
        }

        // diable the controller and state
        state.direction = Direction.RIGHT;
        state._renderer.gameObject.GetComponent<MobRenderer>().SetDirection(state);
        state.enabled = false;
        enabled = false;
    }

}
