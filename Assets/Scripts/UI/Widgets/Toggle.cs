using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : Widget {

    public Select[] selections;
    public bool isToggled = false;

    void Awake() {
        Deactivate();
    }

    void OnMouseDown() {
        menu.DeactivateAll();
        SetToggle(!isToggled);
    }

    void SetToggle(bool _toggle) {
        isToggled = _toggle;
        for (int i = 0; i < selections.Length; i++) {
            selections[i].gameObject.SetActive(isToggled);
        }
    }

    public override void Activate() {
        SetToggle(true);
    }

    public override void Deactivate() {
        SetToggle(false);
    }

}
