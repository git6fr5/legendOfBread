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
            gameObject.SetActive(true);
            attackAnimation.frameRate = state._renderer.currAnimation.frameRate;
            PlayAnimation(attackAnimation);
            SetRotation(state);
        }
        else {
            weapon.Reset();
            gameObject.SetActive(false);
        }
    }

    public void SetRotation(State state) {
        if (state.direction == Direction.RIGHT) {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (state.direction == Direction.UP) {
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (state.direction == Direction.LEFT) {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (state.direction == Direction.DOWN) {
            transform.localRotation = Quaternion.Euler(0, 0, 270);
        }
    }

}
