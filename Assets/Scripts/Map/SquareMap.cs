using System.Collections.Generic;
using UnityEngine;

public class SquareMap : IMap
{
    
    private readonly Dictionary<(int, int), ICell> _cells = new Dictionary<(int, int), ICell>();

    
    private static readonly (int, int)[] _cardinalDirections = new (int, int)[]
    {
        (1, 0), (0, 1), (-1, 0), (0, -1)  // Right, Up, Left, Down
    };

   
    private static readonly (int, int)[] _diagonalDirections = new (int, int)[]
    {
        (1, 1), (-1, 1), (-1, -1), (1, -1) // Top-right, Top-left, Bottom-left, Bottom-right
    };

    private const float size = 2f; 

    
    private bool _allowDiagonal;


    public static float GetCellSize() {
        return size;
    }

    public SquareMap(bool allowDiagonal = false)
    {
        _allowDiagonal = allowDiagonal;
    }

    
    public static Vector3 SquareToWorld(int x, int y)
    {
        
        float worldX = x * size;
        float worldZ = y * size;
        return new Vector3(worldX, 0, worldZ);
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
        // Start by checking cardinal neighbors
        foreach (var direction in _cardinalDirections)
        {
            var neighborX = cell.X + direction.Item1;
            var neighborY = cell.Y + direction.Item2;
            if (_cells.TryGetValue((neighborX, neighborY), out var neighbor))
            {
                yield return neighbor;
            }
        }

        // If diagonal movement is allowed, check diagonal neighbors as well
        if (_allowDiagonal)
        {
            foreach (var direction in _diagonalDirections)
            {
                var neighborX = cell.X + direction.Item1;
                var neighborY = cell.Y + direction.Item2;
                if (_cells.TryGetValue((neighborX, neighborY), out var neighbor))
                {
                    yield return neighbor;
                }
            }
        }
    }

   
    public void Clear()
    {
        _cells.Clear();
    }
}
