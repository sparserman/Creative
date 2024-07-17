using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Point : MonoBehaviour
{
    GameManager gm;
    SoundManager sm;

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
        sm = gm.GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void WorldClick()
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/" + "WorldInfo") as GameObject);
        WorldInfo worldInfo = go.GetComponent<WorldInfo>();
        worldInfo.transform.SetParent(canvas.transform, true);
        worldInfo.transform.position = transform.position;

        // ���� �ֱ�
        worldInfo.worldName.text = worldName;
        worldInfo.worldImage.sprite = worldImage;
        worldInfo.worldDescription.text = worldDescription;
        worldInfo.managerImage.sprite = managerImage;
        worldInfo.managerName.text = managerName;
        worldInfo.worldDetails.text = worldDetails;
        worldInfo.managerState.text = managerState;
        worldInfo.management.text = management.ToString() + "%";

        worldInfo.gameObject.SetActive(true);

        // ����
        if (sm != null)
        {
            sm.PlayEffectSound(sm.click);
        }

        gm.goList.Add(worldInfo.gameObject);
    }
}
