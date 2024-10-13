using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MobInfo
{
    // Ÿ��
    public EnemyType type;

    // �Ĺ�
    public string faction;

    // ����������
    public bool having;

    // ��ġ�ߴ� ��
    public bool placement;

    // ����
    public string nameText;
    public string mobName;
    public Sprite sprite;
    public Sprite illust;

    public Stat stat;

    // �ڿ�
    public int gold;
    public int magic;
    public int food;

    // ���ð�
    public float curWaitingTime;
    public float waitingTime;

    // ��ų�� ��밡���� ��������
    public bool skillOn;

    // ����
    public void ResetStat()
    {
        stat.hp = stat.maxHp;           // ü�� ȸ��
    }

    // �ʱ�ȭ
    public void InitStat()
    {
        stat.hp = stat.maxHp;           // ü�� ȸ��
        stat.mp = 0;           // MP �ʱ�ȭ
    }
}

public class CommandPanel : MonoBehaviour
{
    GameManager gm;

    public GameObject fieldPanel;

    List<MobInfo> havingTabList = new List<MobInfo>();              // �������ִ� �� ����Ʈ
    public List<GameObject> tabList = new List<GameObject>();              // ���� ǥ�õǰ��ִ� �� ����Ʈ
    public List<GameObject> posList = new List<GameObject>();       // �� ������ ����Ʈ
    public Queue<MobInfo> waitingTab = new Queue<MobInfo>();               // ������� �� ť

    // �� ���� Ÿ�̸�
    public float timer;

    void Start()
    {
        gm = GameManager.GetInstance();

        Init();
    }

    void Update()
    {
        SettingTab();   // �� ����
        TimerUpdate();  // ���ð� ����
    }

    void Init()
    {
        // Ư�� ���� �� ��ġ ����
        posList.Add(GameObject.Find("pos1"));
        posList.Add(GameObject.Find("pos2"));
        posList.Add(GameObject.Find("pos3"));
        posList.Add(GameObject.Find("pos4"));

        posList.Add(GameObject.Find("pos5"));   // �� ���� ��ġ

        // ������ �ִ� Ư������ ����Ʈ�� �ֱ�
        for (int i = 0; i < gm.gi.specialMobList.Count; i++)
        {
            // �������̰�, ��ġ�� ������
            if (gm.gi.specialMobList[i].having && gm.gi.specialMobList[i].placement)
            {
                // ���� �� ����Ʈ�� �ֱ�
                havingTabList.Add(gm.gi.specialMobList[i]);
            }
        }


        int count = havingTabList.Count;
        // ��� ť�� �ֱ�
        for (int i = 0; i < count; i++)
        {
            int n = Random.Range(0, havingTabList.Count - 1);
            waitingTab.Enqueue(havingTabList[n]);
            havingTabList.Remove(havingTabList[n]);
        }

    }

    // �� ����
    void SettingTab()
    {
        if (tabList.Count <= 3)
        {
            float t = 0;
            t += Time.deltaTime;
            if (t >= timer)
            {
                if (waitingTab.Count > 0)
                {
                    // �� ����
                    GameObject mobTab = Instantiate(Resources.Load("Prefabs/" + "MobTab") as GameObject);
                    mobTab.transform.SetParent(transform, false);
                    mobTab.transform.position = posList[4].transform.position;      // �ʱ� ������ ������ �� ������
                    mobTab.GetComponent<MobTab>().panel = gameObject;               // ������ panel�� �־��ְ�
                    mobTab.GetComponent<MobTab>().fieldPanel = fieldPanel;               // �ʵ�� �ʵ� �ְ�
                    mobTab.GetComponent<MobTab>().mobInfo = waitingTab.Dequeue();

                    tabList.Add(mobTab);
                    // �гο����� ��ġ
                    mobTab.GetComponent<MobTab>().num = tabList.Count - 1;

                    t = 0;
                }
            }
        }
    }

    void TimerUpdate()
    {
        // ���ð�
        for (int i = 0; i < gm.gi.specialMobList.Count; i++)
        {
            // �������̸鼭
            if (gm.gi.specialMobList[i].having && gm.gi.specialMobList[i].placement)
            {
                // Ÿ�̸Ӱ� ���� �ֵ�
                if (gm.gi.specialMobList[i].curWaitingTime >= 0)
                {
                    gm.gi.specialMobList[i].curWaitingTime -= Time.deltaTime;
                }
            }
        }
    }
}
