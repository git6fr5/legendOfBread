using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadChannelSelect : SelectorUI {

    public ChannelSelect[] childrenChannels;
    public ChannelOverhead channelOverhead;
    public bool isToggled = false;

    void Start() {
        SetToggle(false);
    }

    public override void OnMouseDown() {
        for (int i = 0; i < channelOverhead.headChannels.Length; i++) {
            channelOverhead.headChannels[i].SetToggle(false);
        }
        SetToggle(true);
    }

    void SetToggle(bool toggle) {
        for (int i = 0; i < childrenChannels.Length; i++) {
            childrenChannels[i].gameObject.SetActive(toggle);
        } 
    }


}
