using UnityEngine;

public class Credits : MonoBehaviour
{
    float timer = 0;
    [SerializeField] GameObject[] scene1;
    [SerializeField] GameObject[] scene2;
    [SerializeField] GameObject[] scene3;
    [SerializeField] GameObject[] scene4;
    [SerializeField] GameObject[] scene5;


    private void Start()
    {
        
    }


    void Update()
    {
        timer += Time.deltaTime;
        if(timer < 10)
        {
            enableAll(scene1);
        }
        if(timer > 10 && timer < 20)
        {
            enableAll(scene2);
            disableAll(scene1);
        }
        if(timer > 20 && timer < 30)
        {
            enableAll(scene3);
            disableAll(scene2);
        }
        if( timer > 30 && timer < 40)
        {
            enableAll(scene4);
            disableAll(scene3);
        }
        if(timer > 40)
        {
            enableAll(scene5);
            disableAll(scene4);
        }
    }

    public void enableAll(GameObject[] gos)
    {
        foreach (GameObject go in gos)
        {
            go.SetActive(true);
        }
    }

    public void disableAll(GameObject[] gos)
    {
        foreach(GameObject go in gos)
        {
            go.SetActive(false);
        }
    }
}
