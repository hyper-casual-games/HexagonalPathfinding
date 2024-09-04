using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Hex Grid Settings")]
    [SerializeField] private GameObject _hexCellPrefab;
    [SerializeField, Range(3, 15)] private int _mapRadius = 5;
    [SerializeField, Range(0.1f, 0.5f)] private float _hexObstacleProbability = 0.25f;

    [Header("Square Grid Settings")]
    [SerializeField] private GameObject _baseCellPrefab;
    [SerializeField, Range(3, 15)] private int _mapWidth = 5;
    [SerializeField, Range(3, 15)] private int _mapHeight = 5;
    [SerializeField, Range(0.1f, 0.5f)] private float _squareObstacleProbability = 0.25f;
    [SerializeField] private bool _squareDiagonalSearch = false;

    [Header("General Settings")]
    [SerializeField, Range(0.0f, 0.05f)] private float _stepDelay = 0.02f;
    [SerializeField] private bool _generateHexMap = true;  // Toggle between hex and square map generation

    private WaitForSeconds _waitForSeconds;

    private IMap _hexMap;
    private IMap _squareMap;

    private void Awake()
    {
        PathManager.OnStartCell += OnStartCell;
    }

    private void OnDestroy()
    {
        PathManager.OnStartCell -= OnStartCell;
    }

    void Start()
    {
        _waitForSeconds = new WaitForSeconds(_stepDelay);
        _hexMap = new HexMap();
        _squareMap = new SquareMap(_squareDiagonalSearch);
        GenerateMap();
    }

    private void OnStartCell(ICell cell)
    {
        ClearPaths();
    }

    public void HighlightPath(IList<ICell> path)
    {
        StartCoroutine(HighlightPathWithDelay(path));
    }

    private IEnumerator HighlightPathWithDelay(IList<ICell> path)
    {
        foreach (MapCell cell in path)
        {
            cell.Select();
            yield return _waitForSeconds;
        }
    }

    public void ClearPaths()
    {
        foreach (Transform child in transform)
        {
            MapCell cell = child.GetComponent<MapCell>();

            if (!cell.IsWalkable) continue;

            cell.Deselect();
        }
    }

    void GenerateMap()
    {
        if (_generateHexMap)
        {
            GenerateHexMapByRadius();
        }
        else
        {
            GenerateSquareMap();
        }
    }

    // Generate the hex map based on the map radius
    void GenerateHexMapByRadius()
    {
        for (int q = -_mapRadius; q <= _mapRadius; q++)
        {
            for (int r = Mathf.Max(-_mapRadius, -q - _mapRadius); r <= Mathf.Min(_mapRadius, -q + _mapRadius); r++)
            {
                bool isWalkable = Random.value > _hexObstacleProbability;
                CreateHexCell(q, r, isWalkable);
            }
        }
    }

    // Create a hexagonal cell
    void CreateHexCell(int q, int r, bool isWalkable)
    {
        Vector3 position = HexMap.HexToWorld(q, r);
        GameObject cellObj = Instantiate(_hexCellPrefab, position, Quaternion.identity, transform);
        HexCell cell = cellObj.GetComponent<HexCell>();
        cell.Init(q, r, isWalkable);
        _hexMap.AddCell(cell);
    }

    // Generate the square map based on width and height
    void GenerateSquareMap()
    {
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                bool isWalkable = Random.value > _squareObstacleProbability;

                if (y == 0) isWalkable = true;

                CreateSquareCell(x, y, isWalkable);
            }
        }
    }

    // Create a square cell
    void CreateSquareCell(int x, int y, bool isWalkable)
    {
        // Calculate the center of the grid
        float gridWidth = _mapWidth * SquareMap.GetCellSize();
        float gridHeight = _mapHeight * SquareMap.GetCellSize();

        // Calculate the world position for this cell, offset to center the grid
        Vector3 position = SquareMap.SquareToWorld(x, y);
        position.x -= gridWidth / 2f - SquareMap.GetCellSize() / 2f;
        position.z -= gridHeight / 2f - SquareMap.GetCellSize() / 2f;


      
        GameObject cellObj = Instantiate(_baseCellPrefab, position, Quaternion.identity, transform);
        SquareCell cell = cellObj.GetComponent<SquareCell>();
        cell.Init(x, y, isWalkable);
        _squareMap.AddCell(cell);
    }

    public void ClearMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (_generateHexMap)
        {
            _hexMap.Clear();
        }
        else
        {
            _squareMap.Clear();
        }
    }

    public void ReloadMap()
    {
        ClearMap();
        GenerateMap();
    }

    public IMap GetMap()
    {
        if (_generateHexMap)
        {
            return _hexMap;
        }
        else
        {
            return _squareMap;
        }
     
    }

    

    
}
