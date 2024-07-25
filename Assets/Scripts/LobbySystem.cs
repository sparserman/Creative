using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySystem : MonoBehaviour
{
    public GameObject worldMap;
    SoundManager sm;
    GameManager gm;

    void Start()
    {
        gm = GameManager.GetInstance();
        sm = gm.GetComponent<SoundManager>();

        if (sm != null)
        {
            sm.PlayBGMSound(sm.lobbyBGM);
        }

        gm.timerOn = true;
    }

    void Update()
    {
        // 키보드 입력
        InputSystem();
    }

    public void InputSystem()
    {
       
    }
}
