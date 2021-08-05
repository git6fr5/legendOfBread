using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorSelect : MonoBehaviour {

    public Editor.Mode mode;
    [System.Serializable] public class SelectEvent : UnityEvent<Editor.Mode> { }

    void OnMouseDown() {
        SelectEvent.Invoke(mode);
    }

}
