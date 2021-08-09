using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

using Directions = Compass.Direction;
using Shape = Geometry.Shape;

using Challenge = Room.Challenge;

public class Map : MonoBehaviour {

    /* --- ENUMS --- */
    public enum Channel {
        SHAPE,
        PATH,
        CHALLENGE,
        channelCount
    };

    /* --- COMPONENTS --- */
    [Space(5)][Header("IO")]
    protected string path = "Maps/";
    protected string fileExtension = ".map";
    // maps
    [Space(5)][Header("Maps")]
    public Minimap minimap;

    /* --- VARIABLES --- */
    // mode
    [Space(5)][Header("Edit Mode")]
    public Channel mode = Channel.SHAPE;
    // the dimensions of the dungeon (number of rooms)
    [Space(5)][Header("Dungeon Dimensions")]
    [HideInInspector] public int[][][] mapChannels;
    [Range(1, 8)] public  int sizeVertical = 7;
    [Range(1, 8)] public int sizeHorizontal = 7;

    /* --- FILES --- */
    // reads the dungeon from the given path
    public virtual void Read(string fileName) {
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
    }

}
