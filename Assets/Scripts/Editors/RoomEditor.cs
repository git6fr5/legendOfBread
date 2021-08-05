using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class RoomEditor : MonoBehaviour {

    /* --- ENUMS --- */
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
    // public bool autoSave = false;
    public string readPath;
    // public string savePath;

    [Space(5)][Header("Maps")]
    public Tilemap groundMap;
    public Tilemap wallMap;

    // layouts
    [Space(5)][Header("Tiles")]
    public Layout tileLayout;

    /* --- VARIABLES --- */
    // room dimensions 
    [Space(5)][Header("Room Dimensions")]
    public Vector2Int id;
    [HideInInspector] public int[][] grid;
    [Range(2, 16)] public int sizeVertical = 7;
    [Range(2, 16)] public int sizeHorizontal = 7;
    [Range(1, 32)] public int borderVertical = 4;
    [Range(1, 32)] public int borderHorizontal = 4;
    // offset
    int horOffset = 0;
    int vertOffset = 0;

    /* --- UNITY --- */
    // runs once on compilation
    void Awake() {
        tileLayout.Organize();
    }

    // runs every time this is activated
    void Start() {
        SetChannels();
        if (read) { Read(); }
        else { SetGrid(); }
        SetOffset();
        PrintMap();
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
    }

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

    // initialize the offset of tilemap
    void SetOffset() {
        // this will do weird stuff if the transform positions aren't at integers
        horOffset = (int)(sizeHorizontal / 2 + transform.position.x);
        vertOffset = (int)(sizeVertical / 2 + transform.position.y);
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
    public void AddShape(DungeonEditor.Shape shape) {
        // create the shape sub grid
        int dimensionVertical = sizeVertical - 2 * borderVertical;
        int dimensionHorizontal = sizeHorizontal - 2 * borderHorizontal;
        int[][] subGrid = Geometry.ConstructBase(shape, (int)Tiles.EMPTY, (int)Tiles.CENTER, dimensionVertical, dimensionHorizontal);
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
        int value = 1; // starting from one to account for the 0th null tile
        if (grid[i][j] != (int)Tiles.EMPTY) {
            // is top empty
            if (CellEmpty(i - 1, j)) {
                value += 8;
            }
            // is right empty
            if (CellEmpty(i, j - 1)) {
                value += 4;
            }
            // is bottom empty
            if (CellEmpty(i + 1, j)) {
                value += 2;
            }
            // is left empty (i think this might be backwards but it just started working and im scared to mess with it)
            if (CellEmpty(i, j + 1)) {
                value += 1;
            }
            grid[i][j] = value;
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
            groundMap.SetTile(tilePosition, tileBase);
        }
    }

    // prints out a grid cell to a tile
    public string PrintText() {
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
        return gridString;
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

    // grid coordinate to tile map position
    public Vector3Int GridToTileMap(int i, int j) {
        return new Vector3Int(j - horOffset, -(i - vertOffset + 1), 0);
    }


}