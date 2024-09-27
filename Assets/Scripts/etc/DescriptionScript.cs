using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DescriptionClass
{
    public string description;
    public string variable;
}

public class DescriptionScript : MonoBehaviour
{
    [SerializeField]
    public List<DescriptionClass> dList;

    public Vector2 boxSize;
    public Color32 color;
    public Sprite sprite;

    GameObject go;

    private void OnMouseDown()
    {
        go = Instantiate(Resources.Load("Prefabs/" + "DescriptionWindow") as GameObject);
        go.GetComponent<Image>().rectTransform.sizeDelta = boxSize;
        go.GetComponent<Image>().sprite = sprite;
        go.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        for(int i = 0; i < dList.Count; i++)
        {
            GameObject g = Instantiate(Resources.Load("Prefab/" + "DescriptionText") as GameObject);
            if (dList[i].variable != null)
            {
                g.GetComponent<TextMeshProUGUI>().text = dList[i].description + dList[i].variable;
            }
            else
            {
                g.GetComponent<TextMeshProUGUI>().text = dList[i].description;
            }

            g.GetComponent<TextMeshProUGUI>().rectTransform.sizeDelta = boxSize * 0.7f;
        }
    }

    private void OnMouseUp() 
    { 
        if(go != null)
        {
            Destroy(go);
        }
    }
}
