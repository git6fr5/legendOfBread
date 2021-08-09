using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Controller
{

    int thinkTickInterval = 1024;
    int thinkTicker = 0;
    public bool isActive = false;

    /* --- COMPONENTS --- */

    /* --- VARIABLES --- */

    /* --- UNITY --- */

    /* --- METHODS --- */
    public override void GetInput() {
        thinkTicker++;
        if (thinkTicker >= thinkTickInterval) {
            OnThink();
            thinkTicker = thinkTicker % thinkTickInterval;
        }
    }

    public virtual void OnThink() {
        //
    }

    public override void Hit(Hitbox hit) {
        print("Hello");
        Activate();
    }

    public virtual void Activate() {

    }

}
