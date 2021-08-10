using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Controller {

    [Space(5)]
    [Header("ID")]
    public int ID = 0;

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
        Activate();
    }

    public virtual void Activate() {

    }

}
