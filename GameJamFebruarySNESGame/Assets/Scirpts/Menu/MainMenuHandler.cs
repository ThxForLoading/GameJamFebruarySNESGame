using UnityEngine;

public class MainMenuHandler : MonoBehaviour
{
    [Header("PressAnyKey")]
    [SerializeField] GameObject pressAnyKey;
    private bool pressAnyKeyHighlighted;
    [SerializeField] float flashSpeed;
    float timer;

    [Header("MainMenu")]
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject exitButton;

    [Header("SaveSlots")]
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject slot1;
    [SerializeField] GameObject slot2;
    [SerializeField] GameObject slot3;

    [Header("LogicHelpers")]
    public MenuState menuState;

    public enum MenuState
    {
        PressAnyKey,
        MainMenu,
        SaveSlots
    }

    private void Start()
    {
        menuState = MenuState.PressAnyKey;
    }

    private void Update()
    {
        toggleMenuElements();

        timer = timer + Time.deltaTime;
        if(timer > flashSpeed)
        {
            flashPressAnyKey();
            timer = 0;
        }
    }


    public void flashPressAnyKey()
    {
        if (pressAnyKeyHighlighted)
        {
            //Un-Highlight the Anykey thing here
            pressAnyKeyHighlighted = false;
        }
        else
        {
            //Highlight anykey here
            pressAnyKeyHighlighted = true;
        }
    }

    public void toggleMenuElements()
    {
        switch (menuState)
        {
            case MenuState.PressAnyKey:
                if (!pressAnyKey.activeSelf) pressAnyKey.SetActive(true);
                //
                if (playButton.activeSelf) playButton.SetActive(false);
                if (continueButton.activeSelf) continueButton.SetActive(false);
                if (exitButton.activeSelf) exitButton.SetActive(false);
                if (backButton.activeSelf) backButton.SetActive(false);
                if (slot1.activeSelf) slot1.SetActive(false);
                if (slot2.activeSelf) slot2.SetActive(false);
                if (slot3.activeSelf) slot3.SetActive(false);
                break;
            case MenuState.MainMenu:
                if (!playButton.activeSelf) playButton.SetActive(true);
                if (!continueButton.activeSelf) continueButton.SetActive(true);
                if (!exitButton.activeSelf) exitButton.SetActive(true);
                //
                if (pressAnyKey.activeSelf) pressAnyKey.SetActive(false);
                if (backButton.activeSelf) backButton.SetActive(false);
                if (slot1.activeSelf) slot1.SetActive(false);
                if (slot2.activeSelf) slot2.SetActive(false);
                if (slot3.activeSelf) slot3.SetActive(false);
                break;
            case MenuState.SaveSlots:
                if (!backButton.activeSelf) backButton.SetActive(true);
                if (!slot1.activeSelf) slot1.SetActive(true);
                if (!slot2.activeSelf) slot2.SetActive(true);
                if (!slot3.activeSelf) slot3.SetActive(true);
                //
                if (pressAnyKey.activeSelf) pressAnyKey.SetActive(false);
                if (playButton.activeSelf) playButton.SetActive(false);
                if (continueButton.activeSelf) continueButton.SetActive(false);
                if (exitButton.activeSelf) exitButton.SetActive(false);
                break;
            default:
                Debug.Log("No state, this shouldn't happen");
                break;
        }
    }
}

