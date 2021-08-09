using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

using Directions = Compass.Direction;
using Shape = Geometry.Shape;

using Challenge = Room.Challenge;

public class MapEditor : Map {

    /* --- COMPONENTS --- */
    // maps
    [Space(5)]
    [Header("Maps")]
    public Tilemap shapeMap;
    public Tilemap pathMap;
    public Tilemap challengeMap;
    // tiles
    [Space(5)]
    [Header("Tiles")]
    public TileBase[] shapeTiles;
    public TileBase[] pathTiles;
    public TileBase[] challengeTiles;

    /* --- VARIABLES --- */
    // mode
    [Space(5)]
    [Header("Edit Mode")]
    public Channel brushChannel = Channel.SHAPE;
    // mouse
    int[] mouseCoord;
    // offset
    int horOffset = 0;
    int vertOffset = 0;
    // lists to store each channels components
    List<Tilemap> maps = new List<Tilemap>();
    List<TileBase[]> tileSets = new List<TileBase[]>();

    /* ---- UNITY --- */
    // runs once after compilation
    void Awake() {
        SetChannels();
        SetOffset();
        SetGrid();
    }

    // runs once before the first frame
    void Start() {
        PrintMap();
    }

    // runs once every frame
    void Update() {
        if (GetInput()) {
            PrintMap();
        }
    }

    /* --- FILES --- */
    // reads the dungeon from the given path
    public override void Read(string fileName) {
        print("Reading from File");
        string dungeon = "";
        using (StreamReader readFile = new StreamReader(GameRules.Path + path + fileName + fileExtension)) {
            dungeon = readFile.ReadToEnd();
        }

        string[] channels = dungeon.Split('\n');
        mapChannels = new int[channels.Length - 1][][];
        for (int n = 0; n < channels.Length - 1; n++) {
            string[] rows = channels[n].Split('\t');
            mapChannels[n] = new int[rows.Length - 1][];
            for (int i = 0; i < rows.Length - 1; i++) {
                string[] columns = rows[i].Split(' ');
                mapChannels[n][i] = new int[columns.Length - 1];
                for (int j = 0; j < columns.Length - 1; j++) {
                    mapChannels[n][i][j] = int.Parse(columns[j]);
                }
            }
        }

        sizeVertical = mapChannels[(int)Channel.SHAPE].Length;
        sizeHorizontal = mapChannels[(int)Channel.SHAPE][0].Length;

        PrintMap();
    }

    // saves the dungeon to the given path
    public void Write(string fileName) {
        print("Writing to File");
        string saveString = "";
        for (int n = 0; n < (int)Channel.channelCount; n++) {
            for (int i = 0; i < sizeVertical; i++) {
                for (int j = 0; j < sizeHorizontal; j++) {
                    saveString += mapChannels[n][i][j].ToString();
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

    // initialize the offset of tile map
    void SetOffset() {
        // this will do weird stuff if the transform positions aren't at integers
        horOffset = (int)(sizeHorizontal / 2 + transform.position.x);
        vertOffset = (int)(sizeVertical / 2 + transform.position.y);
    }

    // initialize a grid full of empty tiles
    void SetGrid() {
        mapChannels = new int[(int)Channel.channelCount][][];
        for (int n = 0; n < (int)Channel.channelCount; n++) {
            mapChannels[n] = new int[sizeVertical][];
            for (int i = 0; i < sizeVertical; i++) {
                mapChannels[n][i] = new int[sizeHorizontal];
                for (int j = 0; j < sizeHorizontal; j++) {
                    mapChannels[n][i][j] = 0;
                }
            }
        }
    }

    // sets the current channel thats being edited
    public void SetBrushChannel(int selectedChannel) {
        brushChannel = (Channel)selectedChannel;
    }

    /* --- INPUT --- */
    bool GetInput() {
        switch (brushChannel) {
            case Channel.SHAPE:
                return BrushShape();               
            case Channel.PATH:
                return BrushPath();
            case Channel.CHALLENGE:
                return BrushChallenge();
            default:
                return false;
        }
    }

    bool BrushShape() {
        if (Input.GetMouseButtonDown(0)) {
            // get the coordinates of the click
            mouseCoord = ClickToGrid();
            // add the room
            AddShape(mouseCoord);
            return true;
        }
        if (Input.GetMouseButtonDown(1)) {
            // get the coordinates of the click
            mouseCoord = ClickToGrid();
            // add the room
            RemoveShape(mouseCoord);
            return true;
        }
        return false;
    }

    bool BrushPath() {
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
    }

    bool BrushChallenge() {
        if (Input.GetMouseButtonDown(0)) {
            // get the coordinates of the click
            mouseCoord = ClickToGrid();
            // add the room
            EditChallenge(mouseCoord);
            return true;
        }
        return false;
    }

    /* --- SHAPES --- */
    void AddShape(int[] origin) {
        if (PointInGrid(origin)) {
            int room = mapChannels[(int)Channel.SHAPE][origin[0]][origin[1]];
            if ((room + 1) % (int)Shape.roomCount == 0) {
                RemoveShape(origin);
            }
            else {
                mapChannels[(int)Channel.SHAPE][origin[0]][origin[1]] = (room + 1) % (int)Shape.roomCount;
            }
        }
    }

    void RemoveShape(int[] origin) {
        mapChannels[(int)Channel.SHAPE][origin[0]][origin[1]] = (int)Shape.EMPTY;
        mapChannels[(int)Channel.CHALLENGE][origin[0]][origin[1]] = (int)Challenge.EMPTY;

        // remove all paths from the room
        mapChannels[(int)Channel.PATH][origin[0]][origin[1]] = (int)Directions.EMPTY;
        // remove all paths attaching to the room as well
        int i = origin[0]; int j = origin[1];
        RemovePath(new int[] { i, j - 1 }, Directions.RIGHT);
        RemovePath(new int[] { i + 1, j }, Directions.UP);
        RemovePath(new int[] { i, j + 1 }, Directions.LEFT);
        RemovePath(new int[] { i - 1, j }, Directions.DOWN);

    }

    bool CheckShape(int[] origin) {
        if (PointInGrid(origin)) {
            if (mapChannels[(int)Channel.SHAPE][origin[0]][origin[1]] != (int)Shape.EMPTY) {
                return true;
            }
        }
        return false;
    }

    /* --- PATHS --- */
    void EditPath(int[] origin, int[] dest) {

        if (CheckShape(origin) && CheckShape(dest)) {
            int originIndex = mapChannels[(int)Channel.PATH][origin[0]][origin[1]];
            int destIndex = mapChannels[(int)Channel.PATH][dest[0]][dest[1]];
            mapChannels[(int)Channel.PATH][origin[0]][origin[1]] = Compass.GetNewPathIndex(originIndex, origin, dest);
            mapChannels[(int)Channel.PATH][dest[0]][dest[1]] = Compass.GetNewPathIndex(destIndex, dest, origin);
        }
    }


    void RemovePath(int[] origin, Directions direction) {
        if (CheckShape(origin)) {
            // remove the right path from the room on the left
            int i = origin[0]; int j = origin[1];
            int pathIndex = mapChannels[(int)Channel.PATH][i][j];
            mapChannels[(int)Channel.PATH][i][j] = Compass.RemovePath(pathIndex, direction);
        }
    }

    /* --- CHALLENGES --- */
    void EditChallenge(int[] origin) {
        if (CheckShape(origin)) {
            int challenge = mapChannels[(int)Channel.CHALLENGE][origin[0]][origin[1]];
            mapChannels[(int)Channel.CHALLENGE][origin[0]][origin[1]] = (challenge + 1) % (int)Challenge.challengeCount;
        }
    }

    /* --- DISPLAY --- */
    public void PrintMap() {
        PrintChannel(Channel.SHAPE);
        PrintChannel(Channel.PATH);
        PrintChannel(Channel.CHALLENGE);
        minimap.PrintMinimap(this);
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
        if (mapChannels[n][i][j] < tileSets[n].Length) {
            TileBase tile = tileSets[n][mapChannels[n][i][j]];
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

    // grid coordinate to tile map position
    public Vector3Int GridToTileMap(int i, int j) {
        return new Vector3Int(j - horOffset, -(i - vertOffset + 1), 0);
    }


}
