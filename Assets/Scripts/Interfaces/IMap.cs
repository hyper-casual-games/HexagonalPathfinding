using System.Collections.Generic;

public interface IMap
{
    void AddCell(ICell cell);
    ICell GetCell(int x, int y);
    IEnumerable<ICell> GetNeighbors(ICell cell);
    void Clear();
    bool IsSquareMap();
}
