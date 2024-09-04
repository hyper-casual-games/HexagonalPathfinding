using UnityEngine;

public interface ICell
{
    int X { get; }
    int Y { get; }
    int Z { get; }
    bool IsWalkable { get; set; }
    Vector3 GetCoordinatesInWorld();
   
}
