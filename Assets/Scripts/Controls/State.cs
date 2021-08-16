using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Direction = Compass.Direction;

// Stores the state of a character
public class State : MonoBehaviour {

    /* --- COMPONENTS --- */

    // rendering
    [Space(5)][Header("Renderer")]
    public Renderer2D _renderer;

    // controls
    [Space(5)][Header("Controls")]
    public Controller controller;
    public Inventory inventory;

    // collision
    [Space(5)][Header("Collision")]
    public Rigidbody2D body;
    public Hitbox hitbox;
    public Transform shadow;

    //
    public Weapon weapon;

    /* --- VARIABLES --- */
    // health
    [Space(5)][Header("Health")]
    public int maxHealth = 5;
    public int currHealth = 5;
    // states
    [Space(5)][Header("Movement")]
    public Direction direction = Direction.RIGHT;

    public bool isAttacking = false;
    public bool isMoving = false;
    public bool isBurning = false;
    public bool isHurt = false;
    public bool isDead = false;


    public float baseSpeed = 0.5f;
    float hurtBuffer = 0.4f;
    float deathBuffer = 0.6f;
    float burnBuffer = 1.0f;
    float burnKnockDistance = 0.15f;
    float burnKnockDuration = 0.025f;

    /* --- UNITY --- */
    void Start() {
    }

    void LateUpdate() {
        _renderer.Render(this);
    }

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

    public void Burn(int burnDamage, int ticks) {
        isBurning = true;
        StartCoroutine(IEBurn(burnBuffer, burnDamage, ticks));
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

    private IEnumerator IEBurn(float delay, int damage, int ticks) {

        yield return new WaitForSeconds(delay);
        for (int i = 0; i < ticks; i++) {
            print("burning");
            Hurt(damage);
            Vector2 directionVector = Compass.DirectionToVector(direction);
            directionVector.y = -directionVector.y;
            Knock(burnKnockDistance, directionVector, burnKnockDuration);
            if (!isDead) {
                yield return new WaitForSeconds(delay);
            }
            else {
                break;
            }
        }

        isBurning = false;
        yield return null;
    }


}
