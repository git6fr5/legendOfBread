using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Editor : MonoBehaviour {

    public enum Mode {
        DUNGEON,
        ROOM,
        modeCount
    }

    public string readPath;

    public Tilemap roomMinimap;
    public TileBase[] roomMinimapTiles;

    public DungeonEditor dungeonEditor;
    public RoomEditor roomEditor;

    void Awake() {
        dungeonEditor.readPath = readPath;
        roomEditor.readPath = readPath;
        SetMode(0);
    }

    void Update() {
        // Minimap();
    }

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

    public void Minimap() {
        int roomIndex = (int)DungeonEditor.Channel.ROOMS;
        for (int i = 0; i < dungeonEditor.sizeVertical; i++) {
            for (int j = 0; j < dungeonEditor.sizeHorizontal; j++) {
                // set the tile 
                int value = dungeonEditor.dungeonChannels[roomIndex][i][j];
                if (value < roomMinimapTiles.Length) {
                    TileBase tile = roomMinimapTiles[value];
                    roomMinimap.SetTile(new Vector3Int(j, -i, 0), tile);
                }
            }
        }
    }

}
