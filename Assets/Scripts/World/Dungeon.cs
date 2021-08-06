using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Channel = DungeonEditor.Channel;
using Shape = DungeonEditor.Shape;
using Challenge = DungeonEditor.Challenge;

using Directions = Coordinates.Directions;

public class Dungeon : MonoBehaviour
{
    /* --- COMPONENTS --- */
    public DungeonEditor dungeonEditor;
    public RoomEditor roomEditor;

    // temp
    public Exit exit;

    public List<Exit> exitList = new List<Exit>();

    /* --- VARIABLES --- */
    public string dungeonFile;
    int[][][] dungeonChannels;
    public int[] roomID = new int[] { 3, 3 };

    /* --- UNITY --- */
    void Awake() {
        dungeonEditor.isEditing = false;
        roomEditor.isEditing = false;
    }

    void Start() {
        dungeonEditor.Read(dungeonFile);
        LoadRoom(roomID);
    }

    /* --- FILES --- */
    public void LoadRoom(int[] id) {
        var roomData = GetRoom(id);
        SetRoom(roomData.Item1, roomData.Item2, roomData.Item3);
        roomID = id;
    }

    (string, Shape, Directions) GetRoom(int[] id) {
        Shape shape = (Shape)dungeonEditor.dungeonChannels[(int)Channel.SHAPE][id[0]][id[1]];
        Directions exits = (Directions)dungeonEditor.dungeonChannels[(int)Channel.PATHS][id[0]][id[1]];

        // use these to get an appropriate room
        // but for now
        return ("basic", shape, exits);
    }

    void SetRoom(string roomFile, Shape shape, Directions exits) {
        roomEditor.defaultExits = exits;
        roomEditor.Read(roomFile);

        for (int i = exitList.Count-1; i >= 0; i--) {
            exitList[i].gameObject.SetActive(false);
            Destroy(exitList[i].gameObject);
        }
        exitList = new List<Exit>();
        for (int i = 0; i < roomEditor.exitLocations.Count; i++) {
            Vector3Int location = roomEditor.exitLocations[i];
            print(location);
            Exit _exit = Instantiate(exit.gameObject, new Vector3(location.x + 0.5f, location.y + 0.5f, 0), Quaternion.identity).GetComponent<Exit>();
            _exit.exitID = roomEditor.exitID[i];
            _exit.gameObject.SetActive(true);
            exitList.Add(_exit);
        }
    }


}
