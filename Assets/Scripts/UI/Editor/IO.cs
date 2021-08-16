using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IO : MonoBehaviour {
    
    // IO Event
    [System.Serializable] public class IOEvent : UnityEvent<string> { }
    
    /* --- Components --- */
    public IOEvent OnIO;
    public Stream stream;

    /* --- Unity --- */
    void OnMouseDown() {
        Log.ReadFile(stream.text);
        OnIO.Invoke(stream.text);
    }

}
