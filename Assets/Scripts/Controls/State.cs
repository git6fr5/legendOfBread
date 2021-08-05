using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Stores the state of a character
public class State : MonoBehaviour {

    public enum Direction {
        RIGHT,
        UP,
        LEFT,
        DOWN
    }

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

    void Update() {
        Render();
    }

    /* --- METHODS --- */
    void Render() {
        if (isMoving) {
            Animation2D animation = null;
            if ((int)direction < character.moveAnimations.Length) {
                animation = character.moveAnimations[(int)direction];
            }
            character.SetAnimation(animation);
        }
        else {
            Sprite sprite = null;
            if ((int)direction < character.directionSprites.Length) {
                sprite = character.directionSprites[(int)direction];
            }
            character.SetSprite(sprite);
        }
    }

}
