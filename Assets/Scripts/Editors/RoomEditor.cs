using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

using Shape = DungeonEditor.Shape;
using Directions = Coordinates.Directions;

public class RoomEditor : MonoBehaviour {

    /* --- ENUMS --- */
    public enum Channel {
        GROUND,
        WALL,
        channelCount
    };

    // very similar to directions but has
    // the center option
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
    [Space(5)][Header("Read/Save")]
    public bool read = false;
    public bool autoWrite = false;
    public string readPath;
    public string savePath;

    [Space(5)][Header("Maps")]
    public Tilemap groundMap;
    public Tilemap wallMap;

    // layouts
    [Space(5)][Header("Tiles")]
    public Layout groundLayout;
    public Layout wallLayout;

    // shape
    public Shape defaultShape = Shape.SQUARE;
    // exits
    public Directions defaultExits = Directions.LEFT;

    /* --- VARIABLES --- */
    // mode
    [Space(5)][Header("Edit Mode")]
    public Channel mode = Channel.WALL;
    // room dimensions 
    [Space(5)][Header("Room Dimensions")]
    public Vector2Int id;
    [HideInInspector] public int[][][] roomChannels;
    [Range(2, 16)] public int sizeVertical = 7;
    [Range(2, 16)] public int sizeHorizontal = 7;
    [Range(1, 32)] public int borderVertical = 4;
    [Range(1, 32)] public int borderHorizontal = 4;
    // offset
    int horOffset = 0;
    int vertOffset = 0;
    // lists to store each channels components
    List<Tilemap> maps = new List<Tilemap>();
    List<Layout> layouts = new List<Layout>();

    /* --- UNITY --- */
    // runs every time this is activated
    void Start() {
        SetChannels();
        if (read) { Read(); }
        else { SetGrid(); }
        SetOffset();
        SetBase();
        PrintAll();
    }

    // runs once every frame
    void Update() {
        if (GetInput()) {
            PrintAll();
            if (autoWrite) { Write(); }
        }
    }

    /* --- FILES --- */
    void Read() {
        // temp
        SetGrid();
    }

    void Write() {
        // temp
    }

    /* --- INITIALIZERS --- */
    // initialize the channels for this room
    void SetChannels() {
        //
        groundLayout.Organize();
        wallLayout.Organize();
        //
        maps.Add(groundMap);
        maps.Add(wallMap);
        //
        layouts.Add(groundLayout);
        layouts.Add(wallLayout);
    }

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

    // initialize the offset of tile map
    void SetOffset() {
        // this will do weird stuff if the transform positions aren't at integers
        horOffset = (int)(sizeHorizontal / 2 + transform.position.x);
        vertOffset = (int)(sizeVertical / 2 + transform.position.y);
    }

    void SetBase() {
        AddShape(defaultShape, Channel.WALL, true);
        AddShape(defaultShape, Channel.GROUND, false);
        SetExits();
        CleanChannel(Channel.WALL);
    }

    void SetExits() {
        if (Coordinates.CheckPath((int)defaultExits, Directions.RIGHT)) {
            int[] point = new int[] { (int)(sizeVertical / 2), (int)(sizeHorizontal - 1) };
            AddPoint(point, Channel.WALL, Tiles.EMPTY);
        }
        if (Coordinates.CheckPath((int)defaultExits, Directions.UP)) {
            int[] point = new int[] { (int)(sizeVertical - 1), (int)(sizeHorizontal / 2) };
            AddPoint(point, Channel.WALL, Tiles.EMPTY);
        }
        if (Coordinates.CheckPath((int)defaultExits, Directions.LEFT)) {
            int[] point = new int[] { (int)(sizeVertical / 2), 0 };
            AddPoint(point, Channel.WALL, Tiles.EMPTY);
        }
        if (Coordinates.CheckPath((int)defaultExits, Directions.DOWN)) {
            int[] point = new int[] { 0, (int)(sizeHorizontal / 2) };
            AddPoint(point, Channel.WALL, Tiles.EMPTY);
        }
    }

    // sets the current channel thats being edited
    public void SetMode(int selectedChannel) {
        mode = (Channel)selectedChannel;
    }

    /* --- INPUT --- */
    bool GetInput() {

        switch (mode) {
            case Channel.WALL:
                if (Input.GetMouseButtonDown(0)) {
                    int[] mouseCoords = ClickToGrid();
                    EditPoint(mouseCoords, Channel.WALL);
                    return true;
                }
                return false;
            default:
                return false;
        }
    }

    /* --- CONSTRUCTION --- */
    // adds a point at the given coordinates
    public void AddPoint(int[] point, Channel channel, Tiles tile = Tiles.CENTER) {
        if (PointInGrid(point)) {
            print("Adding Point");
            roomChannels[(int)channel][point[0]][point[1]] = (int)tile;
        }
        CleanChannel(channel);
    }

    // adds a point at the given coordinates
    public void EditPoint(int[] point, Channel channel, Tiles tile = Tiles.CENTER) {
        if (PointWithinGrid(point)) {
            print("Adding Point");
            roomChannels[(int)channel][point[0]][point[1]] = (int)tile;
        }
        CleanChannel(channel);
    }

    // add a shape sub grid
    public void AddShape(Shape shape, Channel channel, bool isBorder = false) {
        // create the shape sub grid
        int dimensionVertical = sizeVertical;
        int dimensionHorizontal = sizeHorizontal;
        if (!isBorder) {
            dimensionVertical -= 2 * borderVertical;
            dimensionHorizontal -= 2 * borderHorizontal;
        }
        int[][] subGrid = Geometry.ConstructBase(shape, (int)Tiles.EMPTY, (int)Tiles.CENTER, dimensionVertical, dimensionHorizontal, isBorder);
        // add the shape sub grid to the grid
        AttachToGrid(subGrid, channel, isBorder);
    }

    // attach a sub grid to the grid
    public void AttachToGrid(int[][] subGrid, Channel channel, bool isBorder = false) {
        int[] anchor = new int[] { 0, 0 };
        if (!isBorder) {
            anchor[0] += borderVertical;
            anchor[1] += borderHorizontal;
        }
        for (int i = 0; i < subGrid.Length; i++) {
            for (int j = 0; j < subGrid[0].Length; j++) {
                if (subGrid[i][j] != (int)Tiles.EMPTY) {
                    int[] point = new int[] { i + anchor[0], j + anchor[1] };
                    if (PointInGrid(point)) {
                        roomChannels[(int)channel][i + anchor[0]][j + anchor[1]] = subGrid[i][j];

                    }
                }
            }
        }
        CleanChannel(channel);
    }

    // iterate through the grid and clean each cell
    public void CleanChannel(Channel channel) {
        for (int i = 0; i < sizeVertical; i++) {
            for (int j = 0; j < sizeHorizontal; j++) {
                CleanCell(i, j, channel);
            }
        }
    }

    // check what type of cell it is based on its immediate surroundings
    public void CleanCell(int i, int j, Channel channel) {
        // check only the non-empty tiles
        int value = 1; // starting from one to account for the 0th null tile
        if (roomChannels[(int)channel][i][j] != (int)Tiles.EMPTY) {
            // is top empty
            if (CellEmpty(i - 1, j, channel)) {
                value += 8;
            }
            // is right empty
            if (CellEmpty(i, j - 1, channel)) {
                value += 4;
            }
            // is bottom empty
            if (CellEmpty(i + 1, j, channel)) {
                value += 2;
            }
            // is left empty (i think this might be backwards but it just started working and im scared to mess with it)
            if (CellEmpty(i, j + 1, channel)) {
                value += 1;
            }
            roomChannels[(int)channel][i][j] = value;
        }
    }

    // check if the cell at the given coordinates is empty
    bool CellEmpty(int i, int j, Channel channel) {
        if (i < 0 || i > sizeVertical - 1 || j < 0 || j > sizeHorizontal - 1) { return true; }
        if (roomChannels[(int)channel][i][j] == (int)Tiles.EMPTY) {
            return true;
        }
        return false;
    }

    /* --- DISPLAY --- */
    public void PrintAll() {
        PrintMap(Channel.GROUND);
        PrintMap(Channel.WALL);
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
        if (roomChannels[n][i][j] < layouts[n].tiles.Length) {
            TileBase tile = layouts[n].tiles[roomChannels[n][i][j]];
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

    // checks if a coordinate is in the grid
    public bool PointWithinGrid(int[] point) {
        bool isInGrid = (point[1] < sizeHorizontal + 1 && point[1] >= 1 && point[0] < sizeVertical + 1 && point[0] >= 1);
        if (!isInGrid) {
            print(point[0] + ", " + point[1] + " was not within the grid");
        }
        return isInGrid;
    }

    // grid coordinate to tile map position
    public Vector3Int GridToTileMap(int i, int j) {
        return new Vector3Int(j - horOffset, -(i - vertOffset + 1), 0);
    }


}