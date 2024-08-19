using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerFix : MonoBehaviour
{
    public CanvasScaler canvasScaler;
    private float landscapeWidth = 1920f;
    private float landscapeHeight = 1080f;
    private float portraitWidth = 1080f;
    private float portraitHeight = 1920f;

    void Start()
    {
        AdjustCanvasScaler();
    }

    void Update()
    {
        AdjustCanvasScaler();
    }

    private void AdjustCanvasScaler()
    {
        if (Screen.width > Screen.height)
        {
            canvasScaler.referenceResolution = new Vector2(landscapeWidth, landscapeHeight);
            canvasScaler.matchWidthOrHeight = 0.5f;
        }
        else // Portrait
        {
            canvasScaler.referenceResolution = new Vector2(portraitWidth, portraitHeight);
            canvasScaler.matchWidthOrHeight = 0.5f; 
        }
    }
}
