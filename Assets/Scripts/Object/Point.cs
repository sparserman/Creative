using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PointInfo
{
    // Ȱ��ȭ �� ��������
    public bool enable;
    public bool worldDestroy = false;    // �ı� ����

    public string worldName;
    public Sprite worldImage;
    [TextArea]
    public string worldDescription;
    [TextArea]
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


    // ���� �ΰ��� ����
    [SerializeField]
    public List<BarricadeInfo> barricadeList = new List<BarricadeInfo>();   // �ٸ����̵� ����



    // �Լ�
    // �ڿ� ����
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


    // ���� �ڿ��� ǥ��
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

    // ����â ��� ��ġ
    public Vector3 pos;

    void Start()
    {
        gm = GameManager.GetInstance();
        parent = GameObject.Find("Layer1");

        if (pi.worldDestroy)
        {
            gameObject.SetActive(false);
        }

        // ���� �� ���� ���� �ֱ�
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

        // ������ ���� ǥ��
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
            worldInfo.managerName.text = "����";
            worldInfo.managerLevel.text = "";
            worldInfo.managerState.text = "";

            worldInfo.manager = null;
        }

        pi.UpdateResourceValue();
        // ���� ǥ��
        worldInfo.worldName.text = pi.worldName;
        worldInfo.worldImage.sprite = pi.worldImage;
        worldInfo.worldDescription.text = pi.worldDescription;
        worldInfo.worldDetails.text = pi.worldDetails;
        worldInfo.management.text = pi.management.ToString() + "%";
        worldInfo.population.text = pi.population.ToString() + "��";
        worldInfo.resource.text = pi.resourceAmount.ToString();
        worldInfo.resourceDropdown.value = (int)pi.resource;

        // ����

        gm.sm.PlayEffectSound(gm.sm.click);


        gm.goList.Add(worldInfo.gameObject);
    }



}
