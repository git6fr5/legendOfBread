using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class Character : Renderer2D
{
    /* --- VARIABLES --- */
    // animations
    [Space(5)] [Header("Animations")]
    public Sprite[] directionSprites;
    public DirectionalAnimation walkingAnimation;
    public DirectionalAnimation attackAnimation;

    public Material hurtMaterial;
    public Material deathMaterial;

    public override void Render(State state) {

        // Animation
        if (state.isAttacking) {
            if (currAnimation == attackAnimation && !currAnimation.isPlaying) {
                currAnimation = walkingAnimation;
                state.isAttacking = false;
            }
            else {
                attackAnimation.SetDirection(state.direction);
                PlayAnimation(attackAnimation);
                if (state.weapon != null) {
                }
            }
            state.weapon._renderer.Render(state);
        }
        else if (state.isMoving) {
            // if we're moving, get the direction and play the animation
            walkingAnimation.SetDirection(state.direction);
            PlayAnimation(walkingAnimation);
        }
        else {
            // stop any animations if necessary
            StopAnimation();
            SetDirectionIdle(state.direction);
        }

        // Material
        if (state.isDead) {
            SetMaterial(deathMaterial);
        }
        else if (state.isHurt) {
            SetMaterial(hurtMaterial);
        }
        else {
            SetMaterial(null);
        }

    }

    public void SetDirectionIdle(Direction direction) {
        int directionIndex = Compass.ConvertCardinalToIndex(direction);
        Sprite sprite = directionSprites[directionIndex];
        SetSprite(sprite);
    }

}
