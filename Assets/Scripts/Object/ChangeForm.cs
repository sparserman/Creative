using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChangeForm : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Stat stat;

    public List<Sprite> sprites;    // 바뀔 스프라이트

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        stat = GetComponent<Stat>();
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
            if (stat.hp >= stat.maxhp * val)
            {
                spriteRenderer.sprite = sprites[i - 1];
                break;
            }
        }
    }
}
