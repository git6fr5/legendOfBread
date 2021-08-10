using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

using Shape = Geometry.Shape;
using Directions = Compass.Direction;

using MapChannel = Map.Channel;

public class RoomEditor : Room {

    /* --- COMPONENTS --- */
    public RoomMenu menu;

    // shape
    public Shape defaultShape = Shape.SQUARE;
    // exits
    public Directions defaultExits = Directions.LEFT;

    // tags
    public Dictionary<MapChannel, int> tagData;
    public Tags tags;

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
    public override void Read(string fileName) {
        // temp
        print("Reading from File");
        string room = "";
        using (StreamReader readFile = new StreamReader(GameRules.Path + path + fileName + fileExtension)) {
            room = readFile.ReadToEnd();
        }

        string[] channels = room.Split('\n');
        roomChannels = new int[channels.Length - 1][][];
        for (int n = 0; n < channels.Length - 1; n++) {
            string[] rows = channels[n].Split('\t');
            roomChannels[n] = new int[rows.Length - 1][];
            for (int i = 0; i < rows.Length - 1; i++) {
                string[] columns = rows[i].Split(' ');
                roomChannels[n][i] = new int[columns.Length - 1];
                for (int j = 0; j < columns.Length - 1; j++) {
                    roomChannels[n][i][j] = int.Parse(columns[j]);
                }
            }
        }

        Dictionary<string, int[]> allTagData = ReadTags();
        int[] _tagData = null;
        if (allTagData.ContainsKey(fileName)) {
            _tagData = allTagData[fileName];
        }
        SetTags(_tagData);
        menu.ToggleChallenge((Challenge)tagData[MapChannel.CHALLENGE]);

        PrintRoom();
    }

    public void Write(string fileName) {
        print("Writing to File");
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

        UpdateTags(fileName);
    }

    public void UpdateTags(string fileName) {

        Dictionary<string, int[]> allTagData = ReadTags();

        // this rooms tag data to the correct format
        int[] _tagData = new int[(int)MapChannel.channelCount];
        foreach (KeyValuePair<MapChannel, int> tagIndex in tagData) {
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
    void SetTags(int[] tagIndices) {
        if (tagIndices == null) { tagIndices = new int[1]; }
        tagData = new Dictionary<MapChannel, int>();
        for (int n = 0; n < (int)MapChannel.channelCount; n++) {
            if (n < tagIndices.Length) {
                tagData[(MapChannel)n] = tagIndices[n];
            }
            else {
                tagData[(MapChannel)n] = 0;
            }
        }

        tags.shapeTag.sprite = tags.shapeTags[tagData[MapChannel.SHAPE]];
        tags.pathTag.sprite = tags.pathTags[tagData[MapChannel.PATH]];
        tags.challengeTag.sprite = tags.challengeTags[tagData[MapChannel.CHALLENGE]];

    }

    // set the tag for that channel using the dropdown buttons
    public void SetTag(int tagIndex) {
        // hard set to challenges for the moment
        tagData[MapChannel.CHALLENGE] = tagIndex;
        tags.challengeTag.sprite = tags.challengeTags[tagIndex];
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
    public override void PrintRoom() {
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