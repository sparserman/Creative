using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MobProfile : MonoBehaviour
{
    GameManager gm;

    public MobInfo mobInfo;

    public Image image;
    public TextMeshProUGUI mobName;
    public TextMeshProUGUI level;

    public MobWindow mobWindow;
    
    void Start()
    {
        gm = GameManager.GetInstance();
    }

    public void Click()
    {
        mobWindow.InputMobInfo(mobInfo);
    }

    public void PlusClick()
    {
        // üũ�ڽ� ����
        GameObject go = Instantiate(Resources.Load("Prefabs/" + "CheckBox") as GameObject);
        go.transform.SetParent(transform.parent.parent, false);
        go.transform.position = transform.position;

        go.GetComponent<CheckBox>().obj = gameObject;
        go.GetComponent<CheckBox>().type = E_BoxType.MobPlacement;


        // Ȯ��â ���� ����
        // �� á�� �� üũ
        if (ListMaxCheck())
        {
            go.GetComponent<CheckBox>().description.text = "�δ밡 ���� á���ϴ�.";
            go.GetComponent<CheckBox>().button1.gameObject.transform.parent.gameObject.SetActive(false);    // yes��ư ���ֱ�
            go.GetComponent<CheckBox>().button2.text = "Ȯ��";
        }
        else
        {
            go.GetComponent<CheckBox>().description.text = "��ġ�Ͻðڽ��ϱ�?";
        }

        gm.goList.Add(go);

    }

    // ��ġ�� 8ĭ�� �� á�� �� üũ
    bool ListMaxCheck()
    {
        int n = 0;
        for (int i = 0; i < gm.gi.specialMobList.Count; i++)
        {
            if (gm.gi.specialMobList[i].placement)
            {
                n++;
                if(n >= 8)
                {
                    return true;
                }
            }
        }

        return false;
    }
}