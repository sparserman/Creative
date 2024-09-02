using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarricadeInfo
{
    public Vector2 position;
    public float hp;
}


public class GameInfo : MonoBehaviour
{
    public bool firstLobby = true;  // ù ���� �κ� ���� Ȯ�ο�


    // �⺻ �ڿ�
    public int eternium = 0;    // �⺻ �ڿ�    ( ���� ��ȭ �� �ǹ� ��ȭ, ��ġ )
    public int MP = 0;  // �Ŵ��� ����Ʈ  (ex. �ڿ� ���޷� ����, Ư�� �Ŵ��� ��ȭ)
    
    public int CP = 0;  // Ư�� ����Ʈ   (ex. ����ź ����, �δ�� ����, �ٸ�����Ʈ ��ž ��)

    // Ư�� ����
    public int coverNum = 2; // �ٸ����̵� ���� �ο� ��
    public float spawnTime = 10f;    // ��ȯ ��� �ð�
    public float soldier1Hp = 0.0f;   // ����1 hp % up
    public float soldier1Mp = 0.0f;   // ����1 mp % up
    public float soldier1Ad = 0.0f;   // ����1 ad % up


    // �κ� ����
    public List<ManagerInfo> managerList = new List<ManagerInfo>();

    public List<Point> pointList = new List<Point>();
    

    // ���� A
    public List<BarricadeInfo> barricadeList = new List<BarricadeInfo>();   // �ٸ����̵� ����
}
