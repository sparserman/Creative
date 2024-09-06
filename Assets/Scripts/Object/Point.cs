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

public class PointInfo
{
    // 관리자 정보
    public ManagerInfo manager;

    public WorldCode worldCode;

    public string worldName;
    public Sprite worldImage;
    public string worldDescription;
    public string worldDetails;

    public float management;    // 관리율

    public PointInfo(WorldCode p_worldCode = WorldCode.A, string p_worldName = "None"
        , Sprite p_worldImage = null, string p_worldDescription = "", string p_worldDetails = "", float p_management = 0)
    {
        worldCode = p_worldCode;

        worldName = p_worldName;
        worldImage = p_worldImage;
        worldDescription = p_worldDescription;
        worldDetails = p_worldDetails;

        management = p_management;
    }

    public PointInfo DeepCopy()
    {
        PointInfo clone = new PointInfo();

        clone.manager = manager;

        clone.worldCode = worldCode;

        clone.worldName = worldName;
        clone.worldImage = worldImage;
        clone.worldDescription = worldDescription;
        clone.worldDetails = worldDetails;

        clone.management = management;

        return clone;
    }
}

public class Point : MonoBehaviour
{
    public bool infoInputCheck = false;

    GameManager gm;

    GameObject parent;

    // 기입 될 정보들
    public WorldCode worldCode;

    public string worldName;
    public Sprite worldImage;
    public string worldDescription;
    public string worldDetails;

    public float management;    // 관리율

    // 관리자 정보
    public ManagerInfo manager;

    // 정보
    public PointInfo info;

    // 정보창 띄울 위치
    public Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.GetInstance();
        parent = GameObject.Find("Layer1");
        info = new PointInfo(worldCode, worldName, worldImage, worldDescription, worldDetails, management);

        if (gm.gi.worldADestroy)
        {
            gameObject.SetActive(false);
        }

        if (gm.gi.firstLobby)
        {
            if (infoInputCheck)
            {
                gm.gi.pointList.Add(this);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void WorldClick()
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/" + "WorldInfo") as GameObject);
        WorldInfo worldInfo = go.GetComponent<WorldInfo>();
        worldInfo.transform.SetParent(parent.transform, true);
        worldInfo.transform.position = transform.position + pos;
        worldInfo.point = this;

        Point p = null;

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
            worldInfo.managerImage.sprite = p.manager.image;
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

        // 정보 표시
        worldInfo.worldName.text = p.worldName;
        worldInfo.worldImage.sprite = p.worldImage;
        worldInfo.worldDescription.text = p.worldDescription;
        worldInfo.worldDetails.text = p.worldDetails;
        worldInfo.management.text = p.management.ToString() + "%";

        worldInfo.worldCode = p.worldCode;

        worldInfo.gameObject.SetActive(true);

        // 사운드
        if (gm.sm != null)
        {
            gm.sm.PlayEffectSound(gm.sm.click);
        }

        gm.goList.Add(worldInfo.gameObject);
    }

}
