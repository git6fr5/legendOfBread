using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathEditor : MonoBehaviour
{

    /* --- ENUMS --- */
    public enum Directions {
        EMPTY, 
        RIGHT,
        UP, UP_RIGHT,
        LEFT, LEFT_RIGHT, LEFT_UP, LEFT_UP_RIGHT,
        DOWN, DOWN_RIGHT, DOWN_UP, DOWN_UP_RIGHT, DOWN_LEFT, DOWN_LEFT_RIGHT, DOWN_LEFT_UP,
        DOWN_LEFT_UP_RIGHT
    };

    // up indices = 2, 3, 6, 7, 10, 11, 14, 15
    // left indices = 4 ... 7, 13 ... 15

    public TileBase[] pathTiles;

    public static int ManhattanDistance(int[] origin, int[] dest) {
        int yDistance = Mathf.Abs(origin[0] - dest[0]);
        int xDistance = Mathf.Abs(origin[1] - dest[1]);
        // print(xDistance.ToString() + ", " + yDistance.ToString());
        return yDistance + xDistance;
    }

    public static int GetNewPathIndex(int currIndex, int[] origin, int[] dest) {

        // for resetting the path
        //if (origin == dest) {
        //    return 0;
        //}

        // drawing a path thats too long
        if (ManhattanDistance(origin, dest) != 1) {
            return currIndex;
        }

        // get the direction
        else if (dest[1] - origin[1] == 1) {
            // right
            if (currIndex % 2 == 1) {
                return currIndex - 1;
            }
            return currIndex + 1;
        }
        else if (dest[0] - origin[0] == -1) {
            // up
            int check = currIndex % 4;
            if (check >= 2) {
                return currIndex - 2;
            }
            return currIndex + 2;
        }
        else if (dest[1] - origin[1] == -1) {
            // left
            int check = currIndex % 8;
            if (check >= 4) {
                return currIndex - 4;
            }
            return currIndex + 4;
        }
        else if (dest[0] - origin[0] == 1) {
            // down
            int check = currIndex % 16;
            if (check >= 8) {
                return currIndex - 8;
            }
            return currIndex + 8;
        }

        return currIndex;
    }

    public static int RemoveRightPath(int currIndex) {
        return RemovePathEnum(currIndex, 1);
    }

    public static int RemoveUpPath(int currIndex) {
        return RemovePathEnum(currIndex, 2);
    }

    public static int RemoveLeftPath(int currIndex) {
        return RemovePathEnum(currIndex, 4);
    }

    public static int RemoveDownPath(int currIndex) {
        return RemovePathEnum(currIndex, 8);
    }

    static int RemovePathEnum(int currIndex, int multiplier) {
        int check = currIndex % multiplier * 2;
        if (check >= 8) {
            return currIndex - multiplier;
        }
        return currIndex;
    }


}
