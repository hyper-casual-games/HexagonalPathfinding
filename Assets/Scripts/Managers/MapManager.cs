using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Hex Cell Prefab")]
    [SerializeField] private GameObject _hexCellPrefab;

    [Header("Radius-Based Hex Grid Settings")]

    [SerializeField, Range(3, 15)] private int _mapRadius = 5;
    [SerializeField, Range(0.1f, 0.5f)] private float _obstacleProbability = 0.25f;

    [SerializeField, Range(0.0f, 0.05f)] private float _stepDelay = 0.02f;

    private WaitForSeconds _waitForSeconds;
    private HexMap _hexMap;


    void Start()
    {
        _hexMap = new HexMap();
        _waitForSeconds = new WaitForSeconds(_stepDelay);

        GenerateMap();
    }

    private void Awake()
    {
        PathManager.OnStartCell += OnStartCell;
      
    }

 
    private void OnDestroy()
    {
        PathManager.OnStartCell -= OnStartCell;
     
    }

    private void OnStartCell(ICell cell)
    {
        ClearPaths();
    }


    public void HighlightPath(IList<ICell> path) {

        StartCoroutine(HighlightPathWithDelay(path));
    }

    private IEnumerator HighlightPathWithDelay(IList<ICell> path)
    {
        foreach (HexCell cell in path)
        {
            cell.Select();

            yield return _waitForSeconds;

        }


    }


    public void ClearPaths()
    {
        foreach (Transform child in transform)
        {
            HexCell hexCell = child.GetComponent<HexCell>();

            if (!hexCell.IsWalkable) continue;

            hexCell.Deselect();

        }
    }

    void GenerateMap()
    {
        
        GenerateMapByRadius();
       
    }

    void GenerateMapByRadius()
    {
        for (int q = -_mapRadius; q <= _mapRadius; q++)
        {
            for (int r = Mathf.Max(-_mapRadius, -q - _mapRadius); r <= Mathf.Min(_mapRadius, -q + _mapRadius); r++)
            {
                bool isWalkable = Random.value > _obstacleProbability;
                CreateCell(q, r, isWalkable);
            }
        }
    }

    void CreateCell(int q, int r, bool isWalkable)
    {
        Vector3 position = HexMap.HexToWorld(q, r);
        GameObject cellObj = Instantiate(_hexCellPrefab, position, Quaternion.identity, transform);
        HexCell cell = cellObj.GetComponent<HexCell>();
        cell.Init(q, r, isWalkable);
        _hexMap.AddCell(cell);
    }

    public void ClearMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        _hexMap.Clear();
       
    }

    public void ReloadMap()
    {
        ClearMap();
        GenerateMap();
    }

    public HexMap GetHexMap()
    {
        return _hexMap;
    }
}


