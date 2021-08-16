using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class WeaponRenderer : Renderer2D
{
    public Animation2D attackAnimation;
    public Weapon weapon;

    public int attackIndex;



    public override void Render(State state) {
        if (state.isAttacking) {
            gameObject.SetActive(true);

            attackIndex = state._renderer.currAnimation.frameIndex % state._renderer.currAnimation.frameCount;
            print(attackAnimation.frameIndex);
            spriteRenderer.sprite = attackAnimation.frames[attackIndex];

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
