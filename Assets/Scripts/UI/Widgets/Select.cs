using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Select : Widget
{
    [System.Serializable] public class SelectEvent : UnityEvent<int> { }
    public SelectEvent OnSelect;

    protected int index;

    void OnMouseDown() {
        Activate();
    }

    public override void Activate() {
        OnSelect.Invoke(index);
    }

}
