using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum Resource
{
    Gold = 0,
    Magic,
    Food
}

public class WorldInfo : MonoBehaviour
{
    GameManager gm;
    Animator anim;

    public Point point;

    // ������ ����
    public ManagerInfo manager;

    public TextMeshProUGUI worldName;    // ���� �̸�
    public Image worldImage;    // ���� �̹���
    public TextMeshProUGUI worldDescription; // ���� ����

    public Image managerImage;  // �Ŵ��� �̹���
    public TextMeshProUGUI managerName;  // �Ŵ��� �̸�
    public TextMeshProUGUI managerLevel;  // �Ŵ��� ����
    public TextMeshProUGUI worldDetails; // ���� ��Ȳ
    public TextMeshProUGUI managerState; // �Ŵ��� ����

    public TextMeshProUGUI management;    // ������
    public TextMeshProUGUI population;    // �α�
    public TextMeshProUGUI resource;    // �ڿ�������

    public TMP_Dropdown resourceDropdown;   // �ڿ� ���� ��Ӵٿ�


    void Start()
    {
        anim = GetComponent<Animator>();   

        gm = GameManager.GetInstance();
    }


    public void VisitButton()
    {
        // int�� �����ؼ�
        switch(worldName.text)
        {
            case "��Ƽ��":
                gm.lm.StartFadeIn((int)STAGE.WORLD_A);
                break;
        }
        

        gm.sm.PlayEffectSound(gm.sm.click);

    }

    public void WorldInfoOff()
    {
        anim.SetTrigger("Off");
        gm.goList.Remove(gameObject);


        gm.sm.PlayEffectSound(gm.sm.click);

    }

    GameObject worldTab = null;
    public void ManagerTabOpen()
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/" + "ManagerTab") as GameObject);
        go.transform.SetParent(transform, false);
        go.GetComponent<ManagerTab>().worldInfo = this;

        gm.goList.Add(go);
        worldTab = go;


        gm.sm.PlayEffectSound(gm.sm.click);

    }

    // ������ ����
    public void ManagerSelect(ManagerInfo p_mi)
    {
        manager = p_mi;

        // ���� �����ڰ� �ִ� ������ ������ ����
        for (int i = 0; i < gm.gi.pointList.Count; i++)
        {
            if (gm.goList[i].GetComponent<WorldInfo>() != null)
            {
                if(gm.goList[i].GetComponent<WorldInfo>().manager == manager
                    && gm.goList[i].GetComponent<WorldInfo>() != this)
                {
                    gm.goList[i].GetComponent<WorldInfo>().manager = null;
                }
            }
        }

        gm.WorldInfoUpdate();

        worldTab.GetComponent<Animator>().SetTrigger("Off");
        gm.goList.Remove(worldTab);
    }

    // ������ ����
    public void ManagerClear()
    {
        manager = null;
        gm.WorldInfoUpdate();

        worldTab.GetComponent<Animator>().SetTrigger("Off");
        gm.goList.Remove(worldTab);
    }

    // ������ ������ ���� �ֽ�ȭ
    public void ManagerUpdate()
    {
        for (int i = 0; i < gm.gi.pointList.Count; i++)
        {
            if (gm.gi.pointList[i].worldName == worldName.text)
            {
                gm.gi.pointList[i].manager = manager;
            }
        }

        if(manager == null)
        {
            managerImage.sprite = null;
            managerName.text = "";
            managerLevel.text = "";
            managerState.text = "";
        }
        else
        {
            managerImage.sprite = manager.icon;
            managerName.text = manager.managerName;
            managerLevel.text = "Lv." + manager.level.ToString();
            managerState.text = manager.state;
        }
    }

    public void Restart()
    {
        gm.goList.Remove(gameObject);
        Destroy(gameObject);
        point.WorldClick();
    }

    public void DestroyWorldInfo()
    {
        Destroy(gameObject);
    }

    public void ResourceChange(int p_index)
    {
        if (gm != null)
        {
            PointInfo pi = null;
            for (int i = 0; i < gm.gi.pointList.Count; i++)
            {
                if (gm.gi.pointList[i].worldName == worldName.text)
                {
                    pi = gm.gi.pointList[i];
                }
            }

            switch (p_index)
            {
                case (int)Resource.Gold:
                    pi.resource = Resource.Gold;
                    break;
                case (int)Resource.Magic:
                    pi.resource = Resource.Magic;
                    break;
                case (int)Resource.Food:
                    pi.resource = Resource.Food;
                    break;
            }

            pi.UpdateResourceValue();
            resource.text = pi.resourceAmount.ToString();
        }
    }

}
