using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Priority = Log.Priority;

public class Hitbox : MonoBehaviour
{
    /* --- DEBUG --- */
    protected Priority debugPrio = Priority.COLLISION;
    protected string debugTag = "[HITBOX]: ";

    /* --- COMPONENTS ---*/
    public State state;

    /* --- VARIABLES --- */
    public List<Hitbox> container = new List<Hitbox>();

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
        if (collider.tag == tag && collider.GetComponent<Hitbox>() != null && !container.Contains(collider.GetComponent<Hitbox>())) {
            Hitbox hitbox = collider.GetComponent<Hitbox>();
            container.Add(hitbox);
            OnAdd(hitbox);
        }

    }

    void Remove(Collider2D collider) {

        // remove an item if it is no longer in the container
        if (collider.tag == tag && collider.GetComponent<Hitbox>() != null && container.Contains(collider.GetComponent<Hitbox>())) {
            Hitbox hitbox = collider.GetComponent<Hitbox>();
            container.Remove(hitbox);
            OnRemove(hitbox);
        }

    }

    /* --- VIRTUAL --- */

    // on adding
    public virtual void OnAdd(Hitbox hitbox) {
        Log.Write(hitbox.state.name + " is hitting " + state.name, debugPrio, debugTag);
        state.controller.Hit(hitbox);
    }

    // on removing
    public virtual void OnRemove(Hitbox hitbox) {
        Log.Write(hitbox.state.name + " is no longer hitting " + state.name, debugPrio, debugTag);
    }

}
