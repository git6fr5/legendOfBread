using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Directions = Compass.Direction;

public class DirectionalAnimation : Animation2D
{

    public int _frameCount = 4;

    /* --- OVERRIDE --- */
    public override void Play() {
        frameIndex = startIndex + 1;
        timer = (0.5f / frameRate);
        isPlaying = true;
    }

    public override void SetLength() {
        frameCount = _frameCount;
        frame = frames[startIndex];
    }

    /* --- METHODS --- */
    public void SetDirection(Directions direction) {

        // set the start index to the appropriate index for the direction being moved in
        startIndex = Compass.ConvertCardinalToIndex(direction) * frameCount;

        // if the current frame is not part of the correct animation
        // or if it is the first frame of the move cycle
        // snap it back to the start of the correct animation
        if (frameIndex < startIndex || frameIndex >= startIndex + frameCount) {
            SnapToFrame(startIndex);
        }
    }

}
