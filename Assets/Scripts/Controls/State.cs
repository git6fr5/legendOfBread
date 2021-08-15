﻿using System.Collections;
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
    public Hitbox hitbox;
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
        // if (!isHurt) {
        currHealth -= damage;
        if (currHealth <= 0 && !isDead) {
            Death();
        }
        else {
            isHurt = true;
            StartCoroutine(IEHurt(hurtBuffer));
        }
        // }
    }

    public void Death() {
        isDead = true;
        body.isKinematic = true;
        hitbox.gameObject.SetActive(false);
        controller.enabled = false;
        StartCoroutine(IEDead(deathBuffer));
    }

    public void Knock(float magnitude, Vector2 direction, float duration) {
        if (controller.enabled && gameObject.activeSelf) {
            body.velocity = (magnitude * direction.normalized) / duration;
            controller.enabled = false;
            StartCoroutine(IEKnock(duration));
        }
    }

    public void Burn(int burnDamage, int ticks) {
        isBurning = true;
        StartCoroutine(IEBurn(burnBuffer, burnDamage, ticks));
    }

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
