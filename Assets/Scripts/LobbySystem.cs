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
        // Ű���� �Է�
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
        // ���� �������� ����
        if (!worldMap.gameObject.activeSelf)
        {
            // �� Ű��
            worldMap.SetActive(true);
            sm.PlayEffectSound(sm.click);
        }
    }
}
