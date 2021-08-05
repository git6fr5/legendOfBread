using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry2D {

    /* --- ENUMS --- */
    public enum Shape {
        SQUARE, 
        HORIZONTAL_EVEN_RECTANGLES,
        ELLIPSE, 
        TRIANGLE,
        shapeCount
    }

    /* --- METHODS --- */
    // constructs the given shape
    public static int[][] ConstructShape(Shape shape, int backgroundTileID, int fillTileID, int vertical, int horizontal) {
        switch (shape) {
            case Shape.SQUARE:
                Debug.Log("Constructing Square");
                return Square(backgroundTileID, fillTileID, vertical, horizontal);
            case Shape.HORIZONTAL_EVEN_RECTANGLES:
                Debug.Log("Constructing Square");
                return HorizontalEvenRectangles(backgroundTileID, fillTileID, vertical, horizontal);
            case Shape.ELLIPSE:
                Debug.Log("Constructing Ellipse");
                return Ellipse(backgroundTileID, fillTileID, vertical, horizontal);
            case Shape.TRIANGLE:
                Debug.Log("Constructing Triangle");
                return Triangle(backgroundTileID, fillTileID, vertical, horizontal);
            default:
                Debug.Log("Unknown Shape");
                return new int[0][];
        }
    }

    // constructs the given shape
    public static int[][] ConstructRoom(DungeonEditor.Rooms room, int backgroundTileID, int fillTileID, int vertical, int horizontal) {
        switch (room) {
            case DungeonEditor.Rooms.BASIC:
                Debug.Log("Constructing Square");
                return Square(backgroundTileID, fillTileID, vertical, horizontal);
            default:
                return Empty(backgroundTileID, fillTileID, vertical, horizontal);
        }
    }

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
    public static int[][] Square(int backgroundTileID, int fillTileID, int vertical, int horizontal) {
        // initialize the grid
        int[][] square = new int[vertical][];
        for (int i = 0; i < square.Length; i++) {
            square[i] = new int[horizontal];
            for (int j = 0; j < square[i].Length; j++) {
                square[i][j] = fillTileID;
            }
        }
        return square;
    }

    // creates a square sub grid
    public static int[][] HorizontalEvenRectangles(int backgroundTileID, int fillTileID, int vertical, int horizontal) {
        // initialize the grid
        int[][] horizontalEvenRect = new int[vertical][];
        for (int i = 0; i < horizontalEvenRect.Length; i++) {
            horizontalEvenRect[i] = new int[horizontal];
            for (int j = 0; j < horizontalEvenRect[i].Length; j++) {
                horizontalEvenRect[i][j] = backgroundTileID;
            }
        }

        int boundary = (int)Mathf.Floor(vertical / 2);
        for (int i = 0; i < boundary - 1; i++) {
            for (int j = 0; j < horizontalEvenRect[i].Length; j++) {
                horizontalEvenRect[i][j] = fillTileID;
            }
        }
        for (int i = boundary+1; i < horizontalEvenRect.Length; i++) {
            for (int j = 0; j < horizontalEvenRect[i].Length; j++) {
                horizontalEvenRect[i][j] = fillTileID;
            }
        }

        return horizontalEvenRect;
    }

    // creates an ellipse sub grid
    public static int[][] Ellipse(int backgroundTileID, int fillTileID, int vertical, int horizontal) {
        // initialize the grid
        int[][] ellipse = new int[vertical][];
        for (int i = 0; i < ellipse.Length; i++) {
            ellipse[i] = new int[horizontal];
            for (int j = 0; j < ellipse[i].Length; j++) {
                ellipse[i][j] = backgroundTileID;
            }
        }
        // the major and minor axis radii
        float a = (float)horizontal / 2;
        float b = (float)vertical / 2;
        // draw the ellipse grid
        for (int i = 0; i < vertical; i++) {
            for (int j = 0; j < horizontal; j++) {
                float x = (float)j - a;
                float y = (float)i - b;
                float ellipticalBoundary = (x * x / (a * a)) + (y * y / (b * b));
                if (Mathf.Abs(ellipticalBoundary) < 1) { ellipse[i][j] = fillTileID; }
            }
        }
        return ellipse;
    }

    // creates a triangle sub grid
    public static int[][] Triangle(int backgroundTileID, int fillTileID, int vertical, int horizontal) {
        // initialize the grid
        int[][] triangle = new int[vertical][];
        for (int i = 0; i < triangle.Length; i++) {
            triangle[i] = new int[horizontal];
            for (int j = 0; j < triangle[i].Length; j++) {
                triangle[i][j] = backgroundTileID;
            }
        }
        // the side lengths
        float center = (int)(horizontal);
        float ratioA = (float)vertical / center;
        float ratioB = (float)vertical / (horizontal - center);
        // draw the triangle
        for (int i = 0; i < vertical; i++) {
            for (int j = 0; j < horizontal; j++) {
                if (j < center && (vertical - i) < ratioA * j) { triangle[i][j] = fillTileID; }
                if (j >= center && i > ratioB * j + center) { triangle[i][j] = fillTileID; }
            }
        }
        return triangle;
    }

    // draws the boundaries of a shape
    void Boundaries(int[][] shape) {
        for (int i = 0; i < shape.Length; i++) {
            for (int j = 0; j < shape[0].Length; j++) {

            }
        }
    }

    // draws the corners of a shape
    void Corners(int[][] shape) {
        for (int i = 0; i < shape.Length; i++) {
            for (int j = 0; j < shape[0].Length; j++) {

            }
        }
    }
}
