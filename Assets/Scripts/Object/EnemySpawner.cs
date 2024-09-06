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

    // ����
    public void Spawn(string p_name, int p_num)
    {
        for(int i = 0; i < p_num; i++)
        {
            // ������ ����
            GameObject go = Instantiate(Resources.Load("Prefabs/" + p_name) as GameObject);
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
