using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTool : Select {

    public DungeonEditor.Channel channel;

    void Awake() {
        index = (int)channel;
    }
}
