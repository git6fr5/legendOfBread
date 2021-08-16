using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Compass {

    /* --- ENUMS --- */
    public enum Direction {
        EMPTY,
        RIGHT,
        UP, UP_RIGHT,
        LEFT, LEFT_RIGHT, LEFT_UP, LEFT_UP_RIGHT,
        DOWN, DOWN_RIGHT, DOWN_UP, DOWN_UP_RIGHT, DOWN_LEFT, DOWN_LEFT_RIGHT, DOWN_LEFT_UP,
        DOWN_LEFT_UP_RIGHT
    };

    public static Dictionary<Direction, int> BinaryDirections = new Dictionary<Direction, int>() {
        { Direction.RIGHT, 0 },
        { Direction.UP, 1 },
        { Direction.LEFT, 2 },
        { Direction.DOWN, 3 }
    };

    public static Dictionary<Vector2, Direction> VectorDirections = new Dictionary<Vector2, Direction>() {
        { Vector2.right, Direction.RIGHT },
        { Vector2.up, Direction.UP },
        {  -Vector2.right, Direction.LEFT },
        {  -Vector2.up, Direction.DOWN }
    };

    public static Dictionary<Direction, Vector2> DirectionVectors = new Dictionary<Direction, Vector2>() {
        { Direction.RIGHT, Vector2.right },
        { Direction.UP, Vector2.up },
        {  Direction.LEFT, -Vector2.right  },
        {  Direction.DOWN, -Vector2.up  }
    };

    public static int ManhattanDistance(int[] origin, int[] dest) {
        int yDistance = Mathf.Abs(origin[0] - dest[0]);
        int xDistance = Mathf.Abs(origin[1] - dest[1]);
        return yDistance + xDistance;
    }

    public static int GetNewPath(int currIndex, int[] origin, int[] dest) {

        // In the case of drawing a path thats too long.
        if (ManhattanDistance(origin, dest) != 1) {
            return currIndex;
        }

        // Get the direction and edit accordingly.
        Vector2 vectorDirection = new Vector2(dest[1] - origin[1], dest[0] - origin[0]);
        if (VectorDirections.ContainsKey(vectorDirection)) {
            Direction direction = VectorDirections[vectorDirection];
            return EditPath(currIndex, direction, true);
        }

        return currIndex;
    }

    static int EditPath(int currIndex, Direction direction, bool canAppend = false) {
        if (CheckPath(currIndex, direction)) {
            currIndex -= (int)direction;
        }
        else if (canAppend) {
            currIndex += (int)direction;
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

    // Snap the vector to the the four directions.
    public static Vector2 SnapVector(Vector2 vector, bool binary = true) {
        float maxProjection = 0f;
        Vector2 newVector = Vector2.zero;
        foreach(KeyValuePair<Vector2, Direction> vectorDirection in VectorDirections) {
            float projection = Vector2.Dot(vectorDirection.Key, vector);
            if (projection > maxProjection) {
                newVector = vectorDirection.Key;
            }
        }
        return newVector;
    }

}
