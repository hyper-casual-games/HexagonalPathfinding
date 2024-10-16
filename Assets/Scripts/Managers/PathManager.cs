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

    private IMap _map;
    private IPathFinder _pathFinder;

    private MapCell _startCell;
    private MapCell _goalCell;
    private Stopwatch _stopwatch;

    private List<ICell> _cachedPath = new List<ICell>();

    private bool _isPathfindingInProgress = false;

    private void Start()
    {
        _map = _mapManager.GetMap();
        _pathFinder = new AStarPathFinder();
        _stopwatch = new Stopwatch();
    }

    private void Update()
    {
        if (IsInputDetected())
        {
            MapCell cell = GetClickedCell();
            if (cell != null)
            {
                HandleCellInteraction(cell);
            }
        }
    }

    private bool IsInputDetected()
    {
        return Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    }

    private MapCell GetClickedCell()
    {
        if (EventSystem.current.IsPointerOverGameObject(Input.touchCount > 0 ? Input.GetTouch(0).fingerId : -1))
            return null;

        Ray ray = Input.touchCount > 0
            ? Camera.main.ScreenPointToRay(Input.GetTouch(0).position)
            : Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
        {
            return hit.collider.GetComponent<MapCell>();
        }

        return null;
    }

    private void HandleCellInteraction(MapCell cell)
    {
        if (Input.GetKey(KeyCode.S))
        {
            SetCellWalkable(cell);
        }
        else if (Input.GetKey(KeyCode.M))
        {
            SetStartOrGoalCell(cell);
        }
        else
        {
            SelectStartOrGoalCell(cell);
        }
    }

    private void SetCellWalkable(MapCell cell)
    {
        cell.IsWalkable = true;
        cell.UpdateColor();
    }

    private void SetStartOrGoalCell(MapCell cell)
    {
        if (!cell.IsWalkable) return;

        cell.IsWalkable = true;
        cell.UpdateColor();

        _startCell = cell;
        OnStartCell?.Invoke(cell);
        _startCell.Select();

        _goalCell = (MapCell)_map.GetCell(cell.X, 0);
        _goalCell.Select();
        OnEndCell?.Invoke(_goalCell);

        StartPathfinding();
    }

    private void SelectStartOrGoalCell(MapCell cell)
    {
        if (!cell.IsWalkable) return;

        if (_startCell == null)
        {
            _startCell = cell;
            OnStartCell?.Invoke(cell);
            cell.Select();
        }
        else if (_goalCell == null && !_isPathfindingInProgress)
        {
            _goalCell = cell;
            OnEndCell?.Invoke(cell);
            cell.Select();
            StartPathfinding();
        }
        else
        {
            _startCell = null;
            _goalCell = null;
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
        if (_startCell != null && _goalCell != null)
        {
            _stopwatch.Start();

            _cachedPath.Clear();

            IList<ICell> path = _pathFinder.FindPathOnMap(_startCell, _goalCell, _map);

            _stopwatch.Stop();

            if (path.Count > 1)
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

        _startCell = null;
        _goalCell = null;
    }
}
