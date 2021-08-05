using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelSelect : SelectorUI {

    public DungeonEditor dungeonEditor;
    public DungeonEditor.Channel channel;

    public override void OnMouseDown() {
        dungeonEditor.SetMode((int)channel);
    }

}
