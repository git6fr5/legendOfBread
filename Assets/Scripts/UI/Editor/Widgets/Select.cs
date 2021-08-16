using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Select : Widget
{
    [System.Serializable] public class SelectEvent : UnityEvent<int> { }
    public SelectEvent OnSelect;

    [HideInInspector] public int index;

    public override void Activate() {
        OnSelect.Invoke(index);
    }

}
