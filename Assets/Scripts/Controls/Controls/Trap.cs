using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Controller {

    // id
    [Space(5)][Header("ID")]
    public int id = 0;

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

    public virtual void Hit(Hitbox hit) {
        //
    }

}
