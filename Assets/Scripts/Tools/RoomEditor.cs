using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class RoomEditor : MonoBehaviour {

    /* --- ENUMS --- */
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
    [Space(5)]
    [Header("Read/Save")]
    public bool read = false;
    // public bool autoSave = false;
    public string readPath;
    // public string savePath;

    [Space(5)]
    [Header("Maps")]
    public Tilemap tilemap;
    public Text textMap;
    // layouts
    [Space(5)]
    [Header("Tiles")]
    public Layout tileLayout;

    /* --- VARIABLES --- */
    [Space(5)]
    [Header("Auto Generate")]
    public Vector2Int id;
    // room dimensions 
    [Space(5)]
    [Header("Room Dimensions")]
    [HideInInspector] public int[][] grid;
    [Range(2, 16)] public int sizeVertical = 7;
    [Range(2, 16)] public int sizeHorizontal = 7;
    [Range(1, 32)] public int borderVertical = 4;
    [Range(1, 32)] public int borderHorizontal = 4;
    // offset
    int horOffset = 0;
    int vertOffset = 0;
    // reorders the given tile set to match the enum
    int[] reOrder = new int[] { -1, 7, 8, 12, 13, 6, 10, 11, 15, 2, 3, 17, 18, 1, 0, 16, 19, 0, 0, 0, 0, 0, 0, 0, 0 };

    /* --- UNITY --- */
    // runs once on compilation
    void Awake() {
        tileLayout.Organize();
    }

    // runs every time this is activated
    void OnEnable() {
        if (read) { Read(); }
        else { SetGrid(); }
        SetMap();
        PrintText();
        PrintMap();
    }

    /* --- FILES --- */
    void Read() {
        SetGrid();
        string dungeon = "";
        using (StreamReader readFile = new StreamReader("Assets/Resources/Dungeons/" + readPath + ".txt")) {
            dungeon = readFile.ReadToEnd();
        }

        string[] channels = dungeon.Split('\n');
        int[][][] dungeonChannels = new int[channels.Length - 1][][];
        for (int n = 0; n < channels.Length - 1; n++) {
            string[] rows = channels[n].Split('\t');
            dungeonChannels[n] = new int[rows.Length - 1][];
            for (int i = 0; i < rows.Length - 1; i++) {
                string[] columns = rows[i].Split(' ');
                dungeonChannels[n][i] = new int[columns.Length - 1];
                for (int j = 0; j < columns.Length - 1; j++) {
                    print(columns[j]);
                    print(int.Parse(columns[j]));
                    dungeonChannels[n][i][j] = int.Parse(columns[j]);
                }
            }
        }
        DungeonEditor.Rooms room = (DungeonEditor.Rooms)dungeonChannels[(int)DungeonEditor.Channel.ROOMS][id.x][id.y];
        print(room);
        SetRoom(room);
        CleanGrid();
    }

    /* --- INITIALIZERS --- */
    // initialize a grid full of empty tiles
    void SetGrid() {
        grid = new int[sizeVertical][];
        for (int i = 0; i < sizeVertical; i++) {
            grid[i] = new int[sizeHorizontal];
            for (int j = 0; j < grid[i].Length; j++) {
                grid[i][j] = (int)Tiles.EMPTY;
            }
        }
    }

    // initialize a tilemap
    void SetMap() {
        horOffset = (int)(sizeHorizontal / 2); // -(int)sizeHorizontal * id.x + (id.x * (int)sizeHorizontal / 2);
        vertOffset = (int)(sizeVertical / 2); // (int)sizeVertical * id.y + (-id.y * (int)sizeVertical / 2);
        PrintMap();
    }

    /* --- CONSTRUCTION --- */
    // adds a point at the given coordinates
    public void AddPoint(int i, int j) {
        int[] point = new int[] { i, j };
        if (PointInGrid(point)) {
            grid[point[0]][point[1]] = (int)Tiles.CENTER;
        }
    }

    // add a shape sub grid
    public void AddShape(Geometry2D.Shape shape) {
        // create the shape sub grid
        int dimensionVertical = sizeVertical - 2 * borderVertical;
        int dimensionHorizontal = sizeHorizontal - 2 * borderHorizontal;
        int[][] subGrid = Geometry2D.ConstructShape(shape, (int)Tiles.EMPTY, (int)Tiles.CENTER, dimensionVertical, dimensionHorizontal);
        // add the shape sub grid to the grid
        AttachToGrid(subGrid);
    }

    public void SetRoom(DungeonEditor.Rooms room) {
        // create the shape sub grid
        int dimensionVertical = sizeVertical - 2 * borderVertical;
        int dimensionHorizontal = sizeHorizontal - 2 * borderHorizontal;
        int[][] subGrid = Geometry2D.ConstructRoom(room, (int)Tiles.EMPTY, (int)Tiles.CENTER, dimensionVertical, dimensionHorizontal);
        // add the shape sub grid to the grid
        AttachToGrid(subGrid);
    }

    // attach a sub grid to the grid
    public void AttachToGrid(int[][] subGrid) {
        int[] anchor = new int[] { borderVertical, borderHorizontal };
        for (int i = 0; i < subGrid.Length; i++) {
            for (int j = 0; j < subGrid[0].Length; j++) {
                if (subGrid[i][j] != (int)Tiles.EMPTY) {
                    int[] point = new int[] { i + anchor[0], j + anchor[1] };
                    if (PointInGrid(point)) {
                        grid[i + anchor[0]][j + anchor[1]] = subGrid[i][j];

                    }
                }
            }
        }
    }

    // iterate through the grid and clean each cell
    public void CleanGrid() {
        for (int i = 0; i < sizeVertical; i++) {
            for (int j = 0; j < sizeHorizontal; j++) {
                CleanCell(i, j);
            }
        }
    }

    // check what type of cell it is based on its immediate surroundings
    public void CleanCell(int i, int j) {
        // check only the non-empty tiles
        int code = 1; // starting from one to account for the 0th null tile
        if (grid[i][j] != (int)Tiles.EMPTY) {
            // is top empty
            if (CellEmpty(i - 1, j)) {
                code += 8;
            }
            // is right empty
            if (CellEmpty(i, j - 1)) {
                code += 4;
            }
            // is bottom empty
            if (CellEmpty(i + 1, j)) {
                code += 2;
            }
            // is left empty (i think this might be backwards but it just started working and im scared to mess with it)
            if (CellEmpty(i, j + 1)) {
                code += 1;
            }
            grid[i][j] = code;
        }
    }

    // check if the cell at the given coordinates is empty
    bool CellEmpty(int i, int j) {
        if (i < 0 || i > grid.Length - 1 || j < 0 || j > grid[0].Length - 1) { return true; }
        if (grid[i][j] == (int)Tiles.EMPTY) {
            return true;
        }
        return false;
    }

    /* --- DISPLAY --- */

    // prints the grid to a tilemap
    public void PrintMap() {
        for (int i = 0; i < sizeVertical; i++) {
            for (int j = 0; j < sizeHorizontal; j++) {
                PrintTile(i, j);
            }
        }
    }

    // prints out a grid cell to a tile
    public void PrintTile(int i, int j) {
        // get the tile position from the grid coordinates
        Vector3Int tilePosition = GridToTileMap(i, j);
        // set the tile 
        if (grid[i][j] < tileLayout.tiles.Length) {
            TileBase tileBase = tileLayout.tiles[grid[i][j]];
            tilemap.SetTile(tilePosition, tileBase);
        }
    }

    // prints out a grid cell to a tile
    public void PrintText() {
        string gridString = "";
        for (int i = 0; i < sizeVertical; i++) {
            for (int j = 0; j < sizeHorizontal; j++) {
                string cellString = grid[i][j].ToString();
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
