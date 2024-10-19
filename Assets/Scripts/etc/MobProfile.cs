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
        // 확인창 내용 설정
        // 꽉 찼는 지 체크
        if (ListMaxCheck())
        {
            // 체크박스 생성
            GameObject go = Instantiate(Resources.Load("Prefabs/" + "CheckBox") as GameObject);
            go.transform.SetParent(transform.parent.parent, false);
            go.transform.position = transform.position;

            go.GetComponent<CheckBox>().obj = gameObject;
            go.GetComponent<CheckBox>().description.text = "부대가 가득 찼습니다.";
            go.GetComponent<CheckBox>().button1.gameObject.transform.parent.gameObject.SetActive(false);    // yes버튼 없애기
            go.GetComponent<CheckBox>().button2.text = "확인";

            gm.goList.Add(go);
        }
        else
        {
            mobInfo.placement = true;
            // 표시 다시하기
            mobWindow.InitMobProfile();
            mobWindow.MobProfileSetting();
        }
    }

    // 배치할 8칸이 다 찼는 지 체크
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
