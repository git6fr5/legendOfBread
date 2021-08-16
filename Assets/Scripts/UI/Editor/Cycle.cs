using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cycle : MonoBehaviour {
    
    // Cycle Event
    public UnityEvent OnCycle;

    /* --- Unity --- */
    void OnMouseDown() {
        OnCycle.Invoke();
    }
}
