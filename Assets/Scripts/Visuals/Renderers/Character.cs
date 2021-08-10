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
    public MoveAnimation walkingAnimation;
    public MoveAnimation runningAnimation;

    public override void Render(State state) {
        if (state.isMoving) {
            // if we're moving, get the direction and play the animation
            walkingAnimation.SetDirection(state.direction);
            PlayAnimation(walkingAnimation);
        }
        else {
            // stop any animations if necessary
            StopAnimation();
            SetDirectionIdle(state.direction);
        }
    }

    public void SetDirectionIdle(Direction direction) {
        int directionIndex = Compass.ConvertCardinalToIndex(direction);
        Sprite sprite = directionSprites[directionIndex];
        SetSprite(sprite);
    }

}
