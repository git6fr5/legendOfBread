using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class DungeonEditor : MonoBehaviour {

    /* --- ENUMS --- */
    public enum Channel {
        rooms,
        paths,
        channelCount
    };

    public enum Type {
        combat,
        puzzle
    }

    /* --- COMPONENTS --- */
    [Space(5)]
    [Header("Maps")]
    public Tilemap roomMap;
    public Tilemap pathMap;
    public Text textMap;

    // temporary
    public TileBase[] roomTiles;
    public TileBase[] pathTiles;

    /* --- VARIABLES --- */
    [Space(5)]
    [Header("Dungeon Dimensions")]
    // the dimensions of the dungeon (number of rooms)
    [HideInInspector] public int[][][] dungeonChannels;
    [Range(1, 8)] public  int sizeVertical = 4;
    [Range(1, 8)] public int sizeHorizontal = 4;
    // offset
    int horOffset = 0;
    int vertOffset = 0;
    // mouse
    int[] mouseCoord;
    // maps
    List<Tilemap> maps = new List<Tilemap>();
    List<TileBase[]> tileSets = new List<TileBase[]>();


    /* ---- UNITY --- */
    // runs once before the first frame
    void Start() {
        SetGrid();
        SetMap();
        PrintMap(Channel.rooms);
        // PrintText(Channel.rooms);
    }

    // runs once every frame
    void Update() {
        if (GetInput()) {
            PrintMap(Channel.rooms);
            PrintMap(Channel.paths);
            // PrintText(Channel.rooms);
        }
    }

    /* --- INITIALIZERS --- */
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
        maps.Add(roomMap);
        maps.Add(pathMap);
        tileSets.Add(roomTiles);
        tileSets.Add(pathTiles);
    }

    bool GetInput() {
        bool isInputting = false;
        if (Input.GetMouseButtonDown(0)) {
            // get the coordinates of the click
            mouseCoord = ClickToGrid();
            // add the room
            AddRoom(mouseCoord[0], mouseCoord[1]);
            isInputting = true;
        }
        if (Input.GetMouseButtonDown(1)) {
            // get the coordinates of the click
            mouseCoord = ClickToGrid();
        }
        if (Input.GetMouseButtonUp(1)) {
            // get the coordinates of the release
            int[] originCoord = mouseCoord;
            int[] destCoord = ClickToGrid();
            AddPath(originCoord, destCoord);
            isInputting = true;
        }
        return isInputting;
    }

    /* --- CONSTRUCTORS --- */
    void AddRoom(int i, int j) {
        dungeonChannels[(int)Channel.rooms][i][j] = 1;
    }

    void AddPath(int[] origin, int[] destination) {
        int originIndex = dungeonChannels[(int)Channel.paths][origin[0]][origin[1]];
        int destIndex = dungeonChannels[(int)Channel.paths][destination[0]][destination[1]];
        // dungeonChannels[(int)Channel.paths][origin[0]][origin[1]] = (originIndex + 1) % 16; // 
        dungeonChannels[(int)Channel.paths][origin[0]][origin[1]] = PathEditor.GetNewPathIndex(originIndex, origin, destination);
        dungeonChannels[(int)Channel.paths][destination[0]][destination[1]] = PathEditor.GetNewPathIndex(destIndex, destination, origin);
    }

    /* --- DISPLAY --- */
    // note these repeat functionality in room editor
    // can abstract these better
    // prints the grid to a tilemap
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
        if (tileSets[n].Length > dungeonChannels[n][i][j]) {
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
