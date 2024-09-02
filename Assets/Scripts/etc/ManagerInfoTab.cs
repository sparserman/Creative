using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerInfoTab : MonoBehaviour
{
    GameManager gm;

    public ManagerInfo manager;

    public WorldInfo worldInfo;

    public Image image;
    public TextMeshProUGUI managerName;
    public TextMeshProUGUI level;
    public TextMeshProUGUI state;
    public TextMeshProUGUI eterniumPower;   // ä����
    public TextMeshProUGUI defensivePower;  // �����

    Point tempPoint;    // �̹� ��ġ�� �����ڰ� �ִ� ����

    void Start()
    {
        gm = GameManager.GetInstance();

        // ����
        image.sprite = manager.image;
        managerName.text = manager.managerName;
        level.text = "Lv." + manager.level.ToString();
        state.text = manager.state;
        eterniumPower.text = manager.eterniumPower.ToString();
        defensivePower.text = manager.defensivePower.ToString();
    }


    public void ManagerSelect()
    {
        // ������ ����
        GameObject go = Instantiate(Resources.Load("Prefabs/" + "CheckBox") as GameObject);
        go.transform.SetParent(transform.parent.parent, false);
        go.transform.position = transform.position;

        go.GetComponent<CheckBox>().obj = gameObject;
        go.GetComponent<CheckBox>().type = E_BoxType.ManagerSelect;


        // Ȯ��â ���� ����
        // �̹� �ٸ� ����Ʈ�� ������ �����ڰ� ���� �Ǿ��ִ� �� üũ
        if (AllPointManagerCheck() && worldInfo.manager != manager)
        {
            go.GetComponent<CheckBox>().description.text = "�̹� �ٸ� ������ �����ڸ� ����ϰ� �ֽ��ϴ�.\n�����ڸ�\n�����Ͻðڽ��ϱ�?";
        }
        else if (worldInfo.manager != manager)
        {
            go.GetComponent<CheckBox>().description.text = "�����ڸ�\n�����Ͻðڽ��ϱ�?";
        }
        else
        {
            go.GetComponent<CheckBox>().description.text = "�����ڸ�\n�����Ͻðڽ��ϱ�?";
        }

        gm.goList.Add(go);

        if (gm.sm != null)
        {
            gm.sm.PlayEffectSound(gm.sm.click);
        }
    }

    public void ManagerChange()
    {
        if (worldInfo.manager != null)
        {
            if (worldInfo.manager.mCode == manager.mCode)
            {
                worldInfo.ManagerClear();
            }
            else
            {
                if(AllPointManagerCheck())
                {
                    tempPoint.manager = null;
                    tempPoint = null;
                    worldInfo.ManagerSelect(manager);
                }
                else
                {
                    worldInfo.ManagerSelect(manager);
                }
            }
        }
        else
        {
            if (AllPointManagerCheck())
            {
                tempPoint.manager = null;
                tempPoint = null;
                worldInfo.ManagerSelect(manager);
            }
            else
            {
                worldInfo.ManagerSelect(manager);
            }
        }
    }

    // ��� ������ ���õ� ������ ��ġ üũ
    bool AllPointManagerCheck()
    {
        bool flag = false;
        for (int i = 0; i < gm.gi.pointList.Count; i++)
        {
            if (gm.gi.pointList[i].manager != null)
            {
                if (gm.gi.pointList[i].manager == manager)
                {
                    flag = true;
                    tempPoint = gm.gi.pointList[i];
                    break;
                }
            }
        }

        return flag;
    }
}
