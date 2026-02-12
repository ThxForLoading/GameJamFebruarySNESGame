using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class SaveInteractPlayer : MonoBehaviour
{
    PlayerControllerCore controller;
    [SerializeField] GameObject textBox;
    [SerializeField] GameObject textSavePrompt;
    [SerializeField] GameObject textSaveConfirmation;
    [SerializeField] GameObject answerHolder;
    [SerializeField] GameObject yesHighlight;
    [SerializeField] GameObject noHighlight;

    [SerializeField] InputActionReference moveSelector;
    [SerializeField] InputActionReference confirmKey;
    private bool canMove = true;

    bool yesOrNo = true;

    bool confirmAnswer = false;

    bool waitingForInput = false;

    private void OnEnable()
    {
        confirmKey.action.performed += ConfirmKey;
    }

    private void OnDisable()
    {
        confirmKey.action.performed -= ConfirmKey;
    }

    void ConfirmKey(InputAction.CallbackContext context)
    {
        if (!waitingForInput) return;
        confirmAnswer = true;
    }

    void BeginSaveInteraction()
    {
        controller.lockMovement = true;
        if(!textBox.activeSelf) textBox.SetActive(true);
        if(!textSavePrompt.activeSelf) textSavePrompt.SetActive(true);
        if(!answerHolder.activeSelf) answerHolder.SetActive(true);

        StartCoroutine(SelectAnswer());
    }

    IEnumerator SelectAnswer()
    {
        while (!confirmAnswer)
        {
            Vector2 input = moveSelector.action.ReadValue<Vector2>();

            if (canMove && input.x > 0.5f)
            {
                yesOrNo = false;
                canMove = false;
            }
            else if (canMove && input.x < -0.5f)
            {
                yesOrNo = true;
                canMove = false;
            }

            if (Mathf.Abs(input.x) < 0.1f)
            {
                canMove = true;
            }

            yesHighlight.SetActive(yesOrNo);
            noHighlight.SetActive(!yesOrNo);

            yield return null;
        }

        if(yesOrNo)
        {
            if (textSavePrompt.activeSelf) textSavePrompt.SetActive(false);
            if (answerHolder.activeSelf) answerHolder.SetActive(false);
            yield return new WaitForSeconds(1);
            if (!textSaveConfirmation.activeSelf) textSaveConfirmation.SetActive(true);
        }

        EndSaveInteraction();
    }

    void EndSaveInteraction()
    {
        if (textSavePrompt.activeSelf) textSavePrompt.SetActive(false);
        if (answerHolder.activeSelf) answerHolder.SetActive(false);
        if (textSaveConfirmation.activeSelf) textSaveConfirmation.SetActive(false);
        if (textBox.activeSelf) textBox.SetActive(false);

        controller.lockMovement = false;
    }
}
