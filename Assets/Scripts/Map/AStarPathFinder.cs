using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder : IPathFinder
{
    private readonly Stack<Dictionary<ICell, ICell>> _dictionaryPool = new Stack<Dictionary<ICell, ICell>>();
    private readonly Stack<Dictionary<ICell, int>> _costPool = new Stack<Dictionary<ICell, int>>();
    private readonly Stack<List<ICell>> _listPool = new Stack<List<ICell>>();
    private readonly List<ICell> _neighborBuffer = new List<ICell>();

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

                int newCost = costSoFar[current] + 1;
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
     
        return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
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


