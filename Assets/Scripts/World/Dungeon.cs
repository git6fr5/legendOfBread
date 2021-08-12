using System.Collections;
using System.Collections.Generic;
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
    public Tools tools;

    // temp
    Dictionary<string, int[]> roomsTagData;

    // temp
    public Exit exit;


    public List<Exit> exitList = new List<Exit>();

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

        for (int i = exitList.Count - 1; i >= 0; i--) {
            exitList[i].gameObject.SetActive(false);
            Destroy(exitList[i].gameObject);
        }
        exitList = new List<Exit>();

        room.id = id; 
        room.Open(roomFile);
        room.ConstructRoom(roomHash, exits, (Challenge)roomTags[(int)MapChannel.CHALLENGE], tools);

        for (int i = 0; i < room.exitLocations.Count; i++) {
            Vector3Int location = room.exitLocations[i];
           
            //Exit _exit = Instantiate(exit.gameObject, new Vector3(location.x + 0.5f + room.exitID[i][1] / 1.5f, location.y + 0.5f - room.exitID[i][0] / 1.5f, 0), Quaternion.identity).GetComponent<Exit>();
            
            Exit _exit = Instantiate(exit.gameObject, new Vector3(location.x + 0.5f, location.y + 0.5f, 0), Quaternion.identity).GetComponent<Exit>();

            _exit.exitID = room.exitID[i];
            _exit.transform.eulerAngles = Vector3.forward * (180f + room.exitRotations[i]);
            _exit.gameObject.SetActive(true);
            exitList.Add(_exit);
        }

    }


}
