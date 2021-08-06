using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    /* --- DEBUG --- */
   [Space(5)][Header("Debugging")]
    public bool doDebug = false;
    protected string debugTag = "[Container2D]: ";

    /* --- COMPONENTS ---*/
    // the tags for what this container collects
    [Space(5)][Header("Tags")]
    public string[] containerTags;

    /* --- VARIABLES --- */
    public List<Collider2D> container = new List<Collider2D>();
    public bool stickyContainer = false;

    /* --- UNITY --- */
    void OnTriggerEnter2D(Collider2D collider) {
        Add(collider);
    }

    void OnTriggerExit2D(Collider2D collider) {
        Remove(collider);  
    }

    void OnEnable() {
        if (!stickyContainer) { container = new List<Collider2D>(); }
    }

    /* --- METHODS --- */
    void Add(Collider2D collider) {
        // add the item if it is in the container and has the correct tag
        foreach (string containerTag in containerTags) {
            if (!container.Contains(collider) && collider.tag == containerTag) {
                if (doDebug) { print(debugTag + "Added " + collider.name); }
                container.Add(collider);
                // trigger an event
                OnAdd(collider);
            }
        }  
    }

    void Remove(Collider2D collider) { 
        // remove an item if it is no longer in the container
        if (!stickyContainer && container.Contains(collider)) {
            if (doDebug) { print(debugTag + "Removed " + collider.name); }
            container.Remove(collider);
            // trigger an event
            OnRemove(collider);
        }
    }

    /* --- VIRTUAL --- */
    public virtual void OnAdd(Collider2D collider) { 
        // on adding
    }

    public virtual void OnRemove(Collider2D collider) { 
        // on adding
    }
}
