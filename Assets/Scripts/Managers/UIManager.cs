using System;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    static public Action OnReloadButton;

    [SerializeField] private UIPoint _startPoint;
    [SerializeField] private UIPoint _endPoint;
    [SerializeField] private TMP_Text _lengthText;

    [SerializeField] private GameObject _reloadPanel;

    void Start()
    {
        PathManager.OnStartCell += OnStartCell;
        PathManager.OnEndCell += OnEndCell;
        PathManager.OnPathFound += OnPathFound;
        PathManager.OnPathNotFound += OnPathNotFound;
    }

    

    private void OnDestroy()
    {
        PathManager.OnStartCell -= OnStartCell;
        PathManager.OnEndCell -= OnEndCell;
        PathManager.OnPathFound -= OnPathFound;
        PathManager.OnPathNotFound -= OnPathNotFound;
    }

    private void OnStartCell(ICell cell)
    {
        _startPoint.PlayAnimation();
        _endPoint.StopAnimation();

        _startPoint.SetText(FormatCoords(cell.GetCoordinatesInWorld()));
        _endPoint.SetText("");

        if (_lengthText != null)
        {
            _lengthText.text = "";
        }
    }

    private void OnEndCell(ICell cell)
    {
        _startPoint.StopAnimation();
        _endPoint.PlayAnimation();

        _endPoint.SetText(FormatCoords(cell.GetCoordinatesInWorld()));
    }


    private void OnPathNotFound(long time)
    {
        if (_lengthText != null)
        {
            _lengthText.text = $"Path not found!\n Time: {time}ms";

        }
    }

    private void OnPathFound(float length, long time)
    {
        if (_lengthText != null)
        {
            _lengthText.text = $"Length: {length} steps\n Time: {time}ms";

        }
    }

    private string FormatCoords(Vector3 coords)
    {
        return $"x:{coords.x:0.0}\r\nz:{coords.z:0.0}";
    }

 

    public void OnRestartClick()
    {
        OnReloadButton?.Invoke();
    }


    public void ShowPanel( GameObject panel )
    {
        panel.SetActive(true);
    }


    public void HidePanel(GameObject panel)
    {
        panel.SetActive(false );
    }

}
