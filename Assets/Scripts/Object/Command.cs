using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public enum EnemyType
{
    Soldier1,
    Soldier2,
    Soldier3,
    Gangster1,
    Gangster2,
    Gangster3,
    FireWizard,
    Tower
}

public class Command : MonoBehaviour
{
    public string world;

    // �÷��̾�� �浹 üũ
    bool check;

    GameManager gm;
    public GameObject potalUI;
    Animator anim;

    GameObject canvas;

    // �ı� Ÿ�̸�
    bool destroyTimerOn = false;
    float destroyTimer;

    // �ٸ����̵� ������Ʈ
    public List<GameObject> bList;
    int count = 0;  // ��ġ�� �ٸ����̵��� �α�����ŭ ������ ��ȯ�ϱ��
    float tempSpawnTime;  // ���� �ð� ������

    public GameObject soldierLoading;


    void Start()
    {
        anim = potalUI.GetComponent<Animator>();
        gm = GameManager.GetInstance();

        // ���� ���� ������ ���� �ֱ�
        gm.gi.command = this;
        canvas = GameObject.Find("Canvas");

        BarricadeInfoInput();
    }

    void Update()
    {
        InputSystem();

        SpawnSystem();

        if (potalUI.activeSelf)
        {
            // ī�޶� ũ�� ����
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 3, Time.deltaTime * gm.player.controlSpeed);
        }

        if(destroyTimerOn)
        {
            DefeatWorld();
        }
    }

    // �ٸ����̵� ù ���� �Է� �� �ҷ�����
    void BarricadeInfoInput()
    {
        if (gm.gi.barricadeAList.Count == 0)
        {
            for (int i = 0; i < bList.Count; i++)
            {
                gm.gi.barricadeAList.Add(new BarricadeInfo(!bList[i].transform.GetChild(0).gameObject.activeSelf, bList[i].transform.position));
            }
        }
        else
        {
            
            for (int i = 0; i < gm.gi.barricadeAList.Count; i++)
            {
                for(int j = 0; j < bList.Count; j++)
                {
                    if (gm.gi.barricadeAList[i].position == bList[j].transform.position)
                    {
                        bList[j].SetActive(!gm.gi.barricadeAList[i].build);

                        if (gm.gi.barricadeAList[i].build)
                        {
                            // �ٸ����̵� ����
                            GameObject go = Instantiate(Resources.Load("Prefabs/" + "Barricade") as GameObject);
                            go.transform.position = bList[j].transform.position;
                            go.GetComponent<Enemy>().coverNum = gm.gi.coverNum;
                            go.GetComponent<Enemy>().buildPoint = bList[j].transform.GetChild(0).gameObject;
                            count += gm.gi.population;
                        }
                    }
                }
            }

            tempSpawnTime = gm.gi.spawnTime;
            gm.gi.spawnTime = 0.3f;
        }
    }

    // �ٸ����̵� ���� ����
    void BarricadeInfoSave()
    {
        for (int i = 0; i < gm.gi.barricadeAList.Count; i++)
        {
            for (int j = 0; j < bList.Count; j++)
            {
                if (gm.gi.barricadeAList[i].position == bList[j].transform.position)
                {
                    gm.gi.barricadeAList[i].build = !bList[j].transform.GetChild(0).gameObject.activeSelf;
                }
            }
        }
    }

    // �ο� ���귮 ���
    void SpawnSystem()
    {
        int spawnNum = 0;   // ���� �� �� �ִ� �ο� ��

        // �ٸ����̵�� �ο� ��
        for (int i = 0; i < gm.mobList.Count; i++)
        {
            if (gm.mobList[i].GetComponent<Enemy>())
            {
                Enemy e = gm.mobList[i].GetComponent<Enemy>();

                // �ٸ����̵��� ��
                if (e.GetComponent<Stat>().state == E_State.Fixed)
                {
                    spawnNum += gm.gi.population;
                }
            }
        }
        // �ִ� �α� ����
        gm.gi.maxPopulationA = spawnNum;

        // ���� ����ִ� �ο� �� ���� (�÷��̾� ����)
        for (int i = 0; i < gm.mobList.Count; i++)
        {
            if (gm.mobList[i].GetComponent<Enemy>())
            {
                Enemy e = gm.mobList[i].GetComponent<Enemy>();

                // �Ʊ� ������ ��
                if (e.GetComponent<Stat>().team == Team.Blue)
                {
                    if (e.GetComponent<Stat>().state != E_State.Fixed && e.GetComponent<Stat>().state != E_State.Building)
                    {
                        spawnNum--;
                    }
                }
            }
        }

        // ���� �α� ��
        gm.gi.curPopulationA = gm.gi.maxPopulationA - spawnNum;

        SpawnSoldier(spawnNum);
    }

    float timer = 0;
    void SpawnSoldier(int p_num)
    {
        // ������ �ο� ���� 1�� �̻��̸�
        if(p_num >= 1)
        {
            timer += Time.deltaTime;

            // ���� ��� �ð�
            if(timer >= gm.gi.spawnTime)
            {
                Spawn();

                count--;
                timer = 0;

                // ������ ��ȯ �� �ٽ� ���� �ӵ���
                if(count == 0)
                {
                    gm.gi.spawnTime = tempSpawnTime;
                }
            }

        }
        // ��ȯ �ð� ǥ�ÿ�
        soldierLoading.GetComponent<Image>().fillAmount = timer / gm.gi.spawnTime;
    }

    void Spawn()
    {
        if (gm.mobList.Count > 0)
        {
            gm.mobList[0].GetComponent<Enemy>().GameInfoMobUpdate();
        }

        float px = 0;
        if (gm.gi.rightMobNum > gm.gi.leftMobNum)
        {
            if (gm.gi.leftBarricadeNum >= 1)
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
            if (gm.gi.rightBarricadeNum >= 1)
            {
                px = 0.2f;
            }
            else
            {
                px = -0.2f;
            }
        }
        // ����
        GameObject go = Instantiate(Resources.Load("Prefabs/" + "Soldier1") as GameObject);
        go.transform.position = transform.position + new Vector3(px, -0.6f, 0);
        go.GetComponent<Enemy>().EnemySpawn(EnemyType.Soldier1);
    }


    public void OpenPotalUI()
    {
        potalUI.gameObject.SetActive(true);
        GameManager.GetInstance().player.freeze = true;
    }

    void InputSystem()
    {
        // ��ȣ�ۿ� Ű
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (check && !potalUI.activeSelf)
            {
                OpenPotalUI();
            }
            else if (potalUI.activeSelf)
            {
                // Ŀ�ǵ� �����
                gm.mobList.Clear();
                gm.gi.spawnerList.Clear();
                gm.gi.command = null;
                // ���� ����
                BarricadeInfoSave();

                // �κ�� �̵�
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
