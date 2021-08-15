// system modules
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

// unity modules
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

// library modules
using Priority = Log.Priority;
using Shape = Geometry.Shape;
using Directions = Compass.Direction;

public class Room : MonoBehaviour {

    /* --- DEBUG --- */
    protected static Priority debugPrio = Priority.ROOM;
    protected static Priority debugSubPrio = Priority.MID;
    protected static string debugTag = "[ROOM]: ";

    /* --- ENUMS --- */

    // the data that the room needs
    public enum Channel {
        GROUND,
        INTERIOR,
        WALL,
        channelCount
    };

    // the different possible challenges
    public enum Challenge {
        EMPTY,
        COMBAT,
        TRAP,
        challengeCount
    };

    // the ordered layout of the tiles
    public enum Tiles {
        EMPTY,
        CENTER,
        RIGHT,
        UP, UP_RIGHT,
        LEFT, LEFT_RIGHT, LEFT_UP, LEFT_UP_RIGHT,
        DOWN, DOWN_RIGHT, DOWN_UP, DOWN_UP_RIGHT, DOWN_LEFT, DOWN_LEFT_RIGHT, DOWN_LEFT_UP,
        DOWN_LEFT_UP_RIGHT
    };

    /* --- COMPONENTS --- */
    
    // files
    [Space(5)][Header("IO")]
    protected static string path = "DataFiles/Rooms/";
    protected static string fileExtension = ".txt";
    public static string tagFile = "rooms.txt";

    // maps
    [Space(5)][Header("Maps")]
    public Tilemap groundMap;
    public Tilemap interiorMap;
    public Tilemap wallMap;

    // layouts
    [Space(5)][Header("Tiles")]
    public Layout groundLayout;
    public Layout interiorLayout;
    public Layout wallLayout;

    // exit data
    // NOTE: please come back and clean this thanks
    [HideInInspector] public List<Vector3Int> exitLocations = new List<Vector3Int>();
    [HideInInspector] public List<int[]> exitID = new List<int[]>();
    [HideInInspector] public List<float> exitRotations = new List<float>();

    /* --- VARIABLES --- */

    // room dimensions 
    [Space(5)][Header("Room Dimensions")]
    public int[] id = new int[2];
    [HideInInspector] public int[][][] roomChannels;
    [Range(2, 32)] public int sizeVertical = 11;
    [Range(2, 32)] public int sizeHorizontal = 11;
    [Range(1, 8)] public int borderVertical = 2;
    [Range(1, 8)] public int borderHorizontal = 2;

    // offset
    protected int horOffset = 0;
    protected int vertOffset = 0;

    // lists to store each channels components
    List<Tilemap> maps = new List<Tilemap>();
    List<Layout> layouts = new List<Layout>();

    /* --- UNITY --- */

    // runs once on execution
    void Awake() {
        Log.Write("Initializing Room Constructor", debugPrio, debugTag);

        // initialize the default parameters
        SetChannels();
        SetOffset();
    }

    /* --- FILES --- */

    // open a file from the filename
    public virtual void Open(string fileName) {
        Read(fileName);
    }

    // reads data from the file into an array
    protected void Read(string fileName) {
        Log.ReadFile(fileName);

        // read the data from the file
        TextAsset roomAsset = Resources.Load(path + fileName) as TextAsset;
        string room = roomAsset.text;

        // put the data into the appropriate format
        string[] channels = room.Split('\n');
        roomChannels = new int[channels.Length - 1][][];
        for (int n = 0; n < channels.Length - 1; n++) {
            string[] rows = channels[n].Split('\t');
            roomChannels[n] = new int[rows.Length - 1][];
            for (int i = 0; i < rows.Length - 1; i++) {
                string[] columns = rows[i].Split(' ');
                roomChannels[n][i] = new int[columns.Length - 1];
                for (int j = 0; j < columns.Length - 1; j++) {
                    roomChannels[n][i][j] = int.Parse(columns[j]);
                }
            }
        }

    }

    // reads the tag data from the room list file into a dictionary
    public virtual Dictionary<string, int[]> ReadTags() {
        Log.ReadFile(tagFile);

        // read the tag data from the file
        string compiledTagString = "";
        using (StreamReader readFile = new StreamReader(GameRules.Path + path + tagFile)) {
            compiledTagString = readFile.ReadToEnd();
        }

        // split the tag data by room
        string[] rows = compiledTagString.Split('\n');
        string[][] stringData = new string[rows.Length - 1][];

        // split the tag data by the key and value
        for (int i = 0; i < rows.Length - 1; i++) {
            string[] columns = rows[i].Split('\t');
            stringData[i] = new string[columns.Length];
            for (int j = 0; j < columns.Length; j++) {
                stringData[i][j] = columns[j];
            }
        }

        // put the formatted strings into the dictionary in the appropriate format
        Dictionary<string, int[]> compiledTagData = new Dictionary<string, int[]>();
        for (int i = 0; i < stringData.Length; i++) {

            // check that this is a valid entry
            if (stringData[i] != null && stringData[i].Length > 1 && stringData[i][0] != "") {

                // get the filename from the first column
                string[] columns = stringData[i];
                string roomFileName = columns[0];

                // collect the tags into an array
                int[] roomTags = new int[columns.Length - 1];
                for (int j = 1; j < columns.Length; j++) {
                    roomTags[j - 1] = int.Parse(columns[j]);
                }

                // chuck these into a dictionary
                compiledTagData.Add(roomFileName, roomTags);
            }
        }         
        return compiledTagData;
    }

    /* --- INITIALIZERS --- */

    // initialize the channels for this room
    void SetChannels() {

        // organize the walls
        wallLayout.SetDirectionalOrder();
        
        // add the maps
        maps.Add(groundMap);
        maps.Add(interiorMap);
        maps.Add(wallMap);

        // add the layouts
        layouts.Add(groundLayout);
        layouts.Add(interiorLayout);
        layouts.Add(wallLayout);
    }

    // initialize the offset of tile map
    void SetOffset() {
        // NOTE: this will do weird stuff if the transform positions aren't at integers
        horOffset = (int)(sizeHorizontal / 2 + transform.position.x);
        vertOffset = (int)(sizeVertical / 2 + transform.position.y);
    }

    /* --- CONSTRUCTION --- */

    // creates the room based on the challenge and the room data
    public void ConstructRoom(int seed, Directions exits) {
        Log.Write("Constructing Room with ID: " + Log.ID(id), debugPrio, debugTag);

        // edit the data based on this info
        SetGround(seed);
        SetExits(exits);

        // print the tiles
        PrintChannel(Channel.GROUND);
        PrintChannel(Channel.WALL);
    }

    // create the exits for the room
    public void SetExits(Directions exits) {
        Log.Write("Setting exits for Room with ID: " + Log.ID(id), debugSubPrio, debugTag);

        // reset the exit data
        exitLocations = new List<Vector3Int>();
        exitID = new List<int[]>();
        exitRotations = new List<float>();

        // make the coordinates more readable
        int y_0 = borderVertical - 1;
        int x_0 = borderHorizontal - 1;
        int y_mid = (int)((sizeVertical) / 2);
        int x_mid = (int)((sizeHorizontal) / 2);
        int y_1 = sizeVertical - (borderVertical);
        int x_1 = sizeHorizontal - (borderHorizontal);
 
        // check through each exit
        // NOTE: this can be made much cleaner
        if (Compass.CheckPath((int)exits, Directions.RIGHT)) {
            int[] point = new int[] { y_mid, x_1 };
            RemovePoint(point, Channel.WALL);
            exitLocations.Add(GridToTileMap(point[0], point[1]));
            exitID.Add(new int[] { 0, 1 });
            exitRotations.Add(0f);
        }
        if (Compass.CheckPath((int)exits, Directions.UP)) {
            int[] point = new int[] { y_0, x_mid };
            RemovePoint(point, Channel.WALL);
            exitLocations.Add(GridToTileMap(point[0], point[1]));
            exitID.Add(new int[] { -1, 0 });
            exitRotations.Add(-270f);
        }
        if (Compass.CheckPath((int)exits, Directions.LEFT)) {
            int[] point = new int[] { y_mid, x_0 };
            RemovePoint(point, Channel.WALL);
            exitLocations.Add(GridToTileMap(point[0], point[1]));
            exitID.Add(new int[] { 0, -1 });
            exitRotations.Add(-180f);
        }
        if (Compass.CheckPath((int)exits, Directions.DOWN)) {
            int[] point = new int[] { y_1, x_mid };
            RemovePoint(point, Channel.WALL);
            exitLocations.Add(GridToTileMap(point[0], point[1]));
            exitID.Add(new int[] { 1, 0 });
            exitRotations.Add(-90f);
        }

    }

    public void SetGround(int seed) {
        Log.Write("Setting ground for Room with ID: " + Log.ID(id), debugSubPrio, debugTag);

        int _seed = int.Parse(seed.ToString().Substring(3, 2));
        int row = _seed % 4;

        for (int i = 0; i < sizeVertical; i++) {
            for (int j = 0; j < sizeHorizontal; j++) {
                int tileHash = GameRules.HashID(_seed, new int[] { i, j });
                int tileIndex = tileHash % 4;
                tileIndex = 4 * row + tileIndex;
                roomChannels[(int)Channel.GROUND][i][j] = tileIndex;
            }
        }

    }

    protected void RemovePoint(int[] point, Channel channel) {
        // print("Removing Tile");
        roomChannels[(int)channel][point[0]][point[1]] = (int)Tiles.EMPTY;
    }

    // create the challenges here
    public GameObject[] LoadNewObjects(Challenge challenge, ToolSet toolSet) {
        Log.Write("Creating Challenges for Room with ID " + Log.ID(id), debugSubPrio, debugTag);

        // access the appropriate array for the challenges
        GameObject[] objectReferences = toolSet.GetObjects(challenge);
        List<GameObject> objectList = new List<GameObject>();

        // find out where challenges are and place them
        for (int i = 0; i < sizeVertical; i++) {
            for (int j = 0; j < sizeHorizontal; j++) {
                
                // instantiate the appropriate challenge type indexed at
                int challengeIndex = roomChannels[(int)Channel.INTERIOR][i][j];

                // check that its a valid index
                if (challengeIndex < objectReferences.Length && objectReferences[challengeIndex] != null) {

                    // load the object
                    Vector3 position = (Vector3)GridToTileMap(i, j);
                    GameObject challengeObject = Instantiate(objectReferences[challengeIndex], position + new Vector3(0.5f, 0.5f), Quaternion.identity, transform);
                    challengeObject.SetActive(true);
                    objectList.Add(challengeObject);

                }
            }
        }

        return objectList.ToArray();
    }

    /* --- DISPLAY --- */
    public void PrintChannel(Channel channel) {
        for (int i = 0; i < sizeVertical; i++) {
            for (int j = 0; j < sizeHorizontal; j++) {
                PrintTile(channel, i, j);
            }
        }
    }

    // prints out a grid cell to a tile
    public void PrintTile(Channel channel, int i, int j) {
        // get the tile position from the grid coordinates
        Vector3Int tilePosition = GridToTileMap(i, j);

        // get the channel we're editing
        // for now this is just room channel
        int n = (int)channel;

        // set the tile 
        if (roomChannels[n][i][j] < layouts[n].tiles.Length) {
            TileBase tile = layouts[n].tiles[roomChannels[n][i][j]];
            maps[n].SetTile(tilePosition, tile);
        }
    }

    /* --- CONVERSION --- */

    // a given point to grid coordinates 
    public int[] PointToGrid(Vector2 point) {
        int i = (int)(-point.y + vertOffset);
        int j = (int)(point.x + horOffset);
        //print(i + ", " + j);
        return new int[] { i, j };
    }

    // grid coordinate to tile map position
    public Vector3Int GridToTileMap(int i, int j) {
        return new Vector3Int(j - horOffset, -(i - vertOffset + 1), 0);
    }


}