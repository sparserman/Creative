using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CostUI : MonoBehaviour
{
    GameManager gm;

    public TextMeshProUGUI eterniumText;
    public TextMeshProUGUI cpText;

    void Start()
    {
        gm = GameManager.GetInstance();
    }

    void Update()
    {
        TextUpdate();
    }

    void TextUpdate()
    {
        eterniumText.text = gm.gi.eternium.ToString();
        cpText.text = gm.gi.CP.ToString();
    }
}
