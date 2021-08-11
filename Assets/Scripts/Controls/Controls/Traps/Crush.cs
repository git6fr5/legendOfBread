using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = Compass.Direction;

public class Crush : Trap {

    public Crush() {
        id = 1;
    }

    float idleTicks = 0f;
    float idleInterval = 1f;

    float activeTicks = 0f;
    float activeMaxInterval = 1f;

    float followSpeedMultiplier = 2f;
    float followDistance = 3f;

    Direction targetDirection = Direction.EMPTY;

    public override void OnThink() {

        movementVector = Vector2.zero;

        // resting state
        if (followState == FollowState.IDLE ) {
            GetTargetDirection();
            idleTicks += Time.deltaTime;
            if (idleTicks >= idleInterval) {
                idleTicks = 0f;
                _Activate();
            }
        }
        // following state
        else if (followState == FollowState.ACTIVE) {
            activeTicks += Time.deltaTime;
            if (targetDirection != Direction.EMPTY && Vector2.Distance(transform.position, origin) < followDistance && activeTicks < activeMaxInterval) {
                Charge();
            }
            else {
                activeTicks = 0f;
                Deactivate();
            }
        }
        // returning state
        else if (followState == FollowState.DEACTIVE) {
            if ((Vector2.Distance(transform.position, origin) > 0.01f)) {
                Withdraw();
            }
            else {
                transform.position = origin;
                idleTicks = 0f;
                followState = FollowState.IDLE;
            }
        }
        
    }

    void GetTargetDirection() {
        //targetDirection = Direction.EMPTY;
        //for (int i = 0; i < vision.visionContainer.Count; i++) {
        //    if (vision.visionContainer[i].state.tag == "Player") {
        //        Hitbox target = vision.visionContainer[i];
        //        targetDirection = Compass.VectorToCardinalDirection(transform.position - target.transform.position);
        //    }
        //}
    }

    void Charge() {
        movementVector = Compass.DirectionToVector(targetDirection);
    }

    void Withdraw() {
        state.direction = Compass.VectorToCardinalDirection(transform.position - origin);
        movementVector = Compass.DirectionToVector(state.direction);
    }

    //public override void Activate() {
    //    state._renderer.PlayAnimation(state._renderer.currAnimation);
    //}

    public void _Activate() {
        state._renderer.PlayAnimation(state._renderer.currAnimation);
        state.moveSpeed *= followSpeedMultiplier;
        followState = FollowState.ACTIVE;
    }

    public void Deactivate() {
        state.moveSpeed = state.moveSpeed / followSpeedMultiplier;
        followState = FollowState.DEACTIVE;
    }

    //void ChangeDirection() {
    //    int currDirectionIndex = Compass.ConvertCardinalToIndex(state.direction);
    //    int newDirectionIndex = (currDirectionIndex + 2) % 4;
    //    int newDirection = (int)Mathf.Pow(2, newDirectionIndex);
    //    state.direction = (Direction)newDirection;
    //}


}
