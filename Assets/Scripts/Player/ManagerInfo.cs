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

    // ������ ����
    public E_ManagerCode mCode;

    public Sprite image;
    public string managerName;
    public float level;
    public string state;
    public float eterniumPower;   // ä����
    public float defensivePower;  // �����

    // ����������
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
