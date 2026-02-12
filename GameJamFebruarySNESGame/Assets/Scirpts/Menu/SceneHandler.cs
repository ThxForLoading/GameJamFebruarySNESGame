using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler instance;
    Scene currentScene;
    [SerializeField] CanvasGroup fadeCanvas;
    [SerializeField] float fadeDuration = 0.5f;

    SceneManager sceneManager;

    private void Awake()
    {
        if (instance == null)           //GRRR singleton
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        fadeCanvas.alpha = 0f;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithFade(sceneName));
    }

    IEnumerator LoadSceneWithFade(string sceneName)
    {
        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerCore>().lockMovement = true;
        }
        yield return StartCoroutine(Fade(1f));
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(Fade(0f));
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerCore>().lockMovement = false;
        }
    }

    public void LoadSceneAtSpawn(string sceneName, string spawnPoint)
    {
        StartCoroutine(LoadingNextLevel(sceneName, spawnPoint));

    }

    IEnumerator LoadingNextLevel(string levelToLoad, string spawnPoint)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerCore>().lockMovement = true;
        yield return StartCoroutine(Fade(1f));
        yield return new WaitForSeconds(0.5f);
        AsyncOperation asynLoad = SceneManager.LoadSceneAsync(levelToLoad);
        while (!asynLoad.isDone)
        {
            yield return null;
        }
        GameObject[] go = GameObject.FindGameObjectsWithTag("SpawnPoint");
        foreach (GameObject g in go)
        {
            if (g.name == spawnPoint)
            {
                GameObject.FindGameObjectWithTag("Player").transform.position = g.transform.position;
            }
        }
        yield return StartCoroutine(Fade(0f));
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerCore>().lockMovement = false;
    }

    IEnumerator Fade(float targetAlpha)
    {
        Debug.Log("Fading to " + targetAlpha);
        float startAlpha = fadeCanvas.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha;
    }

    public void ReloadScene(string sceneName)
    {
        currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
