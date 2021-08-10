using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class BrushTool : Select {

    public int brushIndex;

    void Awake() {
        index = (int)brushIndex;
    }
}
