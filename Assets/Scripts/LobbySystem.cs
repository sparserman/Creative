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
        // Ű���� �Է�
        InputSystem();
    }

    public void InputSystem()
    {
       
    }
}
