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

        sm.PlayBGMSound(sm.lobbyBGM);
    }

    void Update()
    {
        // 키보드 입력
        InputSystem();
    }

    public void InputSystem()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            WorldMapOn();
        }
    }

    void WorldMapOn()
    {
        // 맵이 꺼져있을 때만
        if (!worldMap.gameObject.activeSelf)
        {
            // 맵 키기
            worldMap.SetActive(true);
            sm.PlayEffectSound(sm.click);
        }
    }
}
