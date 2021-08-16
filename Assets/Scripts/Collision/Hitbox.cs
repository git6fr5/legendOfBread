/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {

    /* --- Components ---*/
    public State state;
    public List<Hitbox> container = new List<Hitbox>();

    /* --- Unity --- */

    // called when the attached collider intersects with another collider
    void OnTriggerEnter2D(Collider2D collider) {
        ScanHit(collider, true);
    }

    // called when the attached collider intersects with another collider
    void OnTriggerExit2D(Collider2D collider) {
        ScanHit(collider, false);
    }

    /* --- Methods --- */
    void ScanHit(Collider2D collider, bool hit) {
        // If we weren't colliding with this object before, then hit.
        if (collider.GetComponent<Hitbox>() != null) {
            Hitbox hitbox = collider.GetComponent<Hitbox>();
            if (!container.Contains(hitbox) && hit) {
                container.Add(hitbox);
                OnHit(hitbox);
            }
            else if (container.Contains(hitbox) && !hit) {
                container.Remove(hitbox);
            }
        }
    }

    void OnHit(Hitbox hitbox) {
        state.controller.Hit(hitbox);
    }

    public void Reset() {
        container = new List<Hitbox>();
    }

}
