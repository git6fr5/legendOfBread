using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Priority = Log.Priority;

public class Collection : MonoBehaviour {
    /* --- DEBUG --- */
    protected Priority debugPrio = Priority.COLLISION;
    protected string debugTag = "[COLLECTION]: ";

    /* --- COMPONENTS ---*/
    public Inventory inventory;

    /* --- UNITY --- */
    void OnTriggerEnter2D(Collider2D collider) {
        Collect(collider);
    }

    /* --- METHODS --- */
    void Collect(Collider2D collider) {

        // add the item if it is in the container and has the correct tag
        if (collider.GetComponent<Item>() != null) {
            Item item = collider.GetComponent<Item>();
            inventory.Add(item);
        }

    }

}
