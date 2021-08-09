﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Direction = Compass.Direction;

// Stores the state of a character
public class State : MonoBehaviour {

    /* --- COMPONENTS --- */
    // rendering
    [Space(5)]
    [Header("Renderer")]
    public Character character;
    // controls
    [Space(5)][Header("Controls")]
    public Controller controller;
    // collision
    [Space(5)]
    [Header("Collision")]
    public Rigidbody2D body;
    public Collider2D hitbox;

    /* --- VARIABLES --- */
    // health
    [Space(5)]
    [Header("Health")]
    public int maxHealth = 5;
    public int currHealth = 5;
    // states
    [Space(5)]
    [Header("Movement")]
    public Direction direction = Direction.RIGHT;
    public bool isMoving = false;
    public float moveSpeed = 0.5f;

    /* --- UNITY --- */
    void Start() {
    }

    void LateUpdate() {
        Render();
    }

    /* --- METHODS --- */
    void Render() {
        if (isMoving) {
            // if we're moving, get the direction and play the animation
            character.walkingAnimation.SetDirection(direction);
            character.PlayAnimation(character.walkingAnimation);
        }
        else {
            // stop any animations if necessary
            character.StopAnimation();
            character.SetDirectionIdle(direction);
        }

    }

}
