using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] string targetScene;
    [SerializeField] string sceneTargetSpawn;

    GameObject sceneHandler;

    private void Awake()
    {
        sceneHandler = GameObject.FindGameObjectWithTag("SceneHandler");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(sceneHandler != null)
            {
                sceneHandler.GetComponent<SceneHandler>().LoadSceneAtSpawn(targetScene, sceneTargetSpawn);
            }
            else
            {
                Debug.Log("Scene handler missing");
            }
        }
    }
}
