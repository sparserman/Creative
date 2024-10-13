using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementMobImage : MonoBehaviour
{
    GameManager gm;

    public MobInfo mobInfo;
    public MobWindow mobWindow;

    void Start()
    {
        gm = GameManager.GetInstance();
    }

    public void Click()
    {
        mobWindow.InputMobInfo(mobInfo);
    }

    public void MinusClick()
    {
        // 체크박스 생성
        GameObject go = Instantiate(Resources.Load("Prefabs/" + "CheckBox") as GameObject);
        go.transform.SetParent(transform.parent.parent, false);
        go.transform.position = transform.position;

        go.GetComponent<CheckBox>().obj = gameObject;
        go.GetComponent<CheckBox>().type = E_BoxType.MobRelease;


        // 확인창 내용 설정
        go.GetComponent<CheckBox>().description.text = "해제하시겠습니까?";

        gm.goList.Add(go);

    }

}
