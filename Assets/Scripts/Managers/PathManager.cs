using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;

public class PathManager : MonoBehaviour
{
    public static Action<ICell> OnStartCell;
    public static Action<ICell> OnEndCell;
    public static Action<float, long> OnPathFound;
    public static Action<long> OnPathNotFound;

    [SerializeField] private MapManager _mapManager;
    [SerializeField] private LayerMask _layerMask;

    private IPathFinder _pathFinder;
    private ICell startCell;
    private ICell goalCell;
    private Stopwatch _stopwatch;

  
    private List<ICell> _cachedPath = new List<ICell>();

   
    private bool _isPathfindingInProgress = false;

    private void Start()
    {
        _pathFinder = new AStarPathFinder();
        _stopwatch = new Stopwatch();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.touchCount > 0 ? Input.GetTouch(0).fingerId : -1)) return;

            Ray ray = Input.touchCount > 0
                ? Camera.main.ScreenPointToRay(Input.GetTouch(0).position)
                : Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
            {
                HexCell cell = hit.collider.GetComponent<HexCell>();
                if (cell != null && cell.IsWalkable)
                {
                    if (startCell == null)
                    {
                        startCell = cell;
                        OnStartCell?.Invoke(cell);
                        cell.Select();
                    }
                    else if (goalCell == null && !_isPathfindingInProgress)
                    {
                        goalCell = cell;
                        OnEndCell?.Invoke(cell);
                        cell.Select();
                        StartPathfinding();
                    }
                    else
                    {
                        startCell = null;
                        goalCell = null;
                    }
                }
            }
        }
    }

    private void StartPathfinding()
    {
        _isPathfindingInProgress = true; 
        FindPath();
        _isPathfindingInProgress = false; 
    }

    private void FindPath()
    {
        if (startCell != null && goalCell != null)
        {
            _stopwatch.Start(); 

          
            _cachedPath.Clear();

            IList<ICell> path = _pathFinder.FindPathOnMap(startCell, goalCell, _mapManager.GetHexMap());

            _stopwatch.Stop();

            if (path.Count > 0)
            {
                _cachedPath.AddRange(path); 
                _mapManager.HighlightPath(_cachedPath);
                OnPathFound?.Invoke(_cachedPath.Count, _stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _mapManager.ClearPaths();
                OnPathNotFound?.Invoke(_stopwatch.ElapsedMilliseconds);
            }
        }

        startCell = null;
        goalCell = null;
    }

    
}
