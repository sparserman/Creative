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
            // ī�޶� ũ�� ����
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 3, Time.deltaTime * gm.player.controlSpeed);
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
                    spawnNum += gm.gi.coverNum;
                }
            }
        }

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
                // ����
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
        // ��ȣ�ۿ� Ű
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
