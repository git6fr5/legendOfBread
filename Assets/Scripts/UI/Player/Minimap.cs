using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Channel = Map.Channel;
using Shape = Geometry.Shape;
using Directions = Compass.Direction;

public class Minimap : MonoBehaviour
{

    public TileBase minimapTile;
    public TileBase playerTile;

    public Tilemap minimapMap;

    public float horOffset;
    public float vertOffset;

    void Awake() {
        SetOffset();
    }

    void Update() {
        minimapMap.transform.position = new Vector3(horOffset, vertOffset, 0);
    }

    public void PrintMinimap(Map map) {

        for (int i = 0; i < map.sizeVertical; i++) {
            for (int j = 0; j < map.sizeHorizontal; j++) {
                Vector3Int tilePosition = GridToTileMap(i, j);

                if (map.mapChannels[(int)Channel.SHAPE][i][j] != (int)Shape.EMPTY) {
                    TileBase tile = minimapTile;
                    minimapMap.SetTile(tilePosition, tile);
                }
                else {
                    minimapMap.SetTile(tilePosition, null);
                }
            }
        }
    }

    public void PrintMiniplayer(int[] playerLocation) {

        Vector3Int playerPosition = GridToTileMap(playerLocation[0], playerLocation[1]);
        minimapMap.SetTile(playerPosition, playerTile);
    }

    // initialize the offset of tile map
    void SetOffset() {
        // this will do weird stuff if the transform positions arent at integers
        // horOffset += (int)transform.position.x + (int)minimapMap.transform.position.x;
        // vertOffset += (int)transform.position.y + (int)minimapMap.transform.position.y;
    }

    // grid coordinate to tilemap position
    public Vector3Int GridToTileMap(int i, int j) {
        return new Vector3Int(j, -(i + 1), 0);
    }

}
