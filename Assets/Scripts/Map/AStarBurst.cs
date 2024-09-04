using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;


[BurstCompile]
public struct Cell
{
    public int X;
    public int Y;
    public bool IsWalkable;
}

[BurstCompile]
public struct AStarPathFinderBurst
{
    private const int CardinalCost = 1;
    private const int DiagonalCost = 14;

    public NativeList<Cell> FindPathOnMap(Cell start, Cell end, NativeArray<Cell> map, int mapWidth, int mapHeight, bool isSquareMap, Allocator allocator)
    {
        NativeList<Cell> path = new NativeList<Cell>(allocator);
        NativeList<Cell> neighbors = new NativeList<Cell>(Allocator.Temp);
        NativeHashMap<int2, int2> cameFrom = new NativeHashMap<int2, int2>(map.Length, Allocator.Temp);
        NativeHashMap<int2, int> costSoFar = new NativeHashMap<int2, int>(map.Length, Allocator.Temp);
        NativePriorityQueue frontier = new NativePriorityQueue(Allocator.Temp);

        int2 startPos = new int2(start.X, start.Y);
        int2 endPos = new int2(end.X, end.Y);

        frontier.Enqueue(startPos, 0);
        cameFrom[startPos] = startPos;
        costSoFar[startPos] = 0;

        while (frontier.Count > 0)
        {
            int2 current = frontier.Dequeue();

            if (current.Equals(endPos))
            {
                break;
            }

            GetNeighbors(current, map, mapWidth, mapHeight, neighbors, allocator);

            for (int i = 0; i < neighbors.Length; i++)
            {
                Cell next = neighbors[i];
                if (!next.IsWalkable) continue;

                int moveCost = CardinalCost;
                if (isSquareMap && IsDiagonalMove(current, new int2(next.X, next.Y)))
                {
                    moveCost = DiagonalCost;
                }

                int newCost = costSoFar[current] + moveCost;

                int2 nextPos = new int2(next.X, next.Y);
                if (!costSoFar.TryGetValue(nextPos, out int oldCost) || newCost < oldCost)
                {
                    costSoFar[nextPos] = newCost;
                    int priority = newCost + Heuristic(nextPos, endPos);
                    frontier.Enqueue(nextPos, priority);
                    cameFrom[nextPos] = current;
                }
            }
        }

        ReconstructPath(cameFrom, startPos, endPos, path);

        // Clean up
        neighbors.Dispose();
        cameFrom.Dispose();
        costSoFar.Dispose();
        frontier.Dispose();

        return path;
    }

    private void GetNeighbors(int2 current, NativeArray<Cell> map, int mapWidth, int mapHeight, NativeList<Cell> neighbors, Allocator allocator)
    {
        neighbors.Clear();

        int2[] directions = new int2[]
        {
            new int2(1, 0), new int2(-1, 0), new int2(0, 1), new int2(0, -1), // Cardinal directions
            new int2(1, 1), new int2(-1, -1), new int2(1, -1), new int2(-1, 1) // Diagonal directions
        };

        foreach (var direction in directions)
        {
            int2 neighborPos = current + direction;
            if (neighborPos.x >= 0 && neighborPos.y >= 0 && neighborPos.x < mapWidth && neighborPos.y < mapHeight)
            {
                neighbors.Add(map[neighborPos.y * mapWidth + neighborPos.x]);
            }
        }
    }

    private int Heuristic(int2 a, int2 b)
    {
        return math.abs(a.x - b.x) + math.abs(a.y - b.y); // Manhattan distance
    }

    private bool IsDiagonalMove(int2 current, int2 next)
    {
        return math.abs(current.x - next.x) == 1 && math.abs(current.y - next.y) == 1;
    }

    private void ReconstructPath(NativeHashMap<int2, int2> cameFrom, int2 start, int2 end, NativeList<Cell> path)
    {
        int2 current = end;

        while (!current.Equals(start))
        {
            if (!cameFrom.TryGetValue(current, out int2 previous))
            {
                path.Clear(); // If path is invalid, clear it.
                return;
            }
            path.Add(new Cell { X = current.x, Y = current.y });
            current = previous;
        }

        path.Add(new Cell { X = start.x, Y = start.y });

        ReversePath(path);
    }

    private void ReversePath(NativeList<Cell> path)
    {
        int count = path.Length;
        for (int i = 0; i < count / 2; i++)
        {
            // Swap elements at index i and count - i - 1
            Cell temp = path[i];
            path[i] = path[count - i - 1];
            path[count - i - 1] = temp;
        }
    }

}


public struct NativePriorityQueue : System.IDisposable
{
    private NativeList<int2> _elements; // Stores the position (X, Y)
    private NativeList<int> _priorities; // Stores the priority of each element

    public NativePriorityQueue(Allocator allocator)
    {
        _elements = new NativeList<int2>(allocator);
        _priorities = new NativeList<int>(allocator);
    }

    public int Count => _elements.Length;

    public void Enqueue(int2 element, int priority)
    {
        _elements.Add(element);
        _priorities.Add(priority);
        HeapifyUp(_elements.Length - 1);
    }

    public int2 Dequeue()
    {
        int2 firstElement = _elements[0];
        int lastIndex = _elements.Length - 1;

        _elements[0] = _elements[lastIndex];
        _priorities[0] = _priorities[lastIndex];

        _elements.RemoveAt(lastIndex);
        _priorities.RemoveAt(lastIndex);

        HeapifyDown(0);

        return firstElement;
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (_priorities[parentIndex] <= _priorities[index])
                break;

            // Swap parent and current index
            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    private void HeapifyDown(int index)
    {
        int lastIndex = _elements.Length - 1;

        while (true)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;
            int smallestIndex = index;

            if (leftChildIndex <= lastIndex && _priorities[leftChildIndex] < _priorities[smallestIndex])
            {
                smallestIndex = leftChildIndex;
            }

            if (rightChildIndex <= lastIndex && _priorities[rightChildIndex] < _priorities[smallestIndex])
            {
                smallestIndex = rightChildIndex;
            }

            if (smallestIndex == index)
            {
                break;
            }

            Swap(index, smallestIndex);
            index = smallestIndex;
        }
    }

    private void Swap(int indexA, int indexB)
    {
        int2 tempElement = _elements[indexA];
        int tempPriority = _priorities[indexA];

        _elements[indexA] = _elements[indexB];
        _priorities[indexA] = _priorities[indexB];

        _elements[indexB] = tempElement;
        _priorities[indexB] = tempPriority;
    }

    public void Dispose()
    {
        if (_elements.IsCreated) _elements.Dispose();
        if (_priorities.IsCreated) _priorities.Dispose();
    }
}
