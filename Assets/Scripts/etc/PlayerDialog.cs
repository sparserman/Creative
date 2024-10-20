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
                dialogueSystem.nextChapter = false;
                break;
            case "TutorialSkip":
                dialogueSystem.nextChapter = true;
                break;
            default:
                dialogueSystem.nextChapter = false;
                break;
        }

        dialogueSystem.nextCheck = true;
    }
}
