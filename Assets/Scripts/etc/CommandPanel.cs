using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MobInfo
{
    // 소유중인지
    public bool having;

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
}

public class CommandPanel : MonoBehaviour
{
    GameManager gm;

    public GameObject fieldPanel;

    void Start()
    {
        gm = GameManager.GetInstance();

        for (int i = 0; i < gm.gi.specialMobList.Count; i++)
        {
            // 보유중이라면
            if (gm.gi.specialMobList[i].having)
            {
                GameObject go = Instantiate(Resources.Load("Prefabs/" + "MobTab") as GameObject);
                MobTab tab = go.GetComponent<MobTab>();

                // 정보 입력
                tab.mobInfo = gm.gi.specialMobList[i];

                // 패널 넣기
                tab.panel = gameObject;
                tab.fieldPanel = fieldPanel;
            }

        }
        
    }

    void Update()
    {
        
    }
}
