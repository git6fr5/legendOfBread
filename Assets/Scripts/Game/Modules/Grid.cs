using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    //// mouse click to grid coordinate
    //public int[] ClickToGrid() {
    //    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    return PointToGrid(mousePos);
    //}

    //// checks if a coordinate is in the grid
    //public bool PointInGrid(int[] point) {
    //    bool isInGrid = (point[1] < sizeHorizontal && point[1] >= 0 && point[0] < sizeVertical && point[0] >= 0);
    //    if (!isInGrid) {
    //        // print(point[0] + ", " + point[1] + " was not in the grid");
    //    }
    //    return isInGrid;
    //}

    //// checks if a coordinate is in the grid
    //// and also not on the border
    //public bool PointWithinBorders(int[] point) {
    //    bool isInHor = (point[1] < sizeHorizontal && point[1] >= 0);
    //    bool isInVert = (point[0] < sizeVertical && point[0] >= 0);
    //    bool isInGrid = (isInHor && isInVert);
    //    if (isInGrid) {
    //        // print(point[0] + ", " + point[1] + " was not within the grid");
    //        if (borderGrid[point[0]][point[1]] != (int)Tiles.EMPTY) {
    //            isInGrid = false;
    //        }
    //    }
    //    return isInGrid;
    //}

    //// a given point to grid coordinates 
    //public int[] PointToGrid(Vector2 point) {
    //    int i = (int)(-point.y + vertOffset);
    //    int j = (int)(point.x + horOffset);
    //    //print(i + ", " + j);
    //    return new int[] { i, j };
    //}

    //// grid coordinate to tile map position
    //public Vector3Int GridToTileMap(int i, int j) {
    //    return new Vector3Int(j - horOffset, -(i - vertOffset + 1), 0);
    //}


}
