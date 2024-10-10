using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 파벌 (관리자 산하)
public enum Faction
{
    Player = 0,
    Liora
}

[System.Serializable]
public class MobInfo
{
    // 소유중인지
    public bool having;

    // 배치했는 지
    public bool placement;

    // 정보
    public string nameText;
    public string mobName;
    public int level;
    public Sprite sprite;

    public Stat stat;

    // 자원
    public int gold;
    public int magic;
    public int food;

    // 대기시간
    public float curWaitingTime;
    public float waitingTime;
}

public class CommandPanel : MonoBehaviour
{
    GameManager gm;

    public GameObject fieldPanel;

    List<MobInfo> havingTabList = new List<MobInfo>();              // 가지고있는 탭 리스트
    public List<GameObject> tabList = new List<GameObject>();              // 현재 표시되고있는 탭 리스트
    public List<GameObject> posList = new List<GameObject>();       // 탭 포지션 리스트
    public Queue<MobInfo> waitingTab = new Queue<MobInfo>();               // 대기중인 탭 큐

    // 탭 생성 타이머
    public float timer;

    void Start()
    {
        gm = GameManager.GetInstance();

        Init();
    }

    void Update()
    {
        SettingTab();   // 탭 세팅
    }

    void Init()
    {
        // 특수 병사 탭 위치 설정
        posList.Add(GameObject.Find("pos1"));
        posList.Add(GameObject.Find("pos2"));
        posList.Add(GameObject.Find("pos3"));
        posList.Add(GameObject.Find("pos4"));

        posList.Add(GameObject.Find("pos5"));   // 탭 생성 위치

        // 가지고 있는 특수병사 리스트를 넣기
        for (int i = 0; i < gm.gi.specialMobList.Count; i++)
        {
            // 보유중이고, 배치한 병사라면
            if (gm.gi.specialMobList[i].having && gm.gi.specialMobList[i].placement)
            {
                // 보유 탭 리스트에 넣기
                havingTabList.Add(gm.gi.specialMobList[i]);
            }
        }

        // 대기 큐에 넣기
        for (int i = 0; i <= havingTabList.Count; i++)
        {
            int n = Random.Range(0, havingTabList.Count - 1);
            waitingTab.Enqueue(havingTabList[n]);
            havingTabList.Remove(havingTabList[n]);
        }

    }

    // 탭 세팅
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
                    // 탭 생성
                    GameObject mobTab = Instantiate(Resources.Load("Prefabs/" + "MobTab") as GameObject);
                    mobTab.transform.SetParent(transform, false);
                    mobTab.transform.position = posList[4].transform.position;      // 초기 지점은 무조건 맨 오른쪽
                    mobTab.GetComponent<MobTab>().panel = gameObject;               // 본인을 panel에 넣어주고
                    mobTab.GetComponent<MobTab>().fieldPanel = fieldPanel;               // 필드는 필드 넣고
                    mobTab.GetComponent<MobTab>().mobInfo = waitingTab.Dequeue();

                    tabList.Add(mobTab);
                    // 패널에서의 위치
                    mobTab.GetComponent<MobTab>().num = tabList.Count - 1;

                    t = 0;
                }
            }
        }
    }
}
