using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ManagerInfo
{
    // 관리자 정보
    public Sprite icon;
    public Sprite illust;
    public string managerName;
    public float level;
    public string state;
    public float eterniumPower;   // 채광량
    public float defensivePower;  // 수비력

    // 보유중인지
    public bool having = false;
}
