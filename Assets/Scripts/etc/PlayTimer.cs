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
        dayText.text = "D-" + gm.gi.day.ToString();


        string hText;
        string mText;
        if(gm.gi.hour < 10)
        {
            hText = "0" + gm.gi.hour.ToString();
        }
        else
        {
            hText = gm.gi.hour.ToString();
        }

        if (gm.gi.minute < 10)
        {
            mText = "0" + gm.gi.minute.ToString();
        }
        else
        {
            mText = gm.gi.minute.ToString();
        }

        timeText.text = hText + ":" + mText;
    }
}
