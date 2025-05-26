using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class DialougeScript : MonoBehaviour
{
    public GameObject player;
    public TextMeshProUGUI dialougeTextField;
    public float textTypingSpeed = 0.03f;
    public List<string> dialougeText;
    int dialougeTextIndex = -1;

    void Start()
    {
        LockPlayerControls();
        GoToNextDialougeText();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) GoToNextDialougeText(); // Current input method, can be realtered
    }

    void GoToNextDialougeText()
    {
        if (dialougeTextIndex < dialougeText.Count - 1)
        {
            dialougeTextIndex++;
            SetDialougeText();
        }
        else
        {
            UnlockPlayerControls();
            this.gameObject.SetActive(false);
        }
    }

    void ClearTextField()
    {
        dialougeTextField.text = "";
    }

    void SetDialougeText()
    {
        ClearTextField();
        StopAllCoroutines();
        StartCoroutine(TypeDialougeText());
    }

    IEnumerator TypeDialougeText()
    {
        foreach (char word in dialougeText[dialougeTextIndex])
        {
            dialougeTextField.text += word;
            yield return new WaitForSeconds(textTypingSpeed);
        }
    }

    void LockPlayerControls()
    {
        //Make player unable to move
        player.GetComponentInChildren<CursorController>().rightClickEnabled = false;
        player.GetComponentInChildren<CursorController>().leftClickEnabled = false;
    }

    void UnlockPlayerControls()
    {
        //Make player able to move again
        player.GetComponentInChildren<CursorController>().rightClickEnabled = true;
        player.GetComponentInChildren<CursorController>().leftClickEnabled = true;
    }


}
