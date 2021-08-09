using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class Crush : Trap {

    public Direction currDirection = Direction.RIGHT;

    public override void OnThink() {
        ChangeDirection();
        movementVector = Compass.DirectionToVector(state.direction);
    }

    void ChangeDirection() {
        print("Changing Direction");
        int currDirectionIndex = Compass.ConvertCardinalToIndex(state.direction);
        int newDirectionIndex = (currDirectionIndex + 2) % 4;
        int newDirection = (int)Mathf.Pow(2, newDirectionIndex);
        state.direction = (Direction)newDirection;
    }

    public override void Activate() {
        print("Activating");
        state._renderer.PlayAnimation(state._renderer.currAnimation);
    }

}
