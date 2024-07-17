using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapIcon : MonoBehaviour
{
    Animator anim;
    GameManager gm;
    SoundManager sm;

    public Collider2D col;

    public LayerMask mask;

    public GameObject worldMap;

    void Start()
    {
        gm = GameManager.GetInstance();
        sm = gm.GetComponent<SoundManager>();
        
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray2D ray = new Ray2D(pos, Vector2.zero);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1, mask);

        // null이 아니면서 본인 collider라면
        if (hit.collider != null)
        {
            Debug.Log(Input.mousePosition);
            if (hit.collider == col)
            {
                anim.SetBool("EnterMouse", true);
                Debug.Log(hit.collider.name);
                Debug.Log(Input.mousePosition);
            }
            else if (hit.collider != col)
            {
                anim.SetBool("EnterMouse", false);
                Debug.Log(hit.collider.name);
            }
        }
    }

    public void MapIconClick()
    {
        anim.SetTrigger("Off");
    }

    public void MapIconOff()
    {
        gameObject.SetActive(false);

        // 맵이 꺼져있을 때만
        if (!worldMap.gameObject.activeSelf)
        {
            // 맵 키기
            worldMap.SetActive(true);
            // 리스트 추가
            gm.goList.Add(worldMap);

            if (sm != null)
            {
                sm.PlayEffectSound(sm.click);
            }
        }
    }
}
