using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobInfoGroup : MonoBehaviour
{
    public List<MobInfo> specialMobInfoList = new List<MobInfo>();

    private void Start()
    {
        for (int i = 0; i < specialMobInfoList.Count; i++)
        {
            GameManager.GetInstance().gi.specialMobList.Add(specialMobInfoList[i]);
        }
    }
}
