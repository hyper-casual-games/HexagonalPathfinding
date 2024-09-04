using System;
using System.Collections;
using UnityEngine;

public class SquareCell : MapCell
{
    public void Init(int x, int y, bool isWalkable)
    {
        X = x;
        Y = y;
        IsWalkable = isWalkable;
        UpdateColor();
    }


    public new Vector3 GetCoordinatesInWorld()
    {
        return SquareMap.SquareToWorld(X, Y);
    }



}
