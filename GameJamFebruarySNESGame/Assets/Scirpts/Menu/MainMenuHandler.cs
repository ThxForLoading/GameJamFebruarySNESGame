using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class MainMenuHandler : MonoBehaviour
{
    [Header("PressAnyKey")]
    [SerializeField] GameObject pressAnyKey;
    private bool pressAnyKeyHighlighted;
    [SerializeField] float flashSpeed;
    float timer;
    [SerializeField] float timeUnitlPlayerCanContinue = 5;

    [Header("SaveSlots")]
    [SerializeField] GameObject slot1;
    [SerializeField] GameObject slot2;
    [SerializeField] GameObject slot3;

    [Header("LogicHelpers")]
    public MenuState menuState;
    [SerializeField] private InputActionReference moveSelector;
    [SerializeField] private InputActionReference confirmKey;
    private int selector = 0;
    private bool anyButtonPressed = false;
    private float helperTimer = 0;
    private bool canMove = true;
    [SerializeField] string defaultScene;

    bool playIntro = false;
    [SerializeField] Animator animator;

    public enum MenuState
    {
        PressAnyKey,
        SaveSlots,
        Waiting
    }

    private void Start()
    {
        menuState = MenuState.Waiting;
        animator.SetBool("PlayIntro",playIntro);
        StartCoroutine(PlayIntro());
    }
    private void OnEnable()
    {
        InputSystem.onAnyButtonPress.Call(OnAnyButtonPressed);
        confirmKey.action.performed += ConfirmKey;
    }

    private void OnDisable()
    {
        confirmKey.action.performed -= ConfirmKey;
    }

    void ConfirmKey(InputAction.CallbackContext context)
    {
        if(menuState == MenuState.SaveSlots)
        {
            menuState = MenuState.Waiting;

            StartCoroutine(PlayIntroCutscene());
        }
    }

    IEnumerator PlayIntroCutscene()
    {
        playIntro = true;
        animator.SetBool("PlayIntro", playIntro);
        yield return new WaitForSeconds(12);
        StartCoroutine(LaunchGame());
    }

    IEnumerator LaunchGame()
    {
        if(string.IsNullOrEmpty(defaultScene))
        {
            Debug.Log("Failed to init game, defaultscene missing");
            yield break;
        }

        SaveHandler.instance.BeginLoadFromSlot(selector);

        string targetScene;
        if (SaveHandler.instance.GetSaveDataForSlot(selector) != null)
        {
            SaveData data = SaveHandler.instance.GetSaveDataForSlot(selector);
            targetScene = data.sceneName;
        }
        else
        {
            targetScene = defaultScene;
        }
            
        yield return new WaitForSeconds(2);
        SceneHandler.instance.LoadScene(targetScene);
    }

    void OnAnyButtonPressed(InputControl control)
    {
        if (menuState == MenuState.PressAnyKey)
        {
            anyButtonPressed = true;
        }
    }

    private void Update()
    {
        if (menuState == MenuState.Waiting) return;

        Vector2 input = moveSelector.action.ReadValue<Vector2>();

        if (canMove && input.x > 0.5f)
        {
            selector++;
            canMove = false;
        }
        else if( canMove && input.x < -0.5f)
        {
            selector--;
            canMove= false;
        }

        selector = Mathf.Clamp(selector, 0, 2);

        if(Mathf.Abs(input.x) < 0.1f)
        {
            canMove = true;
        }

        toggleMenuElements();

        if(menuState == MenuState.PressAnyKey)
        {
            helperTimer += Time.deltaTime;
            if(helperTimer > timeUnitlPlayerCanContinue)
            {
                if (anyButtonPressed)
                {
                    menuState = MenuState.Waiting;
                    StartCoroutine(GoToSaveslot());
                }
            }
        }

        if(menuState == MenuState.SaveSlots)
        {
            switch (selector)
            {
                case 0:
                    slot1.GetComponent<SaveSlotHighlighter>().HighlightThisSlot();
                    slot2.GetComponent<SaveSlotHighlighter>().DehighlightThisSlot();
                    slot3.GetComponent<SaveSlotHighlighter>().DehighlightThisSlot();
                    break;
                case 1:
                    slot2.GetComponent<SaveSlotHighlighter>().HighlightThisSlot();
                    slot3.GetComponent<SaveSlotHighlighter>().DehighlightThisSlot();
                    slot1.GetComponent<SaveSlotHighlighter>().DehighlightThisSlot();
                    break;
                case 2:
                    slot3.GetComponent<SaveSlotHighlighter>().HighlightThisSlot();
                    slot2.GetComponent<SaveSlotHighlighter>().DehighlightThisSlot();
                    slot1.GetComponent<SaveSlotHighlighter>().DehighlightThisSlot();
                    break;
                default:
                    Debug.Log("Selected a saveslot that doesnt exist");
                    break;
            }
        }

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
                if (slot1.activeSelf) slot1.SetActive(false);
                if (slot2.activeSelf) slot2.SetActive(false);
                if (slot3.activeSelf) slot3.SetActive(false);
                break;
            case MenuState.SaveSlots:
                if (!slot1.activeSelf) slot1.SetActive(true);
                if (!slot2.activeSelf) slot2.SetActive(true);
                if (!slot3.activeSelf) slot3.SetActive(true);
                //
                if (pressAnyKey.activeSelf) pressAnyKey.SetActive(false);
                break;
            case MenuState.Waiting:
                    //Debug.Log("Currently Waiting");
                break;
            default:
                Debug.Log("No state, this shouldn't happen");
                break;
        }
    }

    IEnumerator PlayIntro()
    {
        yield return new WaitForSeconds(5);
        menuState = MenuState.PressAnyKey;
    }

    IEnumerator GoToSaveslot()
    {
        yield return new WaitForSeconds(0.5f);
        menuState = MenuState.SaveSlots;
    }
}

