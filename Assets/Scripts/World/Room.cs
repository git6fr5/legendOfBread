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

using MapChannel = Map.Channel;

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
        DOWN_LEFT_UP_RIGHT,
        tileCount
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
    public Tilemap borderMap;

    // layouts
    [Space(5)][Header("Tiles")]
    public Layout groundLayout;
    public Layout interiorLayout;
    public Layout borderLayout;

    // exit data
    // NOTE: please come back and clean this thanks
    [HideInInspector] public List<Vector3Int> exitLocations = new List<Vector3Int>();
    [HideInInspector] public List<int[]> exitID = new List<int[]>();
    [HideInInspector] public List<float> exitRotations = new List<float>();

    /* --- VARIABLES --- */

    // room dimensions 
    [Space(5)][Header("Room Dimensions")]
    public int[] id = new int[2];

    public int[][] groundGrid;
    public int[][] interiorGrid;
    public int[][] borderGrid;

    [Range(2, 32)] public int sizeVertical = 11;
    [Range(2, 32)] public int sizeHorizontal = 11;
    [Range(1, 8)] public int borderVertical = 2;
    [Range(1, 8)] public int borderHorizontal = 2;

    // offset
    protected int horOffset = 0;
    protected int vertOffset = 0;

    /* --- UNITY --- */

    // runs once on execution
    void Awake() {
        Log.Write("Initializing Room Constructor", debugPrio, debugTag);

        // initialize the default parameters
        SetGrid();
        SetLayouts();
        SetOffset();
    }

    /* --- FILES --- */

    // initialize a grid full of empty tiles
    void SetGrid() {
        groundGrid = new int[sizeVertical][];
        for (int i = 0; i < sizeVertical; i++) {
            groundGrid[i] = new int[sizeHorizontal];
            for (int j = 0; j < sizeHorizontal; j++) {
                groundGrid[i][j] = (int)Tiles.EMPTY;
            }
        }
        interiorGrid = new int[sizeVertical][];
        for (int i = 0; i < sizeVertical; i++) {
            interiorGrid[i] = new int[sizeHorizontal];
            for (int j = 0; j < sizeHorizontal; j++) {
                interiorGrid[i][j] = (int)Tiles.EMPTY;
            }
        }

    }

    // open a file from the filename
    public virtual void Open(string fileName) {
        Read(fileName);
    }

    // reads data from the file into an array
    protected void Read(string fileName) {
        Log.ReadFile(fileName);

        string room = "";
        using (StreamReader readFile = new StreamReader(GameRules.Path + path + fileName + fileExtension)) {
            room = readFile.ReadToEnd();
        }

        // put the data into the appropriate format
        string[] rows = room.Split('\t');
        interiorGrid = new int[rows.Length - 1][];
        for (int i = 0; i < rows.Length - 1; i++) {
            string[] columns = rows[i].Split(' ');
            interiorGrid[i] = new int[columns.Length - 1];
            for (int j = 0; j < columns.Length - 1; j++) {
                interiorGrid[i][j] = int.Parse(columns[j]);
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
    void SetLayouts() {

        // organize the walls
        borderLayout.SetOrder();
        
    }

    // initialize the offset of tile map
    void SetOffset() {
        // NOTE: this will do weird stuff if the transform positions aren't at integers
        horOffset = (int)(sizeHorizontal / 2 + transform.position.x);
        vertOffset = (int)(sizeVertical / 2 + transform.position.y);
    }

    /* --- CONSTRUCTION --- */

    // creates the room based on the challenge and the room data
    public void ConstructRoom(int seed, Directions exits, int[] roomTags) {
        Log.Write("Constructing Room with ID: " + Log.ID(id), debugPrio, debugTag);

        // edit the data based on this info
        SetGround(seed);
        SetBorder((Shape)roomTags[(int)MapChannel.SHAPE]);
        SetExits(exits);

        PrintRoom();

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
            RemovePoint(point, borderGrid);
            exitLocations.Add(GridToTileMap(point[0], point[1]));
            exitID.Add(new int[] { 0, 1 });
            exitRotations.Add(0f);
        }
        if (Compass.CheckPath((int)exits, Directions.UP)) {
            int[] point = new int[] { y_0, x_mid };
            RemovePoint(point, borderGrid);
            exitLocations.Add(GridToTileMap(point[0], point[1]));
            exitID.Add(new int[] { -1, 0 });
            exitRotations.Add(-270f);
        }
        if (Compass.CheckPath((int)exits, Directions.LEFT)) {
            int[] point = new int[] { y_mid, x_0 };
            RemovePoint(point, borderGrid);
            exitLocations.Add(GridToTileMap(point[0], point[1]));
            exitID.Add(new int[] { 0, -1 });
            exitRotations.Add(-180f);
        }
        if (Compass.CheckPath((int)exits, Directions.DOWN)) {
            int[] point = new int[] { y_1, x_mid };
            RemovePoint(point, borderGrid);
            exitLocations.Add(GridToTileMap(point[0], point[1]));
            exitID.Add(new int[] { 1, 0 });
            exitRotations.Add(-90f);
        }

    }

    public void SetGround(int seed) {
        Log.Write("Setting ground for Room with ID: " + Log.ID(id), debugSubPrio, debugTag);

        // reset the ground
        groundGrid = new int[sizeVertical][];
        for (int i = 0; i < sizeVertical; i++) {
            groundGrid[i] = new int[sizeHorizontal];
            for (int j = 0; j < sizeHorizontal; j++) {
                groundGrid[i][j] = (int)Tiles.EMPTY;
            }
        }

        int _seed = int.Parse(seed.ToString().Substring(3, 2));
        int row = _seed % 4;

        groundGrid = new int[sizeVertical][];
        for (int i = 0; i < sizeVertical; i++) {
            groundGrid[i] = new int[sizeHorizontal];
            for (int j = 0; j < sizeHorizontal; j++) {
                int tileHash = GameRules.HashID(_seed, new int[] { i, j });
                int tileIndex = tileHash % 4;
                tileIndex = 4 * row + tileIndex;
                groundGrid[i][j] = tileIndex;
            }
        }

    }

    // add a shape sub grid
    public void SetBorder(Shape shape) {
        // reset the grid
        borderGrid = new int[sizeVertical][];
        for (int i = 0; i < sizeVertical; i++) {
            borderGrid[i] = new int[sizeHorizontal];
            for (int j = 0; j < sizeHorizontal; j++) {
                borderGrid[i][j] = (int)Tiles.EMPTY;
            }
        }
        // create the shape sub grid
        int[][] subGrid = Geometry.BorderGrid(shape, (int)Tiles.EMPTY, (int)Tiles.CENTER, sizeVertical, sizeHorizontal, borderHorizontal - 1, borderVertical - 1);
        // add the shape sub grid to the grid
        AttachToGrid(subGrid, borderGrid, new int[] { 0, 0 });
        CleanBorder();
    }


    // attach a sub grid to the grid
    public void AttachToGrid(int[][] subGrid, int[][] grid, int[] anchor) {
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
                int challengeIndex = interiorGrid[i][j];

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
    public void PrintRoom() {
        PrintGridToMap(groundGrid, groundMap, groundLayout);
        PrintGridToMap(borderGrid, borderMap, borderLayout);
    }

    public void PrintGridToMap(int[][] grid, Tilemap tileMap, Layout layout) {
        for (int i = 0; i < sizeVertical; i++) {
            for (int j = 0; j < sizeHorizontal; j++) {
                PrintTile(grid, tileMap, layout, i, j);
            }
        }
    }

    // prints out a grid cell to a tile
    public void PrintTile(int[][] grid, Tilemap tilemap, Layout layout, int i, int j) {
        // get the tile position from the grid coordinates
        Vector3Int tilePosition = GridToTileMap(i, j);

        // set the tile 
        int val = grid[i][j];
        if (val < layout.tiles.Length) {
            TileBase tile = layout.tiles[val];
            tilemap.SetTile(tilePosition, tile);
        }
    }

    /* --- CLEANING --- */

    // iterate through the grid and clean each cell
    public void CleanBorder() {
        for (int i = borderVertical - 1; i < sizeVertical - (borderVertical - 1); i++) {
            for (int j = borderHorizontal - 1; j < sizeHorizontal - (borderHorizontal - 1); j++) {
                if (borderGrid[i][j] != (int)Tiles.CENTER) {
                    CleanBorderCell(i, j);
                }
            }
        }
    }

    // iterate through the grid and clean each cell
    public void CleanBorderCell(int i, int j) {

        int val = borderGrid[i][j];

        if (CellIsFilled(borderGrid, i + 1, j)) { val += 8; }
        if (CellIsFilled(borderGrid, i - 1, j)) { val += 2; }
        if (CellIsFilled(borderGrid, i, j + 1)) { val += 1; }
        if (CellIsFilled(borderGrid, i, j - 1)) { val += 4; }
        if (val != 0) { val += 1; }

        borderGrid[i][j] = val;

    }

    bool CellIsFilled(int[][] grid, int i, int j) {
        if (i < 0 || i > sizeVertical - 1 || j < 0 || j > sizeVertical - 1) {
            return false;
        }
        if (grid[i][j] == (int)Tiles.CENTER) {
            return true;
        }
        return false;
    }

    protected void RemovePoint(int[] point, int[][] grid) {
        // print("Removing Tile");
        if (PointInGrid(point)) {
            grid[point[0]][point[1]] = (int)Tiles.EMPTY;
        }
    }


    /* --- CONVERSION --- */

    // mouse click to grid coordinate
    public int[] ClickToGrid() {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return PointToGrid(mousePos);
    }

    // checks if a coordinate is in the grid
    public bool PointInGrid(int[] point) {
        bool isInGrid = (point[1] < sizeHorizontal && point[1] >= 0 && point[0] < sizeVertical && point[0] >= 0);
        if (!isInGrid) {
            // print(point[0] + ", " + point[1] + " was not in the grid");
        }
        return isInGrid;
    }

    // checks if a coordinate is in the grid
    // and also not on the border
    public bool PointWithinBorders(int[] point) {
        bool isInHor = (point[1] < sizeHorizontal && point[1] >= 0);
        bool isInVert = (point[0] < sizeVertical && point[0] >= 0);
        bool isInGrid = (isInHor && isInVert);
        if (isInGrid) {
            // print(point[0] + ", " + point[1] + " was not within the grid");
            if (borderGrid[point[0]][point[1]] != (int)Tiles.EMPTY) {
                isInGrid = false;
            }
        }
        return isInGrid;
    }

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