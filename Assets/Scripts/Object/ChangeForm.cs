using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Form
{
    public Sprite sprite;
    public Color32 color = new Color32(255, 255, 255, 255);
}

public class ChangeForm : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Image image;
    public Stat stat;

    [SerializeField]
    public List<Form> sprites;    // 바뀔 스프라이트

    void Start()
    {
        if (GetComponent<SpriteRenderer>() != null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if(GetComponent<Image>() != null)
        {
            image = GetComponent<Image>();
        }

        if (GetComponent<Stat>() != null)
        {
            stat = GetComponent<Stat>();
        }
    }

    void Update()
    {
        if (sprites != null)
        {
            ChangeAccordingToHp();
        }
    }

    void ChangeAccordingToHp()
    {
        float val = 1;

        for (int i = 1; i <= sprites.Count; i++)
        {
            val -= (1.0f / sprites.Count);
            if (stat.hp >= stat.maxHp * val)
            {
                if (sprites[i - 1].sprite != null)
                {
                    spriteRenderer.sprite = sprites[i - 1].sprite;
                }

                if (spriteRenderer != null)
                {
                    spriteRenderer.color = sprites[i - 1].color;
                }
                else if (image != null)
                {
                    image.color = sprites[i - 1].color;
                }

                break;
            }
        }
    }
}
