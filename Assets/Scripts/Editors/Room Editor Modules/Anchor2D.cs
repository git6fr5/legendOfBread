using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor2D
{
    /* --- CLASS --- */
    public Anchor2D(float _vertical, float _horizontal) {
        vertical = _vertical;
        horizontal = _horizontal;
    }

    /* --- STATIC --- */
    // center
    public static Anchor2D center = new Anchor2D(0.5f, 0.5f);
    // corners
    public static Anchor2D northEast = new Anchor2D(0f, 1f);
    public static Anchor2D northWest = new Anchor2D(0f, 0f);
    public static Anchor2D southEast = new Anchor2D(1f, 1f);
    public static Anchor2D southWest = new Anchor2D(1f, 0f);
    // cardinals
    public static Anchor2D north = new Anchor2D(0f, 0.5f);
    public static Anchor2D south = new Anchor2D(1f, 0.5f);
    public static Anchor2D east = new Anchor2D(0.5f, 0f);
    public static Anchor2D west = new Anchor2D(0.5f, 1f);
    public static Anchor2D[] anchors = new Anchor2D[] { center, north, northEast, east, southEast, south, southWest, west, northWest };

    /* --- VARIABLES --- */
    public float vertical; 
    public float horizontal;

    /* --- METHODS --- */
    // converts anchor point to a position on the grid
    public int[] AnchorToGrid(int[][] grid, int[][] subGrid) {
        int[] _anchor = new int[2];
        _anchor[0] = (int)(vertical * (grid.Length - subGrid.Length));
        _anchor[1] = (int)(horizontal * (grid[0].Length - subGrid[0].Length));
        return _anchor;
    }

    // converts a grid coordinate to an anchor point
    public static Anchor2D GridToAnchor(int[][] grid, int[] location) {
        float i = ((float)grid.Length + (float)location[0]) / (float)grid.Length;
        float j = (float)location[1] / (float)grid[0].Length;
        return new Anchor2D(i, j);
    }

}
