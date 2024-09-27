using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MobInfo
{
    public bool having;

    public string nameText;
    public string mobName;
    public int level;
    public Image image;

    public Stat stat;
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

                MobInfo tempInfo = gm.gi.specialMobList[i];

                // 정보 입력
                tab.nameText = tempInfo.nameText;
                tab.mobName.text = tempInfo.mobName;
                tab.level.text = "Lv." + tempInfo.level.ToString();
                tab.image = tempInfo.image;

                tab.hp.text = tempInfo.stat.maxHp.ToString();
                tab.ad.text = tempInfo.stat.ad.ToString();

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
