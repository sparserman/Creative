using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_ManagerCode
{
    SIGMA,
    INE,
    JINGBURGER,
    JURURU
}

public class ManagerInfo : MonoBehaviour
{
    GameManager gm;

    // 관리자 정보
    public E_ManagerCode mCode;

    public Sprite image;
    public string managerName;
    public float level;
    public string state;
    public float eterniumPower;   // 채광량
    public float defensivePower;  // 수비력

    // 보유중인지
    public bool having = false;

    void Start()
    {
        gm = GameManager.GetInstance();

        if (gm.gi.firstLobby)
        {
            gm.gi.managerList.Add(this);
        }
    }

}
