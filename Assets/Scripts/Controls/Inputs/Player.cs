using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Controller
{
    /* --- VARIABLES --- */
    public KeyCode rightMove = KeyCode.D;
    public KeyCode leftMove = KeyCode.A;
    public KeyCode upMove = KeyCode.W;
    public KeyCode downMove = KeyCode.S;

    /* --- OVERRIDE --- */
    public override void GetInput() {
        // move 
        horizontalMove = 0; verticalMove = 0;
        if (Input.GetKey(rightMove)) { horizontalMove = 1; }
        else if (Input.GetKey(leftMove)) { horizontalMove = -1; }
        else if (Input.GetKey(upMove)) { verticalMove = 1; }
        else if (Input.GetKey(downMove)) { verticalMove = -1; }

    }
}
