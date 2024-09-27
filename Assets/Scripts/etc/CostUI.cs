using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CostUI : MonoBehaviour
{
    GameManager gm;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI magicText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI cpText;
    public TextMeshProUGUI mpText;

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
        goldText.text = gm.gi.gold.ToString();
        magicText.text = gm.gi.magic.ToString();
        foodText.text = gm.gi.food.ToString();

        cpText.text = gm.gi.CP.ToString();
        mpText.text = gm.gi.MP.ToString();
    }
}
