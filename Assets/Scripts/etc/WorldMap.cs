using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    public Animator anim;
    GameManager gm;
    SoundManager sm;

    void Start()
    {
        gm = GameManager.GetInstance();
        sm = gm.GetComponent<SoundManager>();
        gm.goList.Add(gameObject);

        anim = GetComponent<Animator>();

    }

    void Update()
    {

    }


    // ¸Ê ²ô±â (Anim)
    public void WorldMapOff()
    {
        gameObject.SetActive(false);
    }
}
