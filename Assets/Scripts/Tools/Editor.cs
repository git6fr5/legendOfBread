using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Editor : MonoBehaviour {

    /* --- ENUMS --- */
    public enum Mode {
        DUNGEON,
        ROOM,
        modeCount
    }

    /* --- COMPONENTS --- */
    public DungeonEditor dungeonEditor;
    public RoomEditor roomEditor;

    /* --- VARIABLES --- */
    public string fileName;
    public string parentPath = "Assets/Resources/Dungeons/";
    string fileType = ".txt";

    /* --- UNITY --- */
    // runs once before the first fram
    void Awake() {
        SetPath(defaultPath);
        SetMode(0);
    }

    /* --- METHODS --- */
    // sets the current editing mode
    public void SetMode(Mode mode) {
        switch (mode) {
            case Mode.DUNGEON:
                dungeonEditor.gameObject.SetActive(true);
                roomEditor.gameObject.SetActive(false);
                return;
            case Mode.ROOM:
                dungeonEditor.gameObject.SetActive(false);
                roomEditor.gameObject.SetActive(true);
                return;
            default:
                return;
        }
    }

    /* --- PRINTING --- */
    public static void PrintToMap(int[][] grid, Tilemap tileMap, TileBase[] tileSet, int horOffset, int vertOffset) {
        for (int i = 0; i < sizeVertical; i++) {
            for (int j = 0; j < sizeHorizontal; j++) {
                PrintTile(grid, tileMap, tileSet, i, j, horOffset, vertOffset);
            }
        }
    }

    // prints out a grid cell to a tile
    public static void PrintTile(int[][] grid, Tilemap tileMap, TileBase[] tileSet, int i, int j) {
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
    public static int[] ClickToGrid(int vertOffset, int horOffset) {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return PointToGrid(mousePos, vertOffset, horOffset);
    }

    // a given point to grid coordinates 
    public static int[] PointToGrid(Vector2 point, int vertOffset, int horOffset) {
        int i = (int)(-point.y + vertOffset);
        int j = (int)(point.x + horOffset);
        return new int[] { i, j };
    }

    // checks if a coordinate is in the grid
    public static bool PointInGrid(int[] point, int sizeHorizontal, int sizeVertical) {
        bool isInGrid = (point[1] < sizeHorizontal && point[1] >= 0 && point[0] < sizeVertical && point[0] >= 0);
        if (!isInGrid) {
            print(point[0] + ", " + point[1] + " was not in the grid");
        }
        return isInGrid;
    }

    // grid coordinate to tilemap position
    public static Vector3Int GridToTileMap(int i, int j, int vertOffset, int horOffset) {
        return new Vector3Int(j - horOffset, -(i - vertOffset + 1), 0);
    }

}
