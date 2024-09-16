using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobInfo
{
    public string nameText;
    public string mobName;
    public int level;
    public float hp;
    public float ad;
}

public class CommandPanel : MonoBehaviour
{
    GameManager gm;

    void Start()
    {
        gm = GameManager.GetInstance();

        for (int i = 0; i < gm.gi.specialMobList.Count; i++)
        {
            GameObject go = Instantiate(Resources.Load("Prefabs/" + "MobTab") as GameObject);
            MobTab tab = go.GetComponent<MobTab>();

            tab.nameText = gm.gi.specialMobList[i].nameText;
            tab.mobName.text = gm.gi.specialMobList[i].mobName;
            tab.level.text = gm.gi.specialMobList[i].level.ToString();
        }
        
    }

    void Update()
    {
        
    }
}
