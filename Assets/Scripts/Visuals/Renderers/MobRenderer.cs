using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class MobRenderer : Renderer2D {

    /* --- VARIABLES --- */
    // animations
    [Space(5)]
    [Header("Animations")]
    public Animation2D movingAnimation;

    public override void Render(State state) {
        if (state.isMoving) {
            // if we're moving, get the direction and play the animation
            PlayAnimation(movingAnimation);
        }
        else {
            // stop any animations if necessary
            SetSprite(defaultSprite);
            StopAnimation();
        }

        SetDirection(state);
    }

    public void SetDirection(State state) {
        if (state.direction == Direction.RIGHT) {
            state.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (state.direction == Direction.LEFT) {
            state.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

}
