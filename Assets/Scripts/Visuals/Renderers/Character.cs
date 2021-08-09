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

    public void SetDirectionIdle(Direction direction) {
        int directionIndex = Compass.ConvertCardinalsToIndex(direction);
        Sprite sprite = directionSprites[directionIndex];
        SetSprite(sprite);
    }

}
