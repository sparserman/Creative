using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PointInfo
{
    // 활성화 된 지역인지
    public bool enable;
    public bool worldDestroy = false;    // 파괴 상태

    public string worldName;
    public Sprite worldImage;
    [TextArea]
    public string worldDescription;
    [TextArea]
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


    // 월드 인게임 정보
    [SerializeField]
    public List<BarricadeInfo> barricadeList = new List<BarricadeInfo>();   // 바리케이드 정보



    // 함수
    // 자원 수집
    public void ResourceCollection()
    {
        switch (resource)
        {
            case Resource.Gold:
                GameManager.GetInstance().gi.gold += resourceAmount;
                break;
            case Resource.Magic:
                GameManager.GetInstance().gi.magic += resourceAmount;
                break;
            case Resource.Food:
                GameManager.GetInstance().gi.food += resourceAmount;
                break;
        }

    }


    // 수집 자원량 표시
    public void UpdateResourceValue()
    {
        float var = 0;
        switch (resource)
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
}


public class Point : MonoBehaviour
{
    GameManager gm;

    GameObject parent;  // Layer1

    public PointInfo pi;

    // 정보창 띄울 위치
    public Vector3 pos;

    void Start()
    {
        gm = GameManager.GetInstance();
        parent = GameObject.Find("Layer1");

        if (pi.worldDestroy)
        {
            gameObject.SetActive(false);
        }

        // 시작 시 지역 정보 넣기
        for (int i = 0; i < gm.gi.pointList.Count; i++)
        {
            if (gm.gi.pointList[i].worldName == pi.worldName)
            {
                pi = gm.gi.pointList[i];
            }
        }
    }

    public void WorldClick()
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/" + "WorldInfo") as GameObject);
        WorldInfo worldInfo = go.GetComponent<WorldInfo>();
        worldInfo.transform.SetParent(parent.transform, true);
        worldInfo.transform.position = transform.position + pos;
        worldInfo.point = this;

        // 관리자 정보 표시
        if (pi.manager != null)
        {
            worldInfo.managerImage.sprite = pi.manager.icon;
            worldInfo.managerName.text = pi.manager.managerName;
            worldInfo.managerLevel.text = "Lv." + pi.manager.level.ToString();
            worldInfo.managerState.text = pi.manager.state;

            worldInfo.manager = pi.manager;
        }
        else
        {
            worldInfo.managerImage.sprite = null;
            worldInfo.managerName.text = "없음";
            worldInfo.managerLevel.text = "";
            worldInfo.managerState.text = "";

            worldInfo.manager = null;
        }

        pi.UpdateResourceValue();
        // 정보 표시
        worldInfo.worldName.text = pi.worldName;
        worldInfo.worldImage.sprite = pi.worldImage;
        worldInfo.worldDescription.text = pi.worldDescription;
        worldInfo.worldDetails.text = pi.worldDetails;
        worldInfo.management.text = pi.management.ToString() + "%";
        worldInfo.population.text = pi.population.ToString() + "명";
        worldInfo.resource.text = pi.resourceAmount.ToString();
        worldInfo.resourceDropdown.value = (int)pi.resource;

        // 사운드

        gm.sm.PlayEffectSound(gm.sm.click);


        gm.goList.Add(worldInfo.gameObject);
    }



}
