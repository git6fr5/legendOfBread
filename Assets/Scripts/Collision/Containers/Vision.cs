using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Priority = Log.Priority;

// requirements
[RequireComponent(typeof(CircleCollider2D))]
public class Vision : Hitbox {

    /* --- UNITY --- */
    public override void OnAdd(Hitbox hitbox) {
        Log.Write(hitbox.state.name + " has vision of " + state.name, debugPrio, debugTag);

        // get the state
        state.controller.See(hitbox);

    }

    public override void OnRemove(Hitbox hitbox) {
        Log.Write(hitbox.state.name + " no longer has vision of " + state.name, debugPrio, debugTag);
    }
}
