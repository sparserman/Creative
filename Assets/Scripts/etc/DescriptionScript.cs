using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BoxType
{
    None = 0, Gold, Magic, Food
}

public class DescriptionScript : MonoBehaviour
{
    public BoxType type;

    public Vector2 boxSize;
    public Color32 color;
    public Sprite sprite;
    public string description;

    GameObject go;
    GameManager gm;

    private void Start()
    {
        gm = GameManager.GetInstance();
    }


    private void OnMouseDown()
    {
        
        go = Instantiate(Resources.Load("Prefabs/" + "DescriptionWindow") as GameObject);
        go.transform.SetParent(GameObject.Find("Canvas").transform, false);
        go.GetComponent<Image>().rectTransform.sizeDelta = boxSize;
        go.GetComponent<Image>().sprite = sprite;
        go.GetComponent<Image>().color = color;

        // 내용
        switch (type)
        {
            case BoxType.Gold:
                description = $"시간 당 골드 수집량 : {gm.gi.goldSupply}";
                break;
            case BoxType.Magic:
                description = $"시간 당 마나 수집량 : {gm.gi.magicSupply}";
                break;
            case BoxType.Food:
                description = $"시간 당 식량 수집량 : {gm.gi.foodSupply}";
                break;
        }

        go.GetComponent<DescriptionWindow>().description.text = description;
        go.GetComponent<DescriptionWindow>().description.rectTransform.sizeDelta = boxSize * 0.75f;

        Vector3 temppos = go.transform.position;
        temppos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        temppos.z = 0;
        go.transform.position = temppos;
        
    }

    private void OnMouseUp() 
    { 
        if(go != null)
        {
            Destroy(go);
        }
    }
}
