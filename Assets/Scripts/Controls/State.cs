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
    // collision
    [Space(5)][Header("Collision")]
    public Rigidbody2D body;
    public Collider2D hitbox;
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
    public bool isDead = false;
    public float moveSpeed = 0.5f;

    /* --- UNITY --- */
    void Start() {
    }

    void LateUpdate() {
        _renderer.Render(this);
    }

    /* --- METHODS --- */
    public void Hurt(int damage) {
        currHealth -= damage;
        if (currHealth <= 0) {
            isDead = true;
            gameObject.SetActive(false);
        }
    }

    public void Knock(float magnitude, Vector2 direction, float duration) {
        if (controller.enabled && gameObject.activeSelf) {
            body.velocity = (magnitude * direction.normalized) / duration;
            controller.enabled = false;
            StartCoroutine(IEKnock(duration));
        }
    }

    private IEnumerator IEKnock(float delay) {
        yield return new WaitForSeconds(delay);

        body.velocity = Vector3.zero;
        controller.enabled = true;

        yield return null;
    }

}
