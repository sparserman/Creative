using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class Command : MonoBehaviour
{
    public string worldName;

    // 플레이어와 충돌 체크
    bool check;

    GameManager gm;
    public GameObject potalUI;
    Animator anim;

    GameObject canvas;

    // 파괴 타이머
    bool destroyTimerOn = false;
    float destroyTimer;

    // 바리케이드 오브젝트
    public List<GameObject> bList;
    int count = 0;  // 설치된 바리케이드의 인구수만큼 빠르게 소환하기용
    float tempSpawnTime;  // 스폰 시간 보관용

    public GameObject soldierLoading;

    // 지역 정보
    PointInfo pi;

    void Start()
    {
        anim = potalUI.GetComponent<Animator>();
        gm = GameManager.GetInstance();

        // 지역 입력
        for (int i = 0; i < gm.gi.pointList.Count; i++)
        {
            if (gm.gi.pointList[i].worldName == worldName)
            {
                pi = gm.gi.pointList[i];
            }
        }

        // 현재 월드 정보에 본인 넣기
        gm.command = this;
        canvas = GameObject.Find("Canvas");

        BarricadeInfoInput();
    }

    void Update()
    {
        if (!gm.timerOn)
        {
            return;
        }

        InputSystem();

        SpawnSystem();

        if (potalUI.activeSelf)
        {
            // 카메라 크기 조절
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 3, Time.deltaTime * gm.player.controlSpeed);
        }

        if(destroyTimerOn)
        {
            DefeatWorld();
        }
    }

    // 바리케이드 첫 정보 입력 및 불러오기
    void BarricadeInfoInput()
    {
        if(pi == null)
        {
            return;
        }

        if (pi.barricadeList.Count == 0)
        {
            if (bList.Count != 0)
            {
                for (int i = 0; i < bList.Count; i++)
                {
                    pi.barricadeList.Add(new BarricadeInfo(!bList[i].transform.GetChild(0).gameObject.activeSelf, bList[i].transform.position));
                }
            }
        }
        else
        {
            
            for (int i = 0; i < pi.barricadeList.Count; i++)
            {
                for(int j = 0; j < bList.Count; j++)
                {
                    if (pi.barricadeList[i].position == bList[j].transform.position)
                    {
                        bList[j].SetActive(!pi.barricadeList[i].build);

                        if (pi.barricadeList[i].build)
                        {
                            // 바리케이드 생성
                            GameObject go = Instantiate(Resources.Load("Prefabs/" + "Barricade") as GameObject);
                            go.transform.position = bList[j].transform.position;
                            go.GetComponent<Enemy>().coverNum = gm.gi.coverNum;
                            go.GetComponent<Enemy>().buildPoint = bList[j].transform.GetChild(0).gameObject;
                            count += gm.gi.coverNum;
                        }
                    }
                }
            }

            tempSpawnTime = gm.gi.spawnTime;
            gm.gi.spawnTime = 0.3f;
        }
    }

    // 바리케이드 정보 저장
    void BarricadeInfoSave()
    {
        for (int i = 0; i < pi.barricadeList.Count; i++)
        {
            for (int j = 0; j < bList.Count; j++)
            {
                if (pi.barricadeList[i].position == bList[j].transform.position)
                {
                    pi.barricadeList[i].build = !bList[j].transform.GetChild(0).gameObject.activeSelf;
                }
            }
        }
    }

    // 인원 생산량 계산
    void SpawnSystem()
    {
        int spawnNum = 0;   // 생산 될 수 있는 인원 수

        // 바리케이드당 인원
        for (int i = 0; i < gm.mobList.Count; i++)
        {
            if (gm.mobList[i].GetComponent<Enemy>())
            {
                Enemy e = gm.mobList[i].GetComponent<Enemy>();

                // 바리케이드일 때
                if (e.GetComponent<Enemy>().mobInfo.stat.state == E_State.Fixed)
                {
                    // 바리케이드 수용수만큼 생성 가능
                    spawnNum += gm.gi.coverNum;
                }
            }
        }

        int tempPopulation = gm.gi.population;
        // 현재 살아있는 인원 수 빼기 (플레이어와 특수 병사 제외)
        for (int i = 0; i < gm.mobList.Count; i++)
        {
            if (gm.mobList[i].GetComponent<Enemy>())
            {
                Enemy e = gm.mobList[i].GetComponent<Enemy>();

                // 아군 병사일 때
                if (e.GetComponent<Enemy>().mobInfo.nameText == "Soldier1")
                {
                    if (e.GetComponent<Enemy>().mobInfo.stat.state != E_State.Fixed && e.GetComponent<Enemy>().mobInfo.stat.state != E_State.Building)
                    {
                        spawnNum--;
                        tempPopulation--;
                    }
                }
            }
        }

        if (tempPopulation > 0)
        {
            SpawnSoldier(spawnNum);
        }
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
                Spawn();

                count--;
                timer = 0;

                // 빠르게 소환 후 다시 정상 속도로
                if(count == 0)
                {
                    gm.gi.spawnTime = tempSpawnTime;
                }
            }

        }
        // 소환 시간 표시용
        soldierLoading.GetComponent<Image>().fillAmount = timer / gm.gi.spawnTime;
    }

    void Spawn()
    {
        if (gm.mobList.Count > 0)
        {
            gm.mobList[0].GetComponent<Enemy>().GameInfoMobUpdate();
        }

        float px = 0;
        if (gm.rightMobNum > gm.leftMobNum)
        {
            if (gm.leftBarricadeNum >= 1)
            {
                px = -0.2f;
            }
            else
            {
                px = 0.2f;
            }
        }
        else
        {
            if (gm.rightBarricadeNum >= 1)
            {
                px = 0.2f;
            }
            else
            {
                px = -0.2f;
            }
        }
        // 생성
        GameObject go = Instantiate(Resources.Load("Prefabs/Mob/" + "Soldier1") as GameObject);
        go.transform.position = transform.position + new Vector3(px, -0.6f, 0);
        go.GetComponent<Enemy>().EnemySpawn("Soldier1");
        go.GetComponent<Enemy>().EnemySpawn("Soldier1");
        
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
            if (check && !potalUI.activeSelf)
            {
                OpenPotalUI();
            }
            else if (potalUI.activeSelf)
            {
                // 커맨드 지우기
                gm.mobList.Clear();
                gm.spawnerList.Clear();
                gm.command = null;
                // 정보 저장
                BarricadeInfoSave();

                // 로비로 이동
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

    public void DestroyCommand()
    {
        gm.player.freeze = false;
        gm.player.cameraTarget = gameObject;
        gm.player.cMode = false;
        gm.player.respawnTimer.transform.parent.gameObject.SetActive(false);
        gm.player.respawnTimer.gameObject.SetActive(false);
    }

    public void CreateDestroyParticle()
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/" + "DestroyParticle") as GameObject);
        go.transform.position = transform.position + new Vector3(0, -0.8f, 0);
    }

    public void CreateWarningMessage()
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/" + "WarningMessage") as GameObject);
        go.transform.SetParent(canvas.transform, false);

        destroyTimerOn = true;
    }

    void DefeatWorld()
    {
        if (destroyTimer >= 0)
        {
            destroyTimer += Time.deltaTime;
        }

        if(destroyTimer >= 3f)
        {
            gm.lm.StartFadeIn((int)STAGE.LOBBY);
            destroyTimer = -1;
        }
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
