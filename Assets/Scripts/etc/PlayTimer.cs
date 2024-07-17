using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayTimer : MonoBehaviour
{
    GameManager gm;

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;


    void Start()
    {
        gm = GameManager.GetInstance();
    }

    void Update()
    {
        UpdateDateText();
    }

    void UpdateDateText()
    {
        dayText.text = "D-" + GameManager.day.ToString();


        string hText;
        string mText;
        if(GameManager.hour < 10)
        {
            hText = "0" + GameManager.hour.ToString();
        }
        else
        {
            hText = GameManager.hour.ToString();
        }

        if (GameManager.minute < 10)
        {
            mText = "0" + GameManager.minute.ToString();
        }
        else
        {
            mText = GameManager.minute.ToString();
        }

        timeText.text = hText + ":" + mText;
    }
}
