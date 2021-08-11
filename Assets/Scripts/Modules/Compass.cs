using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Compass : MonoBehaviour {

    /* --- ENUMS --- */
    public enum Direction {
        EMPTY,
        RIGHT,
        UP, UP_RIGHT,
        LEFT, LEFT_RIGHT, LEFT_UP, LEFT_UP_RIGHT,
        DOWN, DOWN_RIGHT, DOWN_UP, DOWN_UP_RIGHT, DOWN_LEFT, DOWN_LEFT_RIGHT, DOWN_LEFT_UP,
        DOWN_LEFT_UP_RIGHT
    };

    // up indices = 2, 3, 6, 7, 10, 11, 14, 15
    // left indices = 4 ... 7, 13 ... 15

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
            int check = currIndex % 2;
            if (check >= 1) {
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

    public static int RemovePath(int currIndex, Direction direction) {
        if (CheckPath(currIndex, direction)) {
            return currIndex - (int)direction;
        }
        return currIndex;
    }

    public static bool CheckPath(int currIndex, Direction direction) {
        int dirValue = (int)direction;
        int check = currIndex % (dirValue * 2);
        if (check >= dirValue) {
            return true;
        }
        return false;
    }

    public static int ConvertCardinalToIndex(Direction direction) {
        return (int)Mathf.Log((int)direction, 2);
    }

    public static Vector2 DirectionToVector(Direction direction) {
        int directionIndex = (int)direction;
        float x = 0; float y = 0;
        // x
        if (directionIndex % 8 >= 4) { x = 1; }
        else if (directionIndex % 2 >= 1) { x = -1; }
        // y
        if (directionIndex % 16 >= 8) { y = -1; }
        else if (directionIndex % 4 >= 2) { y = 1; }

        return new Vector2(x, y);
    }

    public static Direction VectorToCardinalDirection(Vector3 v) {
        v = (Vector2)v;
        float hor = Vector2.Dot(v, Vector2.right);
        float vert = Vector2.Dot(v, Vector2.up);
        if (Mathf.Abs(hor) >= Mathf.Abs(vert)) {
            if (hor >= 0) {
                return Direction.RIGHT;
            }
            else {
                return Direction.LEFT;
            }
        }
        else {
            if (vert >= 0) {
                return Direction.DOWN;
            }
            else {
                return Direction.UP;
            }
        }
    }

}
