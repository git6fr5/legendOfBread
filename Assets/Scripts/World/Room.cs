using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

using Shape = Geometry.Shape;
using Directions = Compass.Direction;

public class Room : MonoBehaviour {

    /* --- ENUMS --- */
    public enum Channel {
        GROUND,
        INTERIOR,
        WALL,
        channelCount
    };

    // very similar to directions but has
    // the center option
    // come back to this
    public enum Tiles {
        EMPTY,
        CENTER,
        RIGHT,
        UP, UP_RIGHT,
        LEFT, LEFT_RIGHT, LEFT_UP, LEFT_UP_RIGHT,
        DOWN, DOWN_RIGHT, DOWN_UP, DOWN_UP_RIGHT, DOWN_LEFT, DOWN_LEFT_RIGHT, DOWN_LEFT_UP,
        DOWN_LEFT_UP_RIGHT
    };

    public enum Challenge {
        EMPTY,
        COMBAT,
        TRAP,
        challengeCount
    }


    /* --- COMPONENTS --- */
    [Space(5)]
    [Header("Read/Save")]
    protected string path = "Rooms/";
    protected string fileExtension = ".room";
    public string tagFile = "rooms.txt";

    [Space(5)]
    [Header("Maps")]
    public Tilemap groundMap;
    public Tilemap interiorMap;
    public Tilemap wallMap;

    // layouts
    [Space(5)]
    [Header("Tiles")]
    public Layout groundLayout;
    public Layout interiorLayout;
    public Layout wallLayout;

    // exits // this is really hacked together lel
    // please come back and clean this thanks
    [HideInInspector] public List<Vector3Int> exitLocations = new List<Vector3Int>();
    [HideInInspector] public List<int[]> exitID = new List<int[]>();
    [HideInInspector] public List<float> exitRotations = new List<float>();

    /* --- VARIABLES --- */
    // room dimensions 
    [Space(5)]
    [Header("Room Dimensions")]
    public Vector2Int id;
    [HideInInspector] public int[][][] roomChannels;
    [Range(2, 32)] public int sizeVertical = 9;
    [Range(2, 32)] public int sizeHorizontal = 9;
    [Range(1, 8)] public int borderVertical = 2;
    [Range(1, 8)] public int borderHorizontal = 2;
    // offset
    protected int horOffset = 0;
    protected int vertOffset = 0;
    // lists to store each channels components
    List<Tilemap> maps = new List<Tilemap>();
    List<Layout> layouts = new List<Layout>();

    /* --- UNITY --- */
    // runs every time this is activated
    void Awake() {
        SetChannels();
        SetOffset();
    }

    /* --- FILES --- */
    public virtual void Read(string fileName) {
        // temp
        print("Reading from File");
        string room = "";
        using (StreamReader readFile = new StreamReader(GameRules.Path + path + fileName + fileExtension)) {
            room = readFile.ReadToEnd();
        }

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

        PrintRoom();
    }

    public virtual Dictionary<string, int[]> ReadTags() {
        // read the tag data from the file
        string allTagString = "";
        using (StreamReader readFile = new StreamReader(GameRules.Path + path + tagFile)) {
            allTagString = readFile.ReadToEnd();
        }
        // read this into a dictionary
        string[] rows = allTagString.Split('\n');
        string[][] stringData = new string[rows.Length - 1][];
        // iterating through the rows
        for (int i = 0; i < rows.Length - 1; i++) {
            string[] columns = rows[i].Split('\t');
            stringData[i] = new string[columns.Length];
            for (int j = 0; j < columns.Length; j++) {
                stringData[i][j] = columns[j];
            }
        }

        Dictionary<string, int[]> allTagData = new Dictionary<string, int[]>();
        for (int i = 0; i < stringData.Length; i++) {
            string[] columns = stringData[i];
            // get the filename from the first column
            if (columns != null && columns.Length > 1 && columns[0] != "") {
                string roomFileName = columns[0];
                // collect the tags into an array
                int[] roomTags = new int[columns.Length - 1];
                for (int j = 1; j < columns.Length; j++) {
                    roomTags[j - 1] = int.Parse(columns[j]);
                }
                // chuck these into a dictionary
                print(roomFileName);
                allTagData.Add(roomFileName, roomTags);
            }
        }
            
        return allTagData;
    }

    /* --- INITIALIZERS --- */
    // initialize the channels for this room
    void SetChannels() {
        //
        // groundLayout.Organize();
        interiorLayout.Organize();
        wallLayout.Organize();
        //
        maps.Add(groundMap);
        maps.Add(interiorMap);
        maps.Add(wallMap);
        //
        layouts.Add(groundLayout);
        layouts.Add(interiorLayout);
        layouts.Add(wallLayout);
    }

    // initialize the offset of tile map
    void SetOffset() {
        // this will do weird stuff if the transform positions aren't at integers
        horOffset = (int)(sizeHorizontal / 2 + transform.position.x);
        vertOffset = (int)(sizeVertical / 2 + transform.position.y);
    }

    /* --- BORDER --- */
    public void SetExits(Directions exits) {
        exitLocations = new List<Vector3Int>();
        exitID = new List<int[]>();
        exitRotations = new List<float>();

        int y_0 = borderVertical - 1;
        int x_0 = borderHorizontal - 1;
        int y_mid = (int)((sizeVertical) / 2);
        int x_mid = (int)((sizeHorizontal) / 2);
        int y_1 = sizeVertical - (borderVertical);
        int x_1 = sizeHorizontal - (borderHorizontal);
 

        // List<int[]> exitCoords = GetExitCoords();
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
        PrintRoom();
    }

    public void SetGround(int seed) {

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
        PrintRoom();
    }

    protected void RemovePoint(int[] point, Channel channel) {
        // print("Removing Tile");
        roomChannels[(int)channel][point[0]][point[1]] = (int)Tiles.EMPTY;
    }

    /* --- DISPLAY --- */
    public void PrintRoom() {
        PrintChannel(Channel.GROUND);
        PrintChannel(Channel.INTERIOR);
        PrintChannel(Channel.WALL);
    }

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
    // grid coordinate to tile map position
    public Vector3Int GridToTileMap(int i, int j) {
        return new Vector3Int(j - horOffset, -(i - vertOffset + 1), 0);
    }


}