using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class BarricadeInfo
{
    public bool build;
    public Vector3 position;

    public BarricadeInfo(bool p_build, Vector3 p_position)
    {
        build = p_build;
        position = p_position;
    }
}

[System.Serializable]
public struct SpawnData
{
    public string world;
    public int day;
    public int hour;
    public int minute;
    public string type;
    public int num;
    public int spawner;
}

public enum SpawnDBType
{
    Tutorial = 0
}

[System.Serializable]
public class GameInfo
{
    public bool firstLobby = true;  // ù ���� �κ� ���� Ȯ�ο�

    // �ð�
    public int day = 0;
    public int hour = 0;
    public int minute = 0;

    // �ڿ� ����
    public int gold = 100;      // �⺻ �ڿ� 3����    ( �⺻ ���� ��ȭ[����ź ����], �ǹ� ��ȭ �� ��ġ )
    public int magic = 100;       // Ư�� �ڿ�         ( Ư�� ���� ��ȯ �� Ư�� ���� ��ȭ )
    public int food = 100;        // ���� �Һ� �ڿ�     ( �α� �� ����, ������ ���� )

    public int goldSupply = 0;      // �ð� �� ���޷�
    public int magicSupply = 0;
    public int foodSupply = 0;

    public int MP = 0;  // ������ ����Ʈ  (ex. �������� �ڿ� ���޷� ����, ������ ��ȭ)
    
    public int CP = 0;  // Ư�� ����Ʈ   (ex. �÷��̾� ��ȭ, �α� �� ����, �ٸ�����Ʈ ��ž ��)

    // Ư�� ����

    // ���� �ִ� �α�
    public int population = 4;

    // �ٸ����̵�
    public int coverNum = 2; // �ٸ����̵� ���� �ο� ��

    // ��� ����
    public float spawnTime = 2;    // ��ȯ ��� �ð�

    // ����1
    public float soldier1Hp = 0.0f;                 // Soldier1 hp % up
    public float soldier1Mp = 0.0f;                 // Soldier1 mp % up
    public float soldier1Ad = 0.0f;                 // Soldier1 ad % up
    public float soldier1AttackSpeed = 0.0f;        // Soldier1 attackSpeed % up

    // �� ������
    public float fireWizardHp = 0.0f;               // FireWizard hp % up
    public float fireWizardMp = 0.0f;               // FireWizard mp % up
    public float fireWizardAd = 0.0f;               // FireWizard ad % up
    public float fireWizardAttackSpeed = 0.0f;      // FireWizard attackSpeed % up
    public float fireTime = 3.0f;                   // ��ȭ ���� �ð� (Ư�� ��ȭ�� ����)

    // �÷��̾�
    public float respawnTime = 0.0f;                // ��Ȱ �ð� ���ҷ�


    // �κ� ����
    public List<ManagerInfo> managerList = new List<ManagerInfo>();     // �Ŵ��� ����Ʈ

    public List<PointInfo> pointList = new List<PointInfo>();                   // ���� ����Ʈ

    [SerializeField]
    public List<MobInfo> specialMobList = new List<MobInfo>();             // Ư������ ����Ʈ


    void InfoReset()
    {
        string path = Path.Combine(Application.dataPath, "InitInfo.json");
        string jsonData = File.ReadAllText(path);
        GameManager.GetInstance().gi = JsonUtility.FromJson<GameInfo>(jsonData);
    }
}
