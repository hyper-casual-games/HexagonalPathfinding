using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder : IPathFinder
{
    private readonly Stack<Dictionary<ICell, ICell>> _dictionaryPool = new Stack<Dictionary<ICell, ICell>>();
    private readonly Stack<Dictionary<ICell, int>> _costPool = new Stack<Dictionary<ICell, int>>();
    private readonly Stack<List<ICell>> _listPool = new Stack<List<ICell>>(256);
    private readonly List<ICell> _neighborBuffer = new List<ICell>(512);

    private const int CardinalCost = 1; // Cost for cardinal (up, down, left, right) movement
    private const int DiagonalCost = 14; // Cost for diagonal movement (approximated by 1.414 * 10 for integer math)

    public IList<ICell> FindPathOnMap(ICell cellStart, ICell cellEnd, IMap map)
    {
        PriorityQueue<ICell> frontier = new PriorityQueue<ICell>();
        frontier.Enqueue(cellStart, 0);

        var cameFrom = GetDictionaryFromPool();
        var costSoFar = GetCostDictionaryFromPool();

        cameFrom[cellStart] = cellStart;
        costSoFar[cellStart] = 0;

        while (frontier.Count > 0)
        {
            ICell current = frontier.Dequeue();

            if (current.Equals(cellEnd))
            {
                break;
            }

            GetNeighbors(map, current, _neighborBuffer);
            foreach (ICell next in _neighborBuffer)
            {
                if (!next.IsWalkable) continue;

                // Check if the map is a square map
                int moveCost = CardinalCost;
                if (map.IsSquareMap() && IsDiagonalMove(current, next))
                {
                    // If it's a diagonal move in a square map, use the higher diagonal cost
                    moveCost = DiagonalCost;
                }

                int newCost = costSoFar[current] + moveCost;

                if (!costSoFar.TryGetValue(next, out int oldCost) || newCost < oldCost)
                {
                    costSoFar[next] = newCost;
                    int priority = newCost + Heuristic(next, cellEnd);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        var path = ReconstructPath(cameFrom, cellStart, cellEnd);

        ReturnDictionaryToPool(cameFrom);
        ReturnCostDictionaryToPool(costSoFar);

        return path;
    }

    private void GetNeighbors(IMap map, ICell current, List<ICell> neighbors)
    {
        neighbors.Clear();
        neighbors.AddRange(map.GetNeighbors(current));
    }

    private int Heuristic(ICell a, ICell b)
    {
        // Manhattan distance heuristic for a square grid
        return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
    }

    private bool IsDiagonalMove(ICell current, ICell next)
    {
        
        // Check if the movement is diagonal by comparing the changes in x and y
        return Mathf.Abs(current.X - next.X) == 1 && Mathf.Abs(current.Y - next.Y) == 1;
    }

    private List<ICell> ReconstructPath(Dictionary<ICell, ICell> cameFrom, ICell start, ICell end)
    {
        var path = GetListFromPool();

        ICell current = end;
        while (current != null && !current.Equals(start))
        {
            path.Add(current);

            if (!cameFrom.ContainsKey(current))
            {
                path.Clear();
                break;
            }

            current = cameFrom[current];
        }

        if (current != null)
        {
            path.Add(start);
            path.Reverse();
        }

        return path;
    }

    private Dictionary<ICell, ICell> GetDictionaryFromPool()
    {
        return _dictionaryPool.Count > 0 ? _dictionaryPool.Pop() : new Dictionary<ICell, ICell>();
    }

    private void ReturnDictionaryToPool(Dictionary<ICell, ICell> dictionary)
    {
        dictionary.Clear();
        _dictionaryPool.Push(dictionary);
    }

    private Dictionary<ICell, int> GetCostDictionaryFromPool()
    {
        return _costPool.Count > 0 ? _costPool.Pop() : new Dictionary<ICell, int>();
    }

    private void ReturnCostDictionaryToPool(Dictionary<ICell, int> dictionary)
    {
        dictionary.Clear();
        _costPool.Push(dictionary);
    }

    private List<ICell> GetListFromPool()
    {
        return _listPool.Count > 0 ? _listPool.Pop() : new List<ICell>();
    }

    private void ReturnListToPool(List<ICell> list)
    {
        list.Clear();
        _listPool.Push(list);
    }
}


