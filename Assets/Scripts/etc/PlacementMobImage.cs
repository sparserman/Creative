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
        // üũ�ڽ� ����
        GameObject go = Instantiate(Resources.Load("Prefabs/" + "CheckBox") as GameObject);
        go.transform.SetParent(transform.parent.parent, false);
        go.transform.position = transform.position;

        go.GetComponent<CheckBox>().obj = gameObject;
        go.GetComponent<CheckBox>().type = E_BoxType.MobRelease;


        // Ȯ��â ���� ����
        go.GetComponent<CheckBox>().description.text = "�����Ͻðڽ��ϱ�?";

        gm.goList.Add(go);

    }

}
