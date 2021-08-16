using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour {

    /* --- Components ---*/
    public List<Mesh> container = new List<Mesh>();

    /* --- Unity --- */

    // called when the attached collider intersects with another collider
    void OnTriggerEnter2D(Collider2D collider) {
        ScanVision(collider, true);
    }

    // called when the attached collider intersects with another collider
    void OnTriggerExit2D(Collider2D collider) {
        ScanVision(collider, false);
    }

    /* --- Methods --- */
    void ScanVision(Collider2D collider, bool see) {
        // If we weren't colliding with this object before, then hit.
        if (collider.GetComponent<Mesh>() != null) {
            Mesh mesh = collider.GetComponent<Mesh>();
            if (!container.Contains(mesh) && see) {
                container.Add(hitbox);
            }
            else if (container.Contains(mesh) && !see) {
                container.Remove(mesh);
            }
        }
    }

    public void Reset() {
        container = new List<Hitbox>();
    }

}
