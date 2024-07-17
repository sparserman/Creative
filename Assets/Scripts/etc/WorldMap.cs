using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    public Animator anim;
    GameManager gm;
    SoundManager sm;

    public GameObject mapIcon;

    void Start()
    {
        gm = GameManager.GetInstance();
        sm = gm.GetComponent<SoundManager>();

        anim = GetComponent<Animator>();

    }

    void Update()
    {

    }


    // ¸Ê ²ô±â (Anim)
    public void WorldMapOff()
    {
        gameObject.SetActive(false);
        mapIcon.SetActive(true);
    }
}
