﻿using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class DungeonEditor : MonoBehaviour {

    /* --- ENUMS --- */
    public enum Channel {
        ROOMS,
        PATHS,
        TYPES,
        NOT_SURE,
        channelCount
    };

    public enum Rooms {
        EMPTY,
        BASIC,
        roomCount
    }

    public enum Type {
        EMPTY,
        COMBAT,
        PUZZLE,
        typeCount
    }

    /* --- COMPONENTS --- */
    [Space(5)][Header("IO")]
    public bool read = false;
    public bool autoSave = false;
    public string readPath;
    public string savePath;
    string parentPath = "Assets/Resources/Dungeons/";
    string fileType = ".txt";

    [Space(5)][Header("Maps")]
    public Tilemap roomMap;
    public Tilemap pathMap;
    public Tilemap typeMap;
    public Text textMap;

    [Space(5)][Header("Selectors")]
    public Channel mode = Channel.ROOMS;

    [Space(5)][Header("Tiles")]
    public TileBase[] roomTiles;
    public TileBase[] pathTiles;
    public TileBase[] typeTiles;

    /* --- VARIABLES --- */
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
    void OnEnable() {
        SetChannels();
        if (read) { Read(); }
        else { SetGrid(); }
        SetMap();
        PrintAll();
    }

    // runs once every frame
    void Update() {
        if (GetInput()) {
            PrintAll();
            if (autoSave) { Save(); }
        }
    }

    /* --- FILES --- */
    // reads the dungeon from the given path
    void Read() {

        string dungeon = "";
        using (StreamReader readFile = new StreamReader(parentPath + readPath + fileType)) {
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
    }

    // saves the dungeon to the given path
    void Save() {
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

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(parentPath, savePath + fileType))) {
            outputFile.WriteLine(saveString);
        }

    }

    /* --- INITIALIZERS --- */
    // initializes the components for each channel
    void SetChannels() {
        // the maps
        maps.Add(roomMap);
        maps.Add(pathMap);
        maps.Add(typeMap);
        // the tile sets
        tileSets.Add(roomTiles);
        tileSets.Add(pathTiles);
        tileSets.Add(typeTiles);
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

    // initialize a tilemap
    void SetMap() {
        horOffset = (int)sizeHorizontal / 2;
        vertOffset = (int)sizeVertical / 2;
    }

    // sets the current channel thats being edited
    public void SetMode(int selectedChannel) {
        mode = (Channel)selectedChannel;
    }

    /* --- INPUT --- */
    bool GetInput() {

        switch (mode) {
            case Channel.ROOMS:
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

            case Channel.TYPES:
                if (Input.GetMouseButtonDown(0)) {
                    // get the coordinates of the click
                    mouseCoord = ClickToGrid();
                    // add the room
                    ChangeType(mouseCoord);
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
            int room = dungeonChannels[(int)Channel.ROOMS][origin[0]][origin[1]];
            dungeonChannels[(int)Channel.ROOMS][origin[0]][origin[1]] = (room + 1) % (int)Rooms.roomCount;
        }  
    }

    void RemoveRoom(int[] origin) {
        dungeonChannels[(int)Channel.ROOMS][origin[0]][origin[1]] = (int)Rooms.EMPTY;
        // remove all paths attaching to the room as well
    }

    bool CheckRoom(int[] origin) {
        if (CheckLocation(origin)) {
            if (dungeonChannels[(int)Channel.ROOMS][origin[0]][origin[1]] != (int)Rooms.EMPTY) {
                return true;
            }
        }
        return false;
    }

    bool CheckLocation(int[] origin) {
        if (origin[0] < sizeVertical && origin[1] >= 0 && origin[1] < sizeHorizontal && origin[1] >= 0) {
            return true;
        }
        return false;
    }

    void EditPath(int[] origin, int[] dest) {

        //if (PathEditor.ManhattanDistance(origin, dest) == 1) {
        //    if (!CheckRoom(origin)) {
        //        AddRoom(origin);
        //    }
        //    if (!CheckRoom(dest)) {
        //        AddRoom(dest);
        //    }
        //}

        if (CheckRoom(origin) && CheckRoom(dest)) {
            int originIndex = dungeonChannels[(int)Channel.PATHS][origin[0]][origin[1]];
            int destIndex = dungeonChannels[(int)Channel.PATHS][dest[0]][dest[1]];
            dungeonChannels[(int)Channel.PATHS][origin[0]][origin[1]] = PathEditor.GetNewPathIndex(originIndex, origin, dest);
            dungeonChannels[(int)Channel.PATHS][dest[0]][dest[1]] = PathEditor.GetNewPathIndex(destIndex, dest, origin);
        }
    }

    void ChangeType(int[] origin) {
        if (CheckRoom(origin)) {
            int type = dungeonChannels[(int)Channel.TYPES][origin[0]][origin[1]];
            dungeonChannels[(int)Channel.TYPES][origin[0]][origin[1]] = (type + 1) % (int)Type.typeCount;
        }
    }

    /* --- DISPLAY --- */
    public void PrintAll() {
        PrintMap(Channel.ROOMS);
        PrintMap(Channel.PATHS);
        PrintMap(Channel.TYPES);
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

    // prints out a grid cell to a tile
    public void PrintText(Channel channel) {
        string gridString = "";
        int[][] roomChannel = dungeonChannels[(int)channel];
        for (int i = 0; i < sizeVertical; i++) {
            for (int j = 0; j < sizeHorizontal; j++) {
                string cellString = roomChannel[i][j].ToString();
                for (int k = cellString.Length; k < 2; k++) {
                    cellString += " ";
                }
                gridString += cellString;
            }
            gridString += "\n";
        }
        textMap.text = gridString;
    }

    /* --- CONVERSION --- */
    // mouse click to grid coordinate
    public int[] ClickToGrid() {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return PointToGrid(mousePos);
    }

    // a given point to grid coordinates 
    public int[] PointToGrid(Vector2 point) {
        int i = (int)(-point.y + vertOffset + transform.position.y);
        int j = (int)(point.x + horOffset + transform.position.x);
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
