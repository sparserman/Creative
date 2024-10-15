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

    // 관리자 정보
    public ManagerInfo manager;

    public TextMeshProUGUI worldName;    // 월드 이름
    public Image worldImage;    // 월드 이미지
    public TextMeshProUGUI worldDescription; // 월드 설명

    public Image managerImage;  // 매니저 이미지
    public TextMeshProUGUI managerName;  // 매니저 이름
    public TextMeshProUGUI managerLevel;  // 매니저 레벨
    public TextMeshProUGUI worldDetails; // 월드 근황
    public TextMeshProUGUI managerState; // 매니저 상태

    public TextMeshProUGUI management;    // 관리율
    public TextMeshProUGUI population;    // 인구
    public TextMeshProUGUI resource;    // 자원수집률

    public TMP_Dropdown resourceDropdown;   // 자원 설정 드롭다운

    public WorldCode worldCode = 0;


    void Start()
    {
        anim = GetComponent<Animator>();   

        gm = GameManager.GetInstance();
    }


    public void VisitButton()
    {
        // int로 변경해서
        gm.lm.StartFadeIn((int)worldCode);

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

    // 관리자 선택
    public void ManagerSelect(ManagerInfo p_mi)
    {
        manager = p_mi;

        // 같은 관리자가 있는 지역의 관리자 빼기
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

    // 관리자 해제
    public void ManagerClear()
    {
        manager = null;
        gm.WorldInfoUpdate();

        worldTab.GetComponent<Animator>().SetTrigger("Off");
        gm.goList.Remove(worldTab);
    }

    // 지역의 관리자 정보 최신화
    public void ManagerUpdate()
    {
        for (int i = 0; i < gm.gi.pointList.Count; i++)
        {
            if (gm.gi.pointList[i].worldCode == worldCode)
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
            Point p = null;
            for (int i = 0; i < gm.gi.pointList.Count; i++)
            {
                if (gm.gi.pointList[i].worldCode == worldCode)
                {
                    p = gm.gi.pointList[i];
                }
            }

            switch (p_index)
            {
                case (int)Resource.Gold:
                    p.resource = Resource.Gold;
                    break;
                case (int)Resource.Magic:
                    p.resource = Resource.Magic;
                    break;
                case (int)Resource.Food:
                    p.resource = Resource.Food;
                    break;
            }

            p.UpdateResourceValue();
            resource.text = p.resourceAmount.ToString();
        }
    }

}
