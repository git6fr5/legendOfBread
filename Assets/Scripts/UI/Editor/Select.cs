using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Select : MonoBehaviour
{
    // Selection Event
    [System.Serializable] public class SelectEvent : UnityEvent<int> { }
    public SelectEvent OnSelect;

    /* --- Variables --- */
    public int index;

    /* --- Unity --- */
    void OnMouseDown() {
        OnSelect.Invoke(index);
    }
}
