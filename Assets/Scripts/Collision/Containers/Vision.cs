using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Priority = Log.Priority;

public class Vision : MonoBehaviour {
    /* --- DEBUG --- */
    protected Priority debugPrio = Priority.COLLISION;
    protected string debugTag = "[VISION]: ";

    /* --- COMPONENTS ---*/
    public State state;

    /* --- VARIABLES --- */
    public List<State> container = new List<State>();

    /* --- UNITY --- */
    void OnTriggerEnter2D(Collider2D collider) {
        Add(collider);
    }

    void OnTriggerExit2D(Collider2D collider) {
        Remove(collider);
    }

    /* --- METHODS --- */
    void Add(Collider2D collider) {

        // add the item if it is in the container and has the correct tag
        if (collider.tag == "Hitbox" && collider.GetComponent<Hitbox>()?.state != null && !container.Contains(collider.GetComponent<Hitbox>().state)) {
            State hitboxState = collider.GetComponent<Hitbox>().state;
            container.Add(hitboxState);
            OnAdd(hitboxState);
        }

    }

    void Remove(Collider2D collider) {

        // remove an item if it is no longer in the container
        if (collider.tag == "Hitbox" && collider.GetComponent<Hitbox>()?.state != null && container.Contains(collider.GetComponent<Hitbox>().state)) {
            State hitboxState = collider.GetComponent<Hitbox>().state;
            container.Remove(hitboxState);
            OnRemove(hitboxState);
        }

    }

    /* --- VIRTUAL --- */
    public void OnAdd(State hitboxState) {
        Log.Write(hitboxState.name + " has vision of " + state.name, debugPrio, debugTag);

        // get the state
        // state.controller.See(hitbox.state, true);
    }

    public void OnRemove(State hitboxState) {
        Log.Write(hitboxState.name + " no longer has vision of " + state.name, debugPrio, debugTag);

        // get the state
        // state.controller.See(hitbox.state, false);
    }
}
