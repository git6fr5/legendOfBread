using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class WeaponRenderer : Renderer2D
{
    public Animation2D attackAnimation;
    public Weapon weapon;

    public override void Render(State state) {
        if (state.isAttacking) {
            // if we're moving, get the direction and play the animation
            weapon.gameObject.SetActive(true);
            // int index = state._renderer.currAnimation.frameIndex % state._renderer.currAnimation.frameCount;
            // SetSprite(attackAnimation.frames[index]);
            attackAnimation.frameRate = state._renderer.currAnimation.frameRate;
            SetRotation(state);
            PlayAnimation(attackAnimation);
        }
        else {
            // attackAnimation.Stop();
            // currAnimation = null;
            weapon.Reset();
            weapon.gameObject.SetActive(false);
        }
    }

    public void SetRotation(State state) {
        if (state.direction == Direction.RIGHT) {
            weapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (state.direction == Direction.UP) {
            weapon.transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (state.direction == Direction.LEFT) {
            weapon.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (state.direction == Direction.DOWN) {
            weapon.transform.localRotation = Quaternion.Euler(0, 0, 270);
        }
    }

}
