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
    public TextMeshProUGUI eterniumPower;   // 채광량
    public TextMeshProUGUI defensivePower;  // 수비력

    Point tempPoint;    // 이미 배치된 관리자가 있는 지역

    void Start()
    {
        gm = GameManager.GetInstance();

        // 세팅
        image.sprite = manager.image;
        managerName.text = manager.managerName;
        level.text = "Lv." + manager.level.ToString();
        state.text = manager.state;
        eterniumPower.text = manager.eterniumPower.ToString();
        defensivePower.text = manager.defensivePower.ToString();
    }


    public void ManagerSelect()
    {
        // 정보탭 생성
        GameObject go = Instantiate(Resources.Load("Prefabs/" + "CheckBox") as GameObject);
        go.transform.SetParent(transform.parent.parent, false);
        go.transform.position = transform.position;

        go.GetComponent<CheckBox>().obj = gameObject;
        go.GetComponent<CheckBox>().type = E_BoxType.ManagerSelect;


        // 확인창 내용 설정
        // 이미 다른 포인트에 선택한 관리자가 설정 되어있는 지 체크
        if (AllPointManagerCheck() && worldInfo.manager != manager)
        {
            go.GetComponent<CheckBox>().description.text = "이미 다른 지역의 관리자를 담당하고 있습니다.\n관리자를\n변경하시겠습니까?";
        }
        else if (worldInfo.manager != manager)
        {
            go.GetComponent<CheckBox>().description.text = "관리자를\n변경하시겠습니까?";
        }
        else
        {
            go.GetComponent<CheckBox>().description.text = "관리자를\n해제하시겠습니까?";
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

    // 모든 지역의 선택된 관리자 배치 체크
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
