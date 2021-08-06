using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Widget : MonoBehaviour {
    
    public Menu menu;

    void OnMouseDown() {
        Activate();
    }

    public virtual void Activate() {
        //
    }

    public virtual void Deactivate() {
        //
    }

}
