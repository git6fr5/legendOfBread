using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Challenge = Room.Challenge;

public class RoomMenu : Menu {

    public IO open;
    public IO read;
    public Select[] challengeSelectors;
    public Toggle[] toggleableStuff;

    Toggle currentlyToggled;

    void Update() {

        if (currentlyToggled != null) {
            for (int i = 0; i < toggleableStuff.Length; i++) {
                if (toggleableStuff[i].isToggled && currentlyToggled != toggleableStuff[i]) {
                    currentlyToggled.Deactivate();
                    currentlyToggled = toggleableStuff[i];
                }
            }
        }
        else {
            for (int i = 0; i < toggleableStuff.Length; i++) {
                if (toggleableStuff[i].isToggled) {
                    currentlyToggled = toggleableStuff[i];
                }
            }
        }

        for (int i = 0; i < toggleableStuff.Length; i++) {
            toggleableStuff[i].brushes.SetActive(toggleableStuff[i].isToggled);
        }
    }

    public void ToggleChallenge(Challenge challenge) {
        if (challenge == Challenge.EMPTY) {
            currentlyToggled.Deactivate();
            currentlyToggled = null;
        }
        else {
            toggleableStuff[(int)challenge - 1].Activate();
        }
    }

}
