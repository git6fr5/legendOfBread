// system modules
using System.Collections;
using System.Collections.Generic;
using System.IO;

// unity modules
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

// library modules
using Priority = Log.Priority;
using Direction = Compass.Direction;
using Shape = Geometry.Shape;

// data modules
using Challenge = Room.Challenge;

public class Map : MonoBehaviour {

    /* --- DEBUG --- */
    protected static Priority debugPrio = Priority.MAP;
    protected static Priority debugSubPrio = Priority.MID;
    protected static string debugTag = "[MAP]: ";

    /* --- ENUMS --- */

    // the data that the map needs
    public enum Channel {
        SHAPE,
        PATH,
        CHALLENGE,
        channelCount
    };

    /* --- COMPONENTS --- */

    // files
    [Space(5)][Header("IO")]
    protected static string path = "Maps/";
    protected static string fileExtension = ".map";

    // minimap
    [Space(5)][Header("Maps")]
    public Minimap minimap;

    /* --- VARIABLES --- */

    // map dimension
    [Space(5)][Header("Map Dimensions")]
    [HideInInspector] public int[][][] mapChannels;
    [Range(1, 8)] public  int sizeVertical = 7;
    [Range(1, 8)] public int sizeHorizontal = 7;

    /* --- UNITY --- */

    // runs once on execution
    protected virtual void Awake() {
        Log.Write("Initializing Map Constructor", debugPrio, debugTag);
    }

    /* --- FILES --- */

    // open a file from the filename
    public virtual void Open(string fileName) {
        Read(fileName);
    }

    // reads data from the file into an array
    public virtual void Read(string fileName) {
        Log.ReadFile(fileName);

        // read the data from the file
        string dungeon = "";
        using (StreamReader readFile = new StreamReader(GameRules.Path + path + fileName + fileExtension)) {
            dungeon = readFile.ReadToEnd();
        }

        // put the data into the appropriate format
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

        // set the dimensions of the map
        sizeVertical = mapChannels[(int)Channel.SHAPE].Length;
        sizeHorizontal = mapChannels[(int)Channel.SHAPE][0].Length;
    }

}
