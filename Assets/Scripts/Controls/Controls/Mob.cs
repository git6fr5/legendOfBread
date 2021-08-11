using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : Controller {

    [Space(5)]
    [Header("ID")]
    public int id = 0;
    public Vision vision;

    public enum FollowState {
        IDLE,
        ACTIVE,
        DEACTIVE,
    }
    public FollowState followState;

    //
    public Vector3 origin;

    /* --- COMPONENTS --- */

    /* --- VARIABLES --- */

    /* --- UNITY --- */
    void Awake() {
        origin = transform.position;
    }

    /* --- METHODS --- */
    public override void GetInput() {
        OnThink();
    }

    public virtual void OnThink() {
        //
    }

    public override void Hit(Hitbox hit) {
        //
    }

}
