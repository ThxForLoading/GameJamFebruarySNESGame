using Unity.Cinemachine;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] PolygonCollider2D mapBounds;
    CinemachineConfiner2D confiner;
    [SerializeField] GameObject cmCamera;
    [SerializeField] GameObject refTargetPosition;
    [SerializeField] bool fadeToBlack;

    private void Awake()
    {
        confiner = cmCamera.GetComponent<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            confiner.BoundingShape2D = mapBounds;

            if (fadeToBlack)
            {
                Debug.Log("Do transition to black, then place the player using coroutine");
            }
            else
            {
                collision.gameObject.transform.position = refTargetPosition.transform.position;
            }
        }
    }
}
