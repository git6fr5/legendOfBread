using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IO : Widget {
    [System.Serializable] public class IOEvent : UnityEvent<string> { }
    
    public IOEvent OnIO;
    public string fileName;

    // temp
    public Stream stream;

    void Update() {
        fileName = stream.text;
    }

    public override void Activate() {
        Log.ReadFile(fileName);
        OnIO.Invoke(fileName);
    }
}
