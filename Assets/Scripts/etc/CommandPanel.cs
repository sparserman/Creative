using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MobInfo
{
    // ����������
    public bool having;

    // ����
    public string nameText;
    public string mobName;
    public int level;
    public Sprite sprite;

    public Stat stat;

    // �ڿ�
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
            // �������̶��
            if (gm.gi.specialMobList[i].having)
            {
                GameObject go = Instantiate(Resources.Load("Prefabs/" + "MobTab") as GameObject);
                MobTab tab = go.GetComponent<MobTab>();

                // ���� �Է�
                tab.mobInfo = gm.gi.specialMobList[i];

                // �г� �ֱ�
                tab.panel = gameObject;
                tab.fieldPanel = fieldPanel;
            }

        }
        
    }

    void Update()
    {
        
    }
}
