using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
   
    private void Awake()
    {
        Application.targetFrameRate = 60;

        UIManager.OnReloadButton += OnReload;
    }

    private void OnDestroy()
    {
        UIManager.OnReloadButton -= OnReload;
    }

    public void OnReload()
    {
        StartCoroutine(ReloadSceneAsync());
    }

    private IEnumerator ReloadSceneAsync()
    {
        
        string currentSceneName = SceneManager.GetActiveScene().name;      
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentSceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}


