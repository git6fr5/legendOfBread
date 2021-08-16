using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry {

    /* --- Enums --- */
    public enum Shape {
        EMPTY,
        SQUARE,
        CROSS,
        shapeCount
    }

    /* --- Methods --- */
    // constructs the given shape border
    public static int[][] BorderGrid(Shape shape, int backgroundTileID, int fillTileID, int vertical, int horizontal, int vertBorder, int horBorder) {
        switch (shape) {
            case Shape.SQUARE:
                Debug.Log("Constructing Square");
                return Square(backgroundTileID, fillTileID, vertical, horizontal, vertBorder, horBorder);
            case Shape.CROSS:
                Debug.Log("Constructing Cross");
                return Cross(backgroundTileID, fillTileID, vertical, horizontal, vertBorder, horBorder);
            default:
                Debug.Log("Unknown Shape");
                return new int[0][];
        }
    }

    /* --- Methods --- */
    // creates a square sub grid
    public static int[][] Empty(int backgroundTileID, int fillTileID, int vertical, int horizontal) {
        // initialize the grid
        int[][] square = new int[vertical][];
        for (int i = 0; i < square.Length; i++) {
            square[i] = new int[horizontal];
            for (int j = 0; j < square[i].Length; j++) {
                square[i][j] = backgroundTileID;
            }
        }
        return square;
    }

    // creates a square sub grid
    public static int[][] Square(int backgroundTileID, int fillTileID, int vertical, int horizontal, int vertBorder, int horBorder) {
        // initialize the grid
        int[][] square = new int[vertical][];
        for (int i = 0; i < square.Length; i++) {
            square[i] = new int[horizontal];
        }
        for (int i = 0; i < vertical; i++) {
            for (int j = 0; j < vertBorder; j++) {
                square[i][j] = fillTileID;
                square[i][vertical - (j+1)] = fillTileID;
            }
        }
        for (int i = 0; i < horizontal; i++) {
            for (int j = 0; j < horBorder; j++) {
                square[j][i] = fillTileID;
                square[horizontal - (j+1)][i] = fillTileID;
            }
        }

        return square;
    }

    // creates a cross sub grid
    public static int[][] Cross(int backgroundTileID, int fillTileID, int vertical, int horizontal, int vertBorder, int horBorder) {
        // initialize the grid
        int vertCross = vertical - vertBorder;
        int horCross = horizontal - horBorder;
        int[][] cross = new int[vertical][];
        for (int i = 0; i < cross.Length; i++) {
            cross[i] = new int[horizontal];
            for (int j = 0; j < cross[i].Length; j++) {
                if (i > Mathf.Floor(vertCross / 4) && i < Mathf.Ceil(3 * vertCross / 4) + 1) { 
                    cross[i][j] = backgroundTileID;
                }
                else if (j > Mathf.Floor(horCross / 4) && j < Mathf.Ceil(3 * horCross / 4) + 1) {
                    cross[i][j] = backgroundTileID; 
                }
                else {
                    cross[i][j] = fillTileID;
                }
            }
        }
        return cross;
    }

}
