using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Point : MonoBehaviour
{
    GameManager gm;

    public Canvas canvas;

    public string worldName;
    public Sprite worldImage;
    public string worldDescription;

    public Sprite managerImage;
    public string managerName;
    public string worldDetails;
    public string managerState;
    public float management;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void WorldClick()
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/" + "WorldInfo") as GameObject);
        WorldInfo worldInfo = go.GetComponent<WorldInfo>();
        worldInfo.transform.parent = canvas.transform;
        worldInfo.transform.position = canvas.transform.position;

        // 정보 넣기
        worldInfo.worldName.text = worldName;
        worldInfo.worldImage.sprite = worldImage;
        worldInfo.worldDescription.text = worldDescription;
        worldInfo.managerImage.sprite = managerImage;
        worldInfo.managerName.text = managerName;
        worldInfo.worldDetails.text = worldDetails;
        worldInfo.managerState.text = managerState;
        worldInfo.management.text = management.ToString() + "%";
    }
}
