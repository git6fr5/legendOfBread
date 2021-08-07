﻿using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

using Directions = Coordinates.Directions;

public class DungeonEditor : MonoBehaviour {

    /* --- ENUMS --- */
    public enum Channel {
        SHAPE,
        PATHS,
        CHALLENGE,
        channelCount
    };

    public enum Shape {
        EMPTY,
        SQUARE,
        HORIZONTAL_EVEN_RECTANGLES,
        roomCount
    }

    public enum Challenge {
        EMPTY,
        COMBAT,
        PUZZLE,
        challengeCount
    }

    /* --- COMPONENTS --- */
    [Space(5)][Header("IO")]
    public bool isEditing = false;
    string path = "Dungeons/";
    string fileExtension = ".dungeon";
    // maps
    [Space(5)][Header("Maps")]
    public Tilemap shapeMap;
    public Tilemap pathMap;
    public Tilemap challengeMap;
    public Minimap minimap;
    // tiles
    [Space(5)][Header("Tiles")]
    public TileBase[] shapeTiles;
    public TileBase[] pathTiles;
    public TileBase[] challengeTiles;

    /* --- VARIABLES --- */
    // mode
    [Space(5)][Header("Edit Mode")]
    public Channel mode = Channel.SHAPE;
    // the dimensions of the dungeon (number of rooms)
    [Space(5)][Header("Dungeon Dimensions")]
    [HideInInspector] public int[][][] dungeonChannels;
    [Range(1, 8)] public  int sizeVertical = 4;
    [Range(1, 8)] public int sizeHorizontal = 4;
    // offset
    int horOffset = 0;
    int vertOffset = 0;
    // mouse
    int[] mouseCoord;
    // lists to store each channels components
    List<Tilemap> maps = new List<Tilemap>();
    List<TileBase[]> tileSets = new List<TileBase[]>();

    /* ---- UNITY --- */
    // runs once before the first frame
    void Start() {
        SetChannels();
        SetOffset();
        if (isEditing) {
            SetGrid();
            PrintAll();
        }
    }

    // runs once every frame
    void Update() {
        if (isEditing) {
            if (GetInput()) {
                PrintAll();
            }
        }    
    }

    /* --- FILES --- */
    // reads the dungeon from the given path
    public void Read(string fileName) {
        print("Reading from File");
        string dungeon = "";
        using (StreamReader readFile = new StreamReader(GameRules.Path + path + fileName + fileExtension)) {
            dungeon = readFile.ReadToEnd();
        }

        string[] channels = dungeon.Split('\n');
        dungeonChannels = new int[channels.Length - 1][][];
        for (int n = 0; n < channels.Length - 1; n++) {
            string[] rows = channels[n].Split('\t');
            dungeonChannels[n] = new int[rows.Length - 1][];
            for (int i = 0; i < rows.Length - 1; i++) {
                string[] columns = rows[i].Split(' ');
                dungeonChannels[n][i] = new int[columns.Length - 1];
                for (int j = 0; j < columns.Length - 1; j++) {
                    dungeonChannels[n][i][j] = int.Parse(columns[j]);
                }
            }
        }

        sizeVertical = dungeonChannels[(int)Channel.SHAPE].Length;
        sizeHorizontal = dungeonChannels[(int)Channel.SHAPE][0].Length;

        if (isEditing) {
            PrintAll();
        }
    }

    // saves the dungeon to the given path
    public void Write(string fileName) {
        if (!isEditing) { return; }

        print("Writing to File");
        string saveString = "";
        for (int n = 0; n < (int)Channel.channelCount; n++) {
            for (int i = 0; i < sizeVertical; i++) {
                for (int j = 0; j < sizeHorizontal; j++) {
                    saveString += dungeonChannels[n][i][j].ToString();
                    saveString += " ";
                }
                saveString += "\t";
            }
            saveString += "\n";
        }

        using (StreamWriter outputFile = new StreamWriter(GameRules.Path + path + fileName + fileExtension)) {
            outputFile.WriteLine(saveString);
        }

    }

    /* --- INITIALIZERS --- */
    // initializes the components for each channel
    void SetChannels() {
        // the maps
        maps.Add(shapeMap);
        maps.Add(pathMap);
        maps.Add(challengeMap);
        // the tile sets
        tileSets.Add(shapeTiles);
        tileSets.Add(pathTiles);
        tileSets.Add(challengeTiles);
    }

    // initialize a grid full of empty tiles
    void SetGrid() {
        dungeonChannels = new int[(int)Channel.channelCount][][];
        for (int n = 0; n < (int)Channel.channelCount; n++) {
            dungeonChannels[n] = new int[sizeVertical][];
            for (int i = 0; i < sizeVertical; i++) {
                dungeonChannels[n][i] = new int[sizeHorizontal];
                for (int j = 0; j < sizeHorizontal; j++) {
                    dungeonChannels[n][i][j] = 0;
                }
            }
        }
    }

    // initialize the offset of tile map
    void SetOffset() {
        // this will do weird stuff if the transform positions arent at integers
        horOffset = (int)(sizeHorizontal / 2 + transform.position.x);
        vertOffset = (int)(sizeVertical / 2 + transform.position.y);
    }

    // sets the current channel thats being edited
    public void SetMode(int selectedChannel) {
        mode = (Channel)selectedChannel;
    }

    /* --- INPUT --- */
    bool GetInput() {

        switch (mode) {
            case Channel.SHAPE:
                if (Input.GetMouseButtonDown(0)) {
                    // get the coordinates of the click
                    mouseCoord = ClickToGrid();
                    // add the room
                    AddRoom(mouseCoord);
                    return true;
                }
                if (Input.GetMouseButtonDown(1)) {
                    // get the coordinates of the click
                    mouseCoord = ClickToGrid();
                    // add the room
                    RemoveRoom(mouseCoord);
                    return true;
                }
                return false;

            case Channel.PATHS:
                if (Input.GetMouseButtonDown(0)) {
                    // get the coordinates of the click
                    mouseCoord = ClickToGrid();
                }
                if (Input.GetMouseButtonUp(0)) {
                    // get the coordinates of the release
                    int[] originCoord = mouseCoord;
                    int[] destCoord = ClickToGrid();
                    EditPath(originCoord, destCoord);
                    return true;
                }
                return false;

            case Channel.CHALLENGE:
                if (Input.GetMouseButtonDown(0)) {
                    // get the coordinates of the click
                    mouseCoord = ClickToGrid();
                    // add the room
                    ChangeChallenge(mouseCoord);
                    return true;
                }
                return false;

            default:
                break;

        }

        return false;
    }

    /* --- CONSTRUCTORS --- */
    void AddRoom(int[] origin) {
        if (CheckLocation(origin)) {
            int room = dungeonChannels[(int)Channel.SHAPE][origin[0]][origin[1]];
            if ((room + 1) % (int)Shape.roomCount == 0){
                RemoveRoom(origin);
            }
            else {
                dungeonChannels[(int)Channel.SHAPE][origin[0]][origin[1]] = (room + 1) % (int)Shape.roomCount;
            }
        }  
    }

    void RemoveRoom(int[] origin) {
        dungeonChannels[(int)Channel.SHAPE][origin[0]][origin[1]] = (int)Shape.EMPTY;
        dungeonChannels[(int)Channel.CHALLENGE][origin[0]][origin[1]] = (int)Challenge.EMPTY;

        // remove all paths attaching to the room as well
        dungeonChannels[(int)Channel.PATHS][origin[0]][origin[1]] = (int)Directions.EMPTY;

        int i = origin[0];
        int j = origin[1];
        if (CheckRoom(new int[] { i, j - 1 })) {
            // remove the right path from the room on the left
            int pathIndex = dungeonChannels[(int)Channel.PATHS][i][j - 1];
            dungeonChannels[(int)Channel.PATHS][i][j - 1] = Coordinates.RemovePath(pathIndex, Directions.RIGHT);
        }
        if (CheckRoom(new int[] { i + 1, j })) {
            // remove the up path from the room below
            int pathIndex = dungeonChannels[(int)Channel.PATHS][i + 1][j];
            dungeonChannels[(int)Channel.PATHS][i + 1][j] = Coordinates.RemovePath(pathIndex, Directions.UP);
        }
        if (CheckRoom(new int[] { i, j + 1 })) {
            // remove the left path from the room on the right
            int pathIndex = dungeonChannels[(int)Channel.PATHS][i][j + 1];
            dungeonChannels[(int)Channel.PATHS][i][j + 1] = Coordinates.RemovePath(pathIndex, Directions.LEFT);

        }
        if (CheckRoom(new int[] { i - 1, j })) {
            // remove the down path from the room above
            int pathIndex = dungeonChannels[(int)Channel.PATHS][i - 1][j];
            dungeonChannels[(int)Channel.PATHS][i - 1][j] = Coordinates.RemovePath(pathIndex, Directions.DOWN);
        }
    }

    bool CheckRoom(int[] origin) {
        if (CheckLocation(origin)) {
            if (dungeonChannels[(int)Channel.SHAPE][origin[0]][origin[1]] != (int)Shape.EMPTY) {
                return true;
            }
        }
        return false;
    }

    bool CheckLocation(int[] origin) {
        if (origin[0] < sizeVertical && origin[0] >= 0 && origin[1] < sizeHorizontal && origin[1] >= 0) {
            return true;
        }
        return false;
    }

    void EditPath(int[] origin, int[] dest) {

        if (CheckRoom(origin) && CheckRoom(dest)) {
            int originIndex = dungeonChannels[(int)Channel.PATHS][origin[0]][origin[1]];
            int destIndex = dungeonChannels[(int)Channel.PATHS][dest[0]][dest[1]];
            dungeonChannels[(int)Channel.PATHS][origin[0]][origin[1]] = Coordinates.GetNewPathIndex(originIndex, origin, dest);
            dungeonChannels[(int)Channel.PATHS][dest[0]][dest[1]] = Coordinates.GetNewPathIndex(destIndex, dest, origin);
        }
    }

    void ChangeChallenge(int[] origin) {
        if (CheckRoom(origin)) {
            int challenge = dungeonChannels[(int)Channel.CHALLENGE][origin[0]][origin[1]];
            dungeonChannels[(int)Channel.CHALLENGE][origin[0]][origin[1]] = (challenge + 1) % (int)Challenge.challengeCount;
        }
    }

    /* --- DISPLAY --- */
    public void PrintAll() {
        PrintMap(Channel.SHAPE);
        PrintMap(Channel.PATHS);
        PrintMap(Channel.CHALLENGE);
        minimap.PrintMap(this);
    }

    public void PrintMap(Channel channel) {
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
        if (dungeonChannels[n][i][j] < tileSets[n].Length) {
            TileBase tile = tileSets[n][dungeonChannels[n][i][j]];
            maps[n].SetTile(tilePosition, tile);
        }
    }

    /* --- CONVERSION --- */
    // mouse click to grid coordinate
    public int[] ClickToGrid() {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return PointToGrid(mousePos);
    }

    // a given point to grid coordinates 
    public int[] PointToGrid(Vector2 point) {
        int i = (int)(-point.y + vertOffset);
        int j = (int)(point.x + horOffset);
        // print(i + ", " + j);
        return new int[] { i, j };
    }

    // checks if a coordinate is in the grid
    public bool PointInGrid(int[] point) {
        bool isInGrid = (point[1] < sizeHorizontal && point[1] >= 0 && point[0] < sizeVertical && point[0] >= 0);
        if (!isInGrid) {
            print(point[0] + ", " + point[1] + " was not in the grid");
        }
        return isInGrid;
    }

    // grid coordinate to tilemap position
    public Vector3Int GridToTileMap(int i, int j) {
        return new Vector3Int(j - horOffset, -(i - vertOffset + 1), 0);
    }


}
