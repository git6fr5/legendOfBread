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
    public Material hurtMaterial;
    public Material deathMaterial;

    void Awake() {
        movingAnimation.frameIndex = Random.Range(0, movingAnimation.frames.Length - 1);
        PlayAnimation(movingAnimation);
    }

    public override void Render(State state) {

        // Animation
        PlayAnimation(movingAnimation);
        SetDirection(state);


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

    public void SetDirection(State state) {
        if (state.direction == Direction.RIGHT) {
            state.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (state.direction == Direction.LEFT) {
            state.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

}
