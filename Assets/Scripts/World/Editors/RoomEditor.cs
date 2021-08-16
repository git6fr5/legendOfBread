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
    [Space(5)] [Header("UI")]
    public ToolBrush toolBrush;

    // temp
    public Shape defaultShape = Shape.SQUARE;
    public Directions defaultExits = Directions.LEFT;

    // tags
    public Dictionary<MapChannel, int> tagDict;

    /* --- VARIABLES --- */
    // mode
    [Space(5)] [Header("Edit Mode")]
    public int brushIndex = 1;

    /* --- UNITY --- */

    // runs every time this is activated
    void Start() {
        SetTags(null);
        PrintRoom();
    }

    // runs once every frame
    void Update() {
        if (GetInput()) {
            PrintGridToMap(interiorGrid, interiorMap, interiorLayout);
        }
    }

    /* --- INITIALIZERS --- */

    // initialize an empty dictionary of tags
    void SetTags(int[] tagArray) {
        if (tagArray == null) { tagArray = new int[3] { 1, 1, 1 }; }

        tagDict = new Dictionary<MapChannel, int>();
        for (int n = 0; n < (int)MapChannel.channelCount; n++) {
            if (n < tagArray.Length) {
                tagDict[(MapChannel)n] = tagArray[n];
            }
            else {
                tagDict[(MapChannel)n] = 0;
            }
        }

        IncrementShapeTag(0);
        // toolBrush.currPathTag.sprite = toolBrush.pathTags[tagDict[MapChannel.PATH]];
        IncrementChallengeTag(0);
    }

    public void IncrementShapeTag(int increment = 1) {

        // increment the tags
        int newIndex = IncrementTag(MapChannel.SHAPE, increment, (int)Shape.shapeCount);
        toolBrush.currShapeTag.sprite = toolBrush.shapeTags[newIndex];

        SetBorder((Shape)newIndex);
        PrintRoom();
    }

    public void IncrementPathTag() {
        //
    }

    // set the tag for that channel using the dropdown buttons
    public void IncrementChallengeTag(int increment = 1) {

        int newIndex = IncrementTag(MapChannel.CHALLENGE, increment, (int)Challenge.challengeCount, true);
        toolBrush.currChallengeTag.sprite = toolBrush.challengeTags[newIndex];
        
        // set the palette
        toolBrush.SetPalette((Challenge)newIndex);
        PrintRoom();
    }

    int IncrementTag(MapChannel channel, int increment, int count, bool allowZero = false) {
        int newIndex = (tagDict[channel] + increment) % (int)count;
        if (!allowZero && newIndex == 0) {
            newIndex = 1;
        }
        tagDict[channel] = newIndex;
        return newIndex;
    }

    public void SetBrushIndex(int index) {
        brushIndex = index;
        print("Setting Brush Index");
    }

    /* --- INPUT --- */
    bool GetInput() {

        if (Input.GetMouseButtonDown(0)) {
            int[] mouseCoords = ClickToGrid();
            EditPoint(mouseCoords);
            return true;
        }
        if (Input.GetMouseButtonDown(1)) {
            int[] mouseCoords = ClickToGrid();
            RemovePoint(mouseCoords, interiorGrid);
            return true;
        }
        return false;

    }

    /* --- CONSTRUCTION --- */

    // adds a point at the given coordinates
    public void EditPoint(int[] point) {
        if (PointWithinBorders(point)) {
            // print("Editing Point");
            interiorGrid[point[0]][point[1]] = (int)brushIndex;
        }
    }

    /* --- FILES --- */

    // open a file from the filename
    public override void Open(string fileName) {
        Read(fileName);
        GetTags(fileName);
        PrintRoom();
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
        for (int i = 0; i < sizeVertical; i++) {
            for (int j = 0; j < sizeHorizontal; j++) {
                saveString += interiorGrid[i][j].ToString();
                saveString += " ";
            }
            saveString += "\t";
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

}