using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class DungeonEditor : MonoBehaviour {

    /* --- ENUMS --- */
    public enum Channel {
        rooms,
        channelCount
    };

    public enum Type {
        combat,
        puzzle
    }

    /* --- COMPONENTS --- */
    [Space(5)]
    [Header("Maps")]
    public Tilemap tileMap;
    public Text textMap;

    // temporary
    public TileBase emptyTile;
    public TileBase filledTile;

    /* --- VARIABLES --- */
    [Space(5)]
    [Header("Dungeon Dimensions")]
    // the dimensions of the dungeon (number of rooms)
    [HideInInspector] public int[][][] roomChannels;
    [Range(1, 8)] public  int sizeVertical = 4;
    [Range(1, 8)] public int sizeHorizontal = 4;
    // offset
    int horOffset = 0;
    int vertOffset = 0;


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
            // PrintText(Channel.rooms);
        }
    }

    /* --- INITIALIZERS --- */
    // initialize a grid full of empty tiles
    void SetGrid() {
        roomChannels = new int[(int)Channel.channelCount][][];
        for (int n = 0; n < (int)Channel.channelCount; n++) {
            roomChannels[n] = new int[sizeVertical][];
            for (int i = 0; i < sizeVertical; i++) {
                roomChannels[n][i] = new int[sizeHorizontal];
                for (int j = 0; j < sizeHorizontal; j++) {
                    roomChannels[n][i][j] = 0;
                }
            }
        }
    }

    // initialize a tilemap
    void SetMap() {
        horOffset = (int)sizeHorizontal / 2;
        vertOffset = (int)sizeVertical / 2;
    }

    bool GetInput() {
        bool isInputting = false;
        if (Input.GetMouseButtonDown(0)) {
            // get the coordinates of the click
            int[] coord = ClickToGrid();
            // add the room
            AddRoom(coord[0], coord[1]);
            isInputting = true;
        }
        return isInputting;
    }

    /* --- CONSTRUCTORS --- */
    void AddRoom(int i, int j) {
        roomChannels[(int)Channel.rooms][i][j] = 1;
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
        if (roomChannels[n][i][j] == 0) {
            tileMap.SetTile(tilePosition, emptyTile);
        }
        else if (roomChannels[n][i][j] == 1) {
            tileMap.SetTile(tilePosition, filledTile);
        }
    }
    // prints out a grid cell to a tile
    public void PrintText(Channel channel) {
        string gridString = "";
        int[][] roomChannel = roomChannels[(int)channel];
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
        print(i + ", " + j);
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
