using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cycle : Widget
{
    public UnityEvent OnCycle;

    [HideInInspector] public int index;

    public override void Activate() {
        OnCycle.Invoke();
    }
}
