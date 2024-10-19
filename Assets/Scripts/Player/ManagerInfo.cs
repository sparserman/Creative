using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ManagerInfo
{
    // ������ ����
    public Sprite icon;
    public Sprite illust;
    public string managerName;
    public float level;
    public string state;
    public float eterniumPower;   // ä����
    public float defensivePower;  // �����

    // ����������
    public bool having = false;
}
