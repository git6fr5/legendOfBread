// system modules
using System.Collections;
using System.Collections.Generic;
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

// data modules
using MapChannel = Map.Channel;

public class RoomEditor : Room {

    /* --- COMPONENTS --- */

    // menu
    [Space(5)][Header("UI")]
    public RoomMenu menu;
    public Tools tools;

    // temp
    public Shape defaultShape = Shape.SQUARE;
    public Directions defaultExits = Directions.LEFT;

    // tags
    public Dictionary<MapChannel, int> tagDict;

    /* --- VARIABLES --- */
    // mode
    [Space(5)][Header("Edit Mode")]
    public Channel brushChannel = Channel.INTERIOR;
    public int brushIndex = 1;

    /* --- UNITY --- */

    // runs every time this is activated
    void Start() {
        SetTags(null);
        SetGrid();

        AddShape(defaultShape, Channel.GROUND);
        AddBorder(defaultShape, Channel.WALL);

        PrintRoom();
    }

    // runs once every frame
    void Update() {
        if (GetInput()) {
            PrintRoom();
        }
    }

    /* --- FILES --- */

    // open a file from the filename
    public override void Open(string fileName) {
        Read(fileName);
        GetTags(fileName);
        PrintRoom();

        // temp
        menu.ToggleChallenge((Challenge)tagDict[MapChannel.CHALLENGE]);
    }

    // saves all the room data to the necessary places
    public void Save(string fileName) {
        Write(fileName);
        WriteTags(fileName);
    }

    // writes the array to the given file
    void Write(string fileName) {
        Log.WriteFile(fileName);

        string saveString = "";
        for (int n = 0; n < (int)Channel.channelCount; n++) {
            for (int i = 0; i < sizeVertical; i++) {
                for (int j = 0; j < sizeHorizontal; j++) {
                    saveString += roomChannels[n][i][j].ToString();
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

    void GetTags(string fileName) {
        Dictionary<string, int[]> allTagData = ReadTags();
        int[] _tagData = null;
        if (allTagData.ContainsKey(fileName)) {
            _tagData = allTagData[fileName];
        }
        SetTags(_tagData);
    }

    void WriteTags(string fileName) {

        Dictionary<string, int[]> allTagData = ReadTags();

        // this rooms tag data to the correct format
        int[] _tagData = new int[(int)MapChannel.channelCount];
        foreach (KeyValuePair<MapChannel, int> tagIndex in tagDict) {
            _tagData[(int)tagIndex.Key] = tagIndex.Value;
        }

        // find this room if it exists
        if (allTagData.ContainsKey(fileName)) {
            allTagData[fileName] = _tagData;
        }
        else {
            allTagData.Add(fileName, _tagData);
        }

        // write this back into the tag file
        string writeTagDataString = "";
        foreach (KeyValuePair<string, int[]> _tags in allTagData) {
            if (_tags.Key != "") {
                writeTagDataString += _tags.Key;
                for (int i = 0; i < _tags.Value.Length; i++) {
                    writeTagDataString += ("\t" + _tags.Value[i].ToString());
                }
                writeTagDataString += "\n";
            }
        }

        using (StreamWriter outputFile = new StreamWriter(GameRules.Path + path + tagFile)) {
            outputFile.Write(writeTagDataString);
        }
    }

    /* --- INITIALIZERS --- */

    // initialize an empty dictionary of tags
    void SetTags(int[] tagArray) {
        if (tagArray == null) { tagArray = new int[1]; }

        tagDict = new Dictionary<MapChannel, int>();
        for (int n = 0; n < (int)MapChannel.channelCount; n++) {
            if (n < tagArray.Length) {
                tagDict[(MapChannel)n] = tagArray[n];
            }
            else {
                tagDict[(MapChannel)n] = 0;
            }
        }

        tools.shapeTag.sprite = tools.shapeTags[tagDict[MapChannel.SHAPE]];
        tools.pathTag.sprite = tools.pathTags[tagDict[MapChannel.PATH]];
        tools.challengeTag.sprite = tools.challengeTags[tagDict[MapChannel.CHALLENGE]];

    }

    // set the tag for that channel using the dropdown buttons
    public void SetTag(int tagIndex) {
        // hard set to challenges channel for the moment
        tagDict[MapChannel.CHALLENGE] = tagIndex;
        tools.challengeTag.sprite = tools.challengeTags[tagIndex];
        tools.SetTools((Challenge)tagIndex);
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

    // sets the current channel thats being edited
    public void SetBrushChannel(int selectedChannel) {
        brushChannel = (Channel)selectedChannel;
    }

    public void SetBrushIndex(int index) {
        brushIndex = index;
        print("Setting Brush Index");
    }

    /* --- INPUT --- */
    bool GetInput() {

        switch (brushChannel) {
            case Channel.INTERIOR:
                if (Input.GetMouseButtonDown(0)) {
                    int[] mouseCoords = ClickToGrid();
                    EditPoint(mouseCoords, Channel.INTERIOR);
                    return true;
                }
                if (Input.GetMouseButtonDown(1)) {
                    int[] mouseCoords = ClickToGrid();
                    RemovePoint(mouseCoords, Channel.INTERIOR);
                    return true;
                }
                return false;
            default:
                return false;
        }
    }

    /* --- DISPLAY --- */
    public void PrintRoom() {
        PrintChannel(Channel.GROUND);
        PrintChannel(Channel.INTERIOR);
        PrintChannel(Channel.WALL);
    }

    /* --- CONSTRUCTION --- */
    // adds a point at the given coordinates
    public void AddPoint(int[] point, Channel channel) {
        if (PointInGrid(point)) {
            // print("Adding Point");
            roomChannels[(int)channel][point[0]][point[1]] = (int)brushIndex;
        }
        // CleanInterior();
    }

    // adds a point at the given coordinates
    public void EditPoint(int[] point, Channel channel, Tiles tile = Tiles.CENTER) {
        if (PointWithinGrid(point)) {
            // print("Editing Point");
            roomChannels[(int)channel][point[0]][point[1]] = (int)brushIndex;
        }
        // CleanInterior();
    }

    // add a shape sub grid
    public void AddShape(Shape shape, Channel channel) {
        // create the shape sub grid
        int dimensionVertical = sizeVertical - 2 * borderVertical;
        int dimensionHorizontal = sizeHorizontal - 2 * borderHorizontal;
        int[][] subGrid = Geometry.ShapeGrid(shape, (int)Tiles.EMPTY, (int)Tiles.CENTER, dimensionVertical, dimensionHorizontal);
        // add the shape sub grid to the grid
        AttachToGrid(subGrid, channel);
    }

    // add a shape sub grid
    public void AddBorder(Shape shape, Channel channel) {
        // create the shape sub grid
        int[][] subGrid = Geometry.BorderGrid(shape, (int)Tiles.EMPTY, (int)Tiles.CENTER, sizeVertical, sizeHorizontal, borderHorizontal, borderVertical);
        // add the shape sub grid to the grid
        AttachToGrid(subGrid, channel, true);
        CleanBorder();
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
    }

    // iterate through the grid and clean each cell
    public void CleanInterior() {
        for (int i = borderVertical; i < sizeVertical - borderVertical; i++) {
            for (int j = borderHorizontal; j < sizeHorizontal - borderHorizontal; j++) {
                CleanInteriorCell(i, j);
            }
        }
    }

    // check what type of cell it is based on its immediate surroundings
    public void CleanInteriorCell(int i, int j) {
        // check only the non-empty tiles
        int value = 1; // starting from one to account for the 0th null tile
        if (roomChannels[(int)Channel.INTERIOR][i][j] != (int)Tiles.EMPTY) {
            // is top empty
            if (CellEmpty(i - 1, j, Channel.INTERIOR)) {
                value += 8;
            }
            // is right empty
            if (CellEmpty(i, j - 1, Channel.INTERIOR)) {
                value += 4;
            }
            // is bottom empty
            if (CellEmpty(i + 1, j, Channel.INTERIOR)) {
                value += 2;
            }
            // is left empty (i think this might be backwards but it just started working and im scared to mess with it)
            if (CellEmpty(i, j + 1, Channel.INTERIOR)) {
                value += 1;
            }
            roomChannels[(int)Channel.INTERIOR][i][j] = value;
        }
    }

    public void CleanBorder() {
        for (int i = 1; i < sizeVertical - 1; i++) {
            for (int j = 1; j < sizeHorizontal - 1; j++) {
                CleanBorderCell(i, j);
            }
        }
        roomChannels[(int)Channel.WALL][borderHorizontal - 1][borderVertical - 1] = 1 + 8 + 4;
        roomChannels[(int)Channel.WALL][borderHorizontal - 1][sizeVertical - borderVertical] = 1 + 8 + 1;
        roomChannels[(int)Channel.WALL][sizeHorizontal - borderHorizontal][borderVertical - 1] = 1 + 2 + 4;
        roomChannels[(int)Channel.WALL][sizeHorizontal - borderHorizontal][sizeVertical - borderVertical] = 1 + 2 + 1;

    }

    void CleanBorderCell(int i, int j) {
        // check only the non-empty tiles
        int value = 1; // starting from one to account for the 0th null tile
        if (roomChannels[(int)Channel.WALL][i][j] != (int)Tiles.EMPTY) {
            // is top empty then point towards top
            if (CellEmpty(i - 1, j, Channel.WALL)) {
                value += 2;
            }
            // if right empty then point right
            if (CellEmpty(i, j - 1, Channel.WALL)) {
                value += 1;
            }
            // if bottom empty then point down
            if (CellEmpty(i + 1, j, Channel.WALL)) {
                value += 8;
            }
            // if left empty then point left
            if (CellEmpty(i, j + 1, Channel.WALL)) {
                value += 4;
            }
            roomChannels[(int)Channel.WALL][i][j] = value;
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
        //print(i + ", " + j);
        return new int[] { i, j };
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
    public bool PointWithinGrid(int[] point) {
        bool isInHor = (point[1] < sizeHorizontal - borderHorizontal && point[1] >= borderHorizontal);
        bool isInVert = (point[0] < sizeVertical - borderVertical && point[0] >= borderVertical);
        bool isInGrid = (isInHor && isInVert);
        if (!isInGrid) {
            // print(point[0] + ", " + point[1] + " was not within the grid");
        }
        return isInGrid;
    }

}