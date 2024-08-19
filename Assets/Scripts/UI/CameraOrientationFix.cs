using UnityEngine;

public class CameraOrientationFix : MonoBehaviour
{
    private Camera perspectiveCamera;
    private float initialVerticalFOV;
    private float initialAspectRatio;
    public float targetFOVLandscape = 12f;
    public float targetFOVPortrait = 24f;

    void Start()
    {
        perspectiveCamera = GetComponent<Camera>();
        if (perspectiveCamera != null && !perspectiveCamera.orthographic)
        {
            initialVerticalFOV = perspectiveCamera.fieldOfView;
            initialAspectRatio = (float)Screen.width / Screen.height;
        }
    }

    void Update()
    {
        if (perspectiveCamera != null && !perspectiveCamera.orthographic)
        {
            float currentAspectRatio = (float)Screen.width / Screen.height;

            
            float landscapeAspectRatio = 16f / 9f;
            float portraitAspectRatio = 9f / 16f;

           
            float t;
            if (currentAspectRatio >= landscapeAspectRatio)
            {
                t = 0f;
            }
            else if (currentAspectRatio <= portraitAspectRatio)
            {
                t = 1f;
            }
            else
            {
                t = (currentAspectRatio - portraitAspectRatio) / (landscapeAspectRatio - portraitAspectRatio);
            }

           
            float targetFOV = Mathf.Lerp(targetFOVPortrait, targetFOVLandscape, t);
            perspectiveCamera.fieldOfView = targetFOV;
        }
    }
}
