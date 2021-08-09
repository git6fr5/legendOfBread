using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Challenge = Room.Challenge;

public class ChallengeTagTool : Select {

    public Challenge challenge;

    void Awake() {
        index = (int)challenge;
    }
}
