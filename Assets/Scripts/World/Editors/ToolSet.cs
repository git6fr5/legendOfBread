using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapChannel = Map.Channel;
using RoomChannel = Room.Channel;
using Challenge = Room.Challenge;

public class ToolSet : MonoBehaviour {

    public Mob[] mobs;
    public Trap[] traps;

    public GameObject[] GetObjects(Challenge challenge) {

        switch (challenge) {
            case Challenge.COMBAT:
                return GetObjectsFromArray(mobs);
            case Challenge.TRAP:
                return GetObjectsFromArray(traps);
            default:
                return new GameObject[0];
        }
    }

    GameObject[] GetObjectsFromArray(Controller[] controllers) {
        int length = GetMaxControllerID(controllers);
        GameObject[] objects = new GameObject[length + 1];
        for (int i = 0; i < controllers.Length; i++) {
            objects[controllers[i].id] = controllers[i].gameObject;
        }
        return objects;
    }

    public int GetMaxControllerID(Controller[] controllers) {

        // therefore we order sprites by their id's here 
        int controllerMaxID = 0;
        for (int i = 0; i < controllers.Length; i++) {
            if (controllers[i].id > controllerMaxID) {
                controllerMaxID = controllers[i].id;
            }
        }

        return controllerMaxID;
    }
}
