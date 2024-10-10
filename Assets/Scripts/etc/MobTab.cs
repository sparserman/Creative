using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;


public class MobTab : MonoBehaviour
{
    GameManager gm;
    Animator anim;

    public MobInfo mobInfo;

    public LayerMask mask;
    public LayerMask panelMask;

    public string nameText;
    public TextMeshProUGUI mobName;
    public TextMeshProUGUI level;
    public Image image;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI ad;
    public TextMeshProUGUI timer;

    // �Ҹ� �ڿ�
    public TextMeshProUGUI gold;
    public Image goldImage;
    public TextMeshProUGUI magic;
    public Image magicImage;
    public TextMeshProUGUI food;
    public Image foodImage;

    GameObject go;

    public GameObject panel;
    public GameObject fieldPanel;

    // ��ġ
    public int num;

    void Start()
    {
        gm = GameManager.GetInstance();
        anim = GetComponent<Animator>();

        Init();
        ResourceImageDrawCheck();
        
    }

    void Update()
    {
        ClickCheck();
        FieldColorChange();
        MoveTab();

        TimerUpdate();
    }

    void ClickCheck()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray2D ray = new Ray2D(pos, Vector2.zero);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1, mask);
        RaycastHit2D panelHit = Physics2D.Raycast(ray.origin, ray.direction, 1, panelMask);


        // MobTab�� ���콺�� ��Ҵµ�
        if (hit)
        {
            // ���� ������Ʈ��
            if (hit.collider.gameObject == gameObject)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    anim.SetTrigger("isClick");
                    go = Instantiate(Resources.Load("Prefabs/Mob/" + nameText) as GameObject);
                    go.GetComponent<Enemy>().spawnWaiting = true;
                    go.GetComponent<Enemy>().stat = mobInfo.stat;

                    // �ʵ� �г� ���̱�
                    fieldPanel.SetActive(true);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    // ��ȯ ���
                    SpawnCancel();
                }
            }
        }
        // �гο� ��Ҵٸ�
        else if(panelHit && go != null)
        {
            // �гξȿ� ���콺�� �ִٸ�
            if (Input.GetMouseButtonUp(0))
            {
                // Ŀ�ǵ� �г��̸�
                if (panelHit.collider.gameObject == panel)
                {
                    // ��ȯ ���
                    SpawnCancel();
                }
                // �ʵ� �г��̸�
                else if(panelHit.collider.gameObject == fieldPanel)
                {
                    if (ResourceCheck() && TimerCheck())
                    {
                        // �ڿ� ���
                        gm.gi.gold -= mobInfo.gold;
                        gm.gi.magic -= mobInfo.magic;
                        gm.gi.food -= mobInfo.food;

                        // ��ȯ
                        go.GetComponent<Enemy>().spawnWaiting = false;
                        gm.mobList.Add(go);

                        // �� ��ġ ����
                        for (int i = 0; i < panel.GetComponent<CommandPanel>().tabList.Count; i++)
                        {
                            panel.GetComponent<CommandPanel>().tabList[i].GetComponent<MobTab>().TabPositionChange(num);
                        }

                        // ���� ���� �� ����Ʈ���� �����
                        panel.GetComponent<CommandPanel>().tabList.Remove(gameObject);
                        // ��� ť�� �ֱ�
                        mobInfo.curWaitingTime = mobInfo.waitingTime;   // ��� �ð� ���� ��
                        panel.GetComponent<CommandPanel>().waitingTab.Enqueue(mobInfo);
                        Destroy(gameObject);
                    }
                    else
                    {
                        // ��ȯ ���
                        SpawnCancel();
                    }

                    // �ʵ� �г� �����
                    fieldPanel.SetActive(false);
                }
            }
        }
        // �̻��� ���̶��
        else
        {
            if (Input.GetMouseButtonUp(0) && go != null)
            {
                // ��ȯ ���
                SpawnCancel();
            }
        }
    }

    void Init()
    {
        // ���� �Է�
        nameText = mobInfo.nameText;
        mobName.text = mobInfo.mobName;
        level.text = "Lv." + mobInfo.level.ToString();
        image.sprite = mobInfo.sprite;

        hp.text = mobInfo.stat.maxHp.ToString();
        ad.text = mobInfo.stat.ad.ToString();

        // �ڿ� �Ҹ� �Է�
        gold.text = mobInfo.gold.ToString();
        magic.text = mobInfo.magic.ToString();
        food.text = mobInfo.food.ToString();
    }

    void ResourceImageDrawCheck()
    {
        // �Ҹ������ʴ� �ڿ� �̹����� �ؽ�Ʈ�� �����
        if(mobInfo.gold == 0)
        {
            goldImage.gameObject.SetActive(false);
            gold.gameObject.SetActive(false);
        }
        if (mobInfo.magic == 0)
        {
            magicImage.gameObject.SetActive(false);
            magic.gameObject.SetActive(false);
        }
        if (mobInfo.food == 0)
        {
            foodImage.gameObject.SetActive(false);
            food.gameObject.SetActive(false);
        }
    }

    bool ResourceCheck()
    {
        if(mobInfo.gold <= gm.gi.gold)
        {
            if (mobInfo.magic <= gm.gi.magic)
            {
                if (mobInfo.food <= gm.gi.food)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void FieldColorChange()
    {
        if(ResourceCheck() && TimerCheck())
        {
            fieldPanel.GetComponent<Image>().color = new Color32(100, 255, 200, 40);
            GetComponent<Image>().color = new Color32(70, 200, 160, 170);
        }
        else
        {
            fieldPanel.GetComponent<Image>().color = new Color32(255, 100, 100, 40);
            GetComponent<Image>().color = new Color32(200, 70, 70, 170);
        }
    }

    void SpawnCancel()
    {
        // ���
        anim.SetTrigger("isCancel");
        Destroy(go);

        // �ʵ� �г� �����
        fieldPanel.SetActive(false);
    }

    // �� �ڸ� ���
    void MoveTab()
    {
        // ��ǥ ��ġ
        Vector3 pos = panel.GetComponent<CommandPanel>().posList[num].transform.position;

        // ����
        Vector3 temppos = transform.position;

        // �ڸ��� õõ�� �̵�
        temppos = Vector3.Lerp(temppos, pos, Time.deltaTime * 5f);
        transform.position = temppos;
    }

    // �� ��ġ ����
    void TabPositionChange(int p_num)
    {
        if (num > p_num)
        {
            num--;
        }
    }

    bool TimerCheck()
    {
        if(mobInfo.curWaitingTime > 0)
        {
            return false;
        }
        return true;
    }

    void TimerUpdate()
    {
        // ���ð�
        mobInfo.curWaitingTime -= Time.deltaTime;
        timer.text = Mathf.Round(mobInfo.curWaitingTime).ToString();

        if (mobInfo.curWaitingTime <= 0)
        {
            timer.gameObject.SetActive(false);
        }
    }
}
