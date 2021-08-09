using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Directions = Compass.Direction;

public class MoveAnimation : Animation2D
{

    /* --- OVERRIDE --- */
    public override void SetLength() {
        frameCount = 4;
        frame = frames[startIndex];
    }

    /* --- METHODS --- */
    public void SetDirection(Directions direction) {
        // set the start index to the appropriate index for the direction being moved in
        startIndex = Compass.ConvertCardinalToIndex(direction) * frameCount;
        // if the current frame is not part of the correct animation
        // snap it back to the start of the correct animation
        if (frameIndex < startIndex || frameIndex >= startIndex + frameCount) {
            SnapToFrame(startIndex);
        }
    }

}
