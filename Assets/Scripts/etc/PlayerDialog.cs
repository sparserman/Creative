using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDialog : MonoBehaviour
{
    public string type;
    public TextMeshProUGUI dialogText;

    public DialogueSystem dialogueSystem;

    public void Click()
    {
        switch (type)
        {
            case "TutorialStart":
                dialogueSystem.PlayerSelected(true);
                dialogueSystem.SetDialogControllerIndex(1001);
                break;
            case "TutorialSkip":
                dialogueSystem.PlayerSelected(false);
                dialogueSystem.SetDialogControllerIndex(1002);
                break;
            default:
                dialogueSystem.PlayerSelected(true);
                break;
        }

        dialogueSystem.nextCheck = true;
    }
}
