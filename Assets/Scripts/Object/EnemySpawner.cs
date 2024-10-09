using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    bool dir = true;
    GameManager gm;

    void Start()
    {
        gm = GameManager.GetInstance();
        DirectionCheck();
        gm.gi.spawnerList.Add(this);
    }


    void DirectionCheck()
    {
        if(transform.position.x > 0)
        {
            dir = true;
        }
        else
        {
            dir = false;
        }
    }

    // 持失
    public void Spawn(string p_name, int p_num)
    {
        for(int i = 0; i < p_num; i++)
        {
            // 旋 持失
            GameObject go = Instantiate(Resources.Load("Prefabs/Mob/" + p_name) as GameObject);
            go.GetComponent<Enemy>().stat.team = Team.Red;
            if(dir)
            {
                go.transform.position = transform.position + new Vector3(i * 0.3f, 0, 0);
            }
            else
            {
                go.transform.position = transform.position - new Vector3(i * 0.3f, 0, 0);
            }
            
        }
    }
}
