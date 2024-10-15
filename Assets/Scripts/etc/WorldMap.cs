using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    Animator anim;
    GameManager gm;

    public GameObject mapIcon;

    void Start()
    {
        gm = GameManager.GetInstance();

        anim = GetComponent<Animator>();

    }

    public void WorldMapOffAnim()
    {
        anim.SetTrigger("Off");

        // ÀÚ±â À§·Î ´Ù ²ô±â
        for (int i = 0; i < gm.goList.Count; i++)
        {
            gm.MenuOff(false);
        }

        gm.sm.PlayEffectSound(gm.sm.click);
    }

    // ¸Ê ²ô±â (Anim)
    public void WorldMapOff()
    {
        gameObject.SetActive(false);
        mapIcon.SetActive(true);
    }
}
