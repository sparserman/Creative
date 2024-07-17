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
        dayText.text = "D-" + gm.day.ToString();


        string hText;
        string mText;
        if(gm.hour < 10)
        {
            hText = "0" + gm.hour.ToString();
        }
        else
        {
            hText = gm.hour.ToString();
        }

        if (gm.minute < 10)
        {
            mText = "0" + gm.minute.ToString();
        }
        else
        {
            mText = gm.minute.ToString();
        }

        timeText.text = hText + ":" + mText;
    }
}
