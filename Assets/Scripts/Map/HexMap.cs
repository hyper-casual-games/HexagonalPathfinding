using System.Collections.Generic;
using UnityEngine;

public class HexMap : IMap
{
    private readonly Dictionary<(int, int), ICell> _cells = new Dictionary<(int, int), ICell>();
    private static readonly (int, int)[] _directions = new (int, int)[]
    {
        (1, 0), (1, -1), (0, -1), (-1, 0), (-1, 1), (0, 1)
    };

    private const float size = 1f; // Adjust size as needed
    private static readonly float width = Mathf.Sqrt(3) * size;
    private static readonly float height = 2 * size;

    public static Vector3 HexToWorld(int q, int r)
    {
        float x = width * (q + r / 2f);
        float z = height * (r * 3f / 4f);
        return new Vector3(x, 0, z);
    }

    public void AddCell(ICell cell)
    {
        _cells[(cell.X, cell.Y)] = cell;
    }

    public ICell GetCell(int x, int y)
    {
        _cells.TryGetValue((x, y), out ICell cell);
        return cell;
    }

    public IEnumerable<ICell> GetNeighbors(ICell cell)
    {
        foreach (var direction in _directions)
        {
            var neighborQ = cell.X + direction.Item1;
            var neighborR = cell.Y + direction.Item2;
            if (_cells.TryGetValue((neighborQ, neighborR), out var neighbor))
            {
                yield return neighbor;
            }
        }
    }

    public void Clear()
    {
        _cells.Clear();
    }

    public bool IsSquareMap()
    {
       return false;
    }
}



