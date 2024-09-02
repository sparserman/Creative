using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public enum EnemyType
{
    Soldier1,
    Soldier2,
    Soldier3,
    Gangster1,
    Gangster2,
    Gangster3
}

public class Command : MonoBehaviour
{
    bool check;

    GameManager gm;
    public GameObject potalUI;
    Animator anim;

    void Start()
    {
        anim = potalUI.GetComponent<Animator>();
        gm = GameManager.GetInstance();
    }

    void Update()
    {
        InputSystem();

        SpawnSystem();

        if (potalUI.activeSelf)
        {
            // 카메라 크기 조절
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 3, Time.deltaTime * gm.player.controlSpeed);
        }
    }

    // 인원 생산량 계산
    void SpawnSystem()
    {
        int spawnNum = 0;   // 생산 될 수 있는 인원 수

        // 바리케이드당 인원 셋
        for (int i = 0; i < gm.mobList.Count; i++)
        {
            if (gm.mobList[i].GetComponent<Enemy>())
            {
                Enemy e = gm.mobList[i].GetComponent<Enemy>();

                // 바리케이드일 때
                if (e.GetComponent<Stat>().state == E_State.Fixed)
                {
                    spawnNum += gm.gi.coverNum;
                }
            }
        }

        // 현재 살아있는 인원 수 빼기 (플레이어 제외)
        for (int i = 0; i < gm.mobList.Count; i++)
        {
            if (gm.mobList[i].GetComponent<Enemy>())
            {
                Enemy e = gm.mobList[i].GetComponent<Enemy>();

                // 아군 병사일 때
                if (e.GetComponent<Stat>().team == Team.Blue)
                {
                    if (e.GetComponent<Stat>().state != E_State.Fixed && e.GetComponent<Stat>().state != E_State.Building)
                    {
                        spawnNum--;
                    }
                }
            }
        }

        SpawnSoldier(spawnNum);
    }

    float timer = 0;
    void SpawnSoldier(int p_num)
    {
        // 생성할 인원 수가 1명 이상이면
        if(p_num >= 1)
        {
            timer += Time.deltaTime;

            // 스폰 대기 시간
            if(timer >= gm.gi.spawnTime)
            {
                // 생성
                GameObject go = Instantiate(Resources.Load("Prefabs/" + "Soldier1") as GameObject);
                go.transform.position = transform.position;
                go.GetComponent<Enemy>().EnemySpawn(EnemyType.Soldier1);

                timer = 0;
            }
        }
    }


    public void OpenPotalUI()
    {
        potalUI.gameObject.SetActive(true);
        GameManager.GetInstance().player.freeze = true;
    }

    void InputSystem()
    {
        // 상호작용 키
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(check && !potalUI.activeSelf)
            {
                OpenPotalUI();
            }
            else if (potalUI.activeSelf)
            {
                gm.mobList.Clear();
                GameManager.GetInstance().lm.StartFadeIn((int)STAGE.LOBBY);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.X))
        {
            if (potalUI.activeSelf)
            {
                anim.SetTrigger("Off");
            }
        }
    }

    public void OffPotalUI()
    {
        gameObject.SetActive(false);
        GameManager.GetInstance().player.freeze = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            check = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            check = false;
        }
    }
}
