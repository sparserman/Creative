using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum WorldCode
{
    A = 11,
    B,
    C,
    D
}

//public class PointInfo
//{
//    // 관리자 정보
//    public ManagerInfo manager;

//    public WorldCode worldCode;

//    public string worldName;
//    public Sprite worldImage;
//    public string worldDescription;
//    public string worldDetails;

//    public float management;    // 관리율

//    public PointInfo(WorldCode p_worldCode = WorldCode.A, string p_worldName = "None"
//        , Sprite p_worldImage = null, string p_worldDescription = "", string p_worldDetails = "", float p_management = 0)
//    {
//        worldCode = p_worldCode;

//        worldName = p_worldName;
//        worldImage = p_worldImage;
//        worldDescription = p_worldDescription;
//        worldDetails = p_worldDetails;

//        management = p_management;
//    }

//    //public PointInfo DeepCopy()
//    //{
//    //    PointInfo clone = new PointInfo();

//    //    clone.manager = manager;

//    //    clone.worldCode = worldCode;

//    //    clone.worldName = worldName;
//    //    clone.worldImage = worldImage;
//    //    clone.worldDescription = worldDescription;
//    //    clone.worldDetails = worldDetails;

//    //    clone.management = management;

//    //    return clone;
//    //}
//}

public class Point : MonoBehaviour
{
    public bool infoInputCheck = false;

    GameManager gm;

    GameObject parent;

    public Point p;

    // 활성화 된 지역인지
    public bool enable;
    public bool worldDestroy = false;    // 파괴 상태

    // 기입 될 정보들
    public WorldCode worldCode;

    public string worldName;
    public Sprite worldImage;
    public string worldDescription;
    public string worldDetails;

    // 자원 관련
    public float management;                // 관리율 ( 여러 이벤트에 영향 및 자원 수집량에 효율 )
    public float population;                // 인구       ( 자원 수집량에 영향 및 음식 소모량 증가 )
    public int resourceAmount;              // 자원수집량
    public Resource resource;               // 수집중인 자원

    // 지역의 자원 효율
    public float goldEfficiency;
    public float magicEfficiency;
    public float foodEfficiency;

    // 관리자 정보
    public ManagerInfo manager;

    // 정보창 띄울 위치
    public Vector3 pos;

    // 월드 인게임 정보
    [SerializeField]
    public List<BarricadeInfo> barricadeList = new List<BarricadeInfo>();   // 바리케이드 정보


    void Start()
    {
        gm = GameManager.GetInstance();
        parent = GameObject.Find("Layer1");

        if (worldDestroy)
        {
            gameObject.SetActive(false);
        }

        if (infoInputCheck)
        {
            gm.gi.pointList.Add(this);
        }
    }

    public void WorldClick()
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/" + "WorldInfo") as GameObject);
        WorldInfo worldInfo = go.GetComponent<WorldInfo>();
        worldInfo.transform.SetParent(parent.transform, true);
        worldInfo.transform.position = transform.position + pos;
        worldInfo.point = this;

        // 지역 정보 넣기
        for(int i = 0; i < gm.gi.pointList.Count; i++)
        {
            if(gm.gi.pointList[i].worldCode == worldCode)
            {
                p = gm.gi.pointList[i];
            }
        }

        // 관리자 정보 표시
        if (p.manager != null)
        {
            worldInfo.managerImage.sprite = p.manager.icon;
            worldInfo.managerName.text = p.manager.managerName;
            worldInfo.managerLevel.text = "Lv." + p.manager.level.ToString();
            worldInfo.managerState.text = p.manager.state;

            worldInfo.manager = p.manager;
        }
        else
        {
            worldInfo.managerImage.sprite = null;
            worldInfo.managerName.text = "없음";
            worldInfo.managerLevel.text = "";
            worldInfo.managerState.text = "";

            worldInfo.manager = null;
        }

        p.UpdateResourceValue();
        // 정보 표시
        worldInfo.worldName.text = p.worldName;
        worldInfo.worldImage.sprite = p.worldImage;
        worldInfo.worldDescription.text = p.worldDescription;
        worldInfo.worldDetails.text = p.worldDetails;
        worldInfo.management.text = p.management.ToString() + "%";
        worldInfo.population.text = p.population.ToString() + "명";
        worldInfo.resource.text = p.resourceAmount.ToString();
        worldInfo.resourceDropdown.value = (int)p.resource;

        worldInfo.worldCode = p.worldCode;

        // 사운드
        if (gm.sm != null)
        {
            gm.sm.PlayEffectSound(gm.sm.click);
        }

        gm.goList.Add(worldInfo.gameObject);
    }

    // 문제의 위치
    public void UpdateResourceValue()
    {
        float var = 0;
        switch(resource)
        {
            case Resource.Gold:
                var = goldEfficiency;
                break;
            case Resource.Magic:
                var = magicEfficiency;
                break;
            case Resource.Food:
                var = foodEfficiency;
                break;
        }

        resourceAmount = (int)(population * var * (management * 0.01f));
    }

    // 자원 수집
    public void ResourceCollection()
    {
        switch(resource)
        {
            case Resource.Gold:
                gm.gi.gold += resourceAmount;
                break;
            case Resource.Magic:
                gm.gi.magic += resourceAmount;
                break;
            case Resource.Food:
                gm.gi.food += resourceAmount;
                break;
        }
        
    }
}
