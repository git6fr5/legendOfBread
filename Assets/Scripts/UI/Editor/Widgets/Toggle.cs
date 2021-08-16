using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : Widget {

    public GameObject brushes;
    public bool isToggled = false;

    void Awake() {
        Deactivate();
    }

    public override void Activate() {
        isToggled = true;
    }

    public override void Deactivate() {
        isToggled = false;
    }

}
