using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Renderer2D
{
    /* --- VARIABLES --- */
    // animations
    [Space(5)] [Header("Animations")]
    public Sprite[] directionSprites;
    public Animation2D[] moveAnimations;
}
