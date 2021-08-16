using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Direction = Compass.Direction;

// Stores the state of a character
public class State : MonoBehaviour {

    /* --- COMPONENTS --- */

    // controls
    [Space(5)][Header("Controls")]
    public Mesh mesh;
    public Controller controller;

    // collision
    [Space(5)][Header("Collision")]
    public Rigidbody2D body;
    public Hitbox hitbox;

    /* --- VARIABLES --- */

    // values
    public int maxHealth = 5;
    public int currHealth = 5;
    public float baseSpeed = 0.5f;
    public Direction direction = Direction.RIGHT;

    // switches
    public bool isAttacking = false;
    public bool isMoving = false;
    public bool isHurt = false;
    public bool isDead = false;

    // buffers
    float hurtBuffer = 0.4f;
    float deathBuffer = 0.6f;

    /* --- METHODS --- */

    public void Hurt(int damage) {
        currHealth -= damage;
        // die if health goes before zero
        if (currHealth <= 0 && !isDead) {
            Death();
        }
        // otherwise simply take damage
        else {
            isHurt = true;
            StartCoroutine(IEHurt(hurtBuffer));
        }

    }

    public void Death() {
        isDead = true;
        // disable the hibox
        hitbox.gameObject.SetActive(false);
        // disable the mesh
        body.isKinematic = true;
        // disable the controller
        controller.enabled = false;

        // set the state to death, and at the end of the buffer
        // call the controller to die
        StartCoroutine(IEDead(deathBuffer));
    }

    public void Knock(float magnitude, Vector2 direction, float duration) {
      
        // controller.Push()
        // maybe this should be called stun?
    }


    /* --- COROUTINES --- */

    private IEnumerator IEKnock(float delay) {
        yield return new WaitForSeconds(delay);

        body.velocity = Vector3.zero;
        controller.enabled = true;

        yield return null;
    }

    private IEnumerator IEHurt(float delay) {
        yield return new WaitForSeconds(delay);

        isHurt = false;

        yield return null;
    }

    private IEnumerator IEDead(float delay) {
        yield return new WaitForSeconds(delay);

        controller.enabled = true;
        controller.Die();

        yield return null;
    }


}
