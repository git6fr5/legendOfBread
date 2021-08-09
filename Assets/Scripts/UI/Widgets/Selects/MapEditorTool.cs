using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorTool : Select {

    public Map.Channel channel;

    void Awake() {
        index = (int)channel;
    }
}
