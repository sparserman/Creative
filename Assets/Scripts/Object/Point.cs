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
//    // ������ ����
//    public ManagerInfo manager;

//    public WorldCode worldCode;

//    public string worldName;
//    public Sprite worldImage;
//    public string worldDescription;
//    public string worldDetails;

//    public float management;    // ������

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

    // Ȱ��ȭ �� ��������
    public bool enable;
    public bool worldDestroy = false;    // �ı� ����

    // ���� �� ������
    public WorldCode worldCode;

    public string worldName;
    public Sprite worldImage;
    public string worldDescription;
    public string worldDetails;

    // �ڿ� ����
    public float management;                // ������ ( ���� �̺�Ʈ�� ���� �� �ڿ� �������� ȿ�� )
    public float population;                // �α�       ( �ڿ� �������� ���� �� ���� �Ҹ� ���� )
    public int resourceAmount;              // �ڿ�������
    public Resource resource;               // �������� �ڿ�

    // ������ �ڿ� ȿ��
    public float goldEfficiency;
    public float magicEfficiency;
    public float foodEfficiency;

    // ������ ����
    public ManagerInfo manager;

    // ����â ��� ��ġ
    public Vector3 pos;

    // ���� �ΰ��� ����
    [SerializeField]
    public List<BarricadeInfo> barricadeList = new List<BarricadeInfo>();   // �ٸ����̵� ����


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

        // ���� ���� �ֱ�
        for(int i = 0; i < gm.gi.pointList.Count; i++)
        {
            if(gm.gi.pointList[i].worldCode == worldCode)
            {
                p = gm.gi.pointList[i];
            }
        }

        // ������ ���� ǥ��
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
            worldInfo.managerName.text = "����";
            worldInfo.managerLevel.text = "";
            worldInfo.managerState.text = "";

            worldInfo.manager = null;
        }

        p.UpdateResourceValue();
        // ���� ǥ��
        worldInfo.worldName.text = p.worldName;
        worldInfo.worldImage.sprite = p.worldImage;
        worldInfo.worldDescription.text = p.worldDescription;
        worldInfo.worldDetails.text = p.worldDetails;
        worldInfo.management.text = p.management.ToString() + "%";
        worldInfo.population.text = p.population.ToString() + "��";
        worldInfo.resource.text = p.resourceAmount.ToString();
        worldInfo.resourceDropdown.value = (int)p.resource;

        worldInfo.worldCode = p.worldCode;

        // ����
        if (gm.sm != null)
        {
            gm.sm.PlayEffectSound(gm.sm.click);
        }

        gm.goList.Add(worldInfo.gameObject);
    }

    // ������ ��ġ
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

    // �ڿ� ����
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
