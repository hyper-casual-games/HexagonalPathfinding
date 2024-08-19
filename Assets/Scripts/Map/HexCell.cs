using System;
using System.Collections;
using UnityEngine;

public class HexCell : MonoBehaviour, ICell
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private float _moveDistance = 0.5f;
    [SerializeField] private float _moveUpDuration = 0.2f;
    [SerializeField] private float _moveDownDuration = 0.5f;
    [SerializeField] private Color _selectColor = Color.red;
    [SerializeField] private Color _deselectColor = Color.white;

    private Color _currentColor;
    private IEnumerator _moveCoroutine;
    private IEnumerator _colorChangeCoroutine;

    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z => -X - Y;
    public bool IsWalkable { get; set; } = true;

    public void Init(int x, int y, bool isWalkable)
    {
        X = x;
        Y = y;
        IsWalkable = isWalkable;
        UpdateColor();
    }

    override public string ToString()
    {
        return $"{X}x{Y}";
    }

    public Vector3 GetCoordinatesInWorld()
    {
        return HexMap.HexToWorld(X, Y);
    }

    public void UpdateColor()
    {
        Color newColor = IsWalkable ? Color.white : Color.gray;
        if (_currentColor != newColor)
        {
            _currentColor = newColor;
            if (_meshRenderer != null)
            {
                _meshRenderer.material.color = newColor;
            }
        }
    }

    public MeshRenderer GetMeshRenderer()
    {
        return _meshRenderer;
    }

    public void Select()
    {
        //StartMoveUp();
        //StartColorChange(_selectColor, (_moveUpDuration + _moveDownDuration) / 2);

        _meshRenderer.material.color = _selectColor;

    }

    public void Deselect()
    {
        //StartMoveDown();
        //StartColorChange(_deselectColor, (_moveUpDuration + _moveDownDuration) / 2);

        _meshRenderer.material.color = _deselectColor;
    }

    private void StartColorChange(Color targetColor, float duration)
    {
        if (_colorChangeCoroutine != null)
        {
            StopCoroutine(_colorChangeCoroutine);
        }
        _colorChangeCoroutine = ChangeColorCoroutine(targetColor, duration);
        StartCoroutine(_colorChangeCoroutine);
    }

    private IEnumerator ChangeColorCoroutine(Color targetColor, float duration)
    {
        Color startColor = _meshRenderer.material.color;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            _meshRenderer.material.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _meshRenderer.material.color = targetColor;
    }

    private void StartMove(Vector3 endPos, float moveDuration, Action callback)
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
        }
        _moveCoroutine = MoveCoroutine(endPos, moveDuration, callback);
        StartCoroutine(_moveCoroutine);
    }

    private IEnumerator MoveCoroutine(Vector3 endPos, float moveDuration, Action callback)
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        callback?.Invoke();
    }

    public void StartMoveUp()
    {
        StartMove(new Vector3(transform.position.x, _moveDistance, transform.position.z), _moveUpDuration, StartMoveDown);
    }

    public void StartMoveDown()
    {
        StartMove(new Vector3(transform.position.x, 0, transform.position.z), _moveDownDuration, null);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
