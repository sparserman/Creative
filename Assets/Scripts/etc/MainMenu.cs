using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    GameManager gm;
    SoundManager sm;

    void Start()
    {
        gm = GameManager.GetInstance();
        sm = gm.GetComponent<SoundManager>();
        sm.PlayBGMSound(sm.mainBGM);
    }

    void Update()
    {
        
    }
}
