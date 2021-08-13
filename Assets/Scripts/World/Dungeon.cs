using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using MapChannel = Map.Channel;
using Challenge = Room.Challenge;

using Shape = Geometry.Shape;
using Directions = Compass.Direction;

public class Dungeon : MonoBehaviour
{
    /* --- COMPONENTS --- */
    public Map map;
    public Room room;
    public ToolSet toolSet;

    // temp
    Dictionary<string, int[]> roomsTagData;

    // temp
    public Exit nullExit;


    public List<Exit> exitList = new List<Exit>();
    public Dictionary<string, GameObject[]> loadedObjects = new Dictionary<string, GameObject[]>();

    /* --- VARIABLES --- */
    public int seed;
    public string mapFile;
    public int[] roomID = new int[] { 3, 3 };

    /* --- UNITY --- */
    void Awake() {
    }

    void Start() {
        seed = GameRules.Hash(seed);
        map.Read(mapFile);
        roomsTagData = room.ReadTags();
        LoadRoom(roomID);
    }

    /* --- FILES --- */
    public void LoadRoom(int[] id) {
        var roomData = GetRoom(id);
        SetRoom(id, roomData.Item1, roomData.Item2, roomData.Item3, roomData.Item4, roomData.Item5);
        roomID = id;
        map.minimap.PrintMinimap(map);
        map.minimap.PrintMiniplayer(roomID);
    }

    (string, Shape, Directions, int[], int) GetRoom(int[] id) {
        Shape shape = (Shape)map.mapChannels[(int)MapChannel.SHAPE][id[0]][id[1]];
        Directions exits = (Directions)map.mapChannels[(int)MapChannel.PATH][id[0]][id[1]];

        // use these to get an appropriate room
        // but for now

        List<KeyValuePair<string, int[]>> tempRoomFiles = new List<KeyValuePair<string, int[]>>();
        foreach (KeyValuePair<string, int[]> _tagData in roomsTagData) {
            if (_tagData.Value[(int)MapChannel.CHALLENGE] == map.mapChannels[(int)MapChannel.CHALLENGE][id[0]][id[1]]) {
                tempRoomFiles.Add(_tagData);

            }
        }

        int _seed = int.Parse(seed.ToString().Substring(2, 2));
        int roomHash = GameRules.HashID(_seed, id);
        int index = (int)(roomHash) % tempRoomFiles.Count;
        string roomFile = tempRoomFiles[index].Key;
        int[] roomTags = tempRoomFiles[index].Value;
        return (roomFile, shape, exits, roomTags, roomHash);
    }

    void SetRoom(int[] id, string roomFile, Shape shape, Directions exits, int[] roomTags, int roomHash) {

        // deload the previous room
        DeloadExits(room.id);
        DeloadObjects(room.id);

        // load the new room
        room.id = id; 
        room.Open(roomFile);
        room.ConstructRoom(roomHash, exits);
        LoadObjects(id, roomTags);
        LoadExits(room.exitLocations, room.exitRotations, room.exitID);

    }

    void LoadObjects(int[] id, int[] roomTags) {

        // load objects (check for pre-existing objects)
        Challenge roomChallenge = (Challenge)roomTags[(int)MapChannel.CHALLENGE];

        // if the dictionary contains the key
        if (loadedObjects.ContainsKey(Log.ID(id))) {

            GameObject[] loadedRoomObjects = loadedObjects[Log.ID(id)];

            // access the array of objects stored at this room id
            // itterate backwards so we can remove unnecessary items to
            // improve performance
            for (int i = loadedRoomObjects.Length - 1; i >= 0; i--) {

                // only load objects that exist and aren't "dead"
                if (loadedRoomObjects[i] == null || loadedRoomObjects[i].GetComponent<State>().isDead) {
                    // loadedRoomObjects.RemoveAt(i);
                }
                else {
                    loadedRoomObjects[i].SetActive(true);
                    if (loadedRoomObjects[i].GetComponent<Mob>() != null) {
                        loadedRoomObjects[i].transform.position = loadedRoomObjects[i].GetComponent<Mob>().origin;
                    }
                    else if (loadedRoomObjects[i].GetComponent<Trap>() != null) {
                        loadedRoomObjects[i].transform.position = loadedRoomObjects[i].GetComponent<Trap>().origin;
                    }
                }
            }

            // loadedObjects[Log.ID(id)] = loadedRoomObjects.ToArray();

        }
        else {
            GameObject[] newObjects = room.LoadNewObjects(roomChallenge, toolSet);
            loadedObjects.Add(Log.ID(id), newObjects);
        }
    }

    void DeloadObjects(int[] id) {
        // unload the previous challenges
        if (loadedObjects.ContainsKey(Log.ID(id))) {
            GameObject[] loadedRoomObjects = loadedObjects[Log.ID(id)];
            for (int i = loadedObjects[Log.ID(id)].Length - 1; i >= 0; i--) {
                loadedObjects[Log.ID(id)][i].SetActive(false);
            }
        }
    }

    // reset the exits
    void DeloadExits(int[] id) {
        for (int i = exitList.Count - 1; i >= 0; i--) {
            exitList[i].gameObject.SetActive(false);
            Destroy(exitList[i].gameObject);
        }
        exitList = new List<Exit>();
    }

    // create new exits
    void LoadExits(List<Vector3Int> exitLocations, List<float> exitRotations, List<int[]> exitIDs) {

        for (int i = 0; i < exitLocations.Count; i++) {

            // cache the location
            Vector3Int location = exitLocations[i];

            // instantiate the exit
            Exit exit = Instantiate(nullExit.gameObject, new Vector3(location.x + 0.5f, location.y + 0.5f, 0), Quaternion.identity).GetComponent<Exit>();

            // set the exit ID
            exit.exitID = exitIDs[i];

            // rotate the exit
            exit.transform.eulerAngles = Vector3.forward * (180f + exitRotations[i]);

            // activate the exit
            exit.gameObject.SetActive(true);

            // cache the exit
            exitList.Add(exit);

        }
    }


}
