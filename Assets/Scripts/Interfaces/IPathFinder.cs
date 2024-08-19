using System.Collections.Generic;

public interface IPathFinder
{
    IList<ICell> FindPathOnMap(ICell cellStart, ICell cellEnd, IMap map);
}