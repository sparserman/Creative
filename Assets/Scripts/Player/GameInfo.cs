using System.Collections;
using System.Collections.Generic;
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

public class GameInfo : MonoBehaviour
{
    GameManager gm;

    public bool firstLobby = true;  // ù ���� �κ� ���� Ȯ�ο�


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

    public List<Point> pointList = new List<Point>();                   // ���� ����Ʈ

    [SerializeField]
    public List<MobInfo> specialMobList = new List<MobInfo>();             // Ư������ ����Ʈ


    // ���� ����
    //TextAsset spawnTxt;
    //string[,] spawnData;
    //int lineSize, rowSize;
    //int spawnWave;         // ���� ���̺�

    // ���� ����
    public SpawnDB spawnDB;
    public List<SpawnData> spawnList = new List<SpawnData>();
    public SpawnDBType typeDB;
    public int branch;          // ���� �б�
    public int spawnWave;       // ���� ���̺�

    // ���� ���� ����
    public Command command;
    public List<EnemySpawner> spawnerList = new List<EnemySpawner>();
    public int rightMobNum;    // ������ �� ����
    public int leftMobNum;     // ���� �� ����
    public int rightBarricadeNum;   // ������ �ٸ����̵� ����
    public int leftBarricadeNum;    // ���� �ٸ����̵� ����

    void Start()
    {
        gm = GameManager.GetInstance();

        //SettingSpawnData();
        SpawnListSetting();
    }

    void Update()
    {
        SpawnTimer();
        ResourceSupplyCalc();
    }


    // ���� ����Ʈ�� �Է�
    void SpawnListSetting()
    {
        spawnDB = Instantiate(Resources.Load("Datas/" + "SpawnDB") as SpawnDB);

        List<SpawnDBEntity> listDB = null;
        // DB ���� ����
        switch (typeDB)
        {
            case SpawnDBType.Tutorial:
                listDB = spawnDB.Tutorial;
                break;
        }

        if (listDB != null)
        {
            // ����Ʈ���� ���� ����
            spawnList.Clear();

            // DB ���� �Է�
            for (int i = 0; i < listDB.Count; i++)
            {
                SpawnData data = new SpawnData();
                
                data.world = listDB[i].world;
                data.day = listDB[i].day;
                data.hour = listDB[i].hour;
                data.minute = listDB[i].minute;
                data.type = listDB[i].type;
                data.num = listDB[i].num;
                data.spawner = listDB[i].spawner;

                spawnList.Add(data);
            }
        }
    }

    void SpawnTimer()
    {
        bool flag = false;

        if (spawnList.Count > spawnWave)
        {
            // ���� �ð� üũ
            if (spawnList[spawnWave].day == gm.day && spawnList[spawnWave].hour == gm.hour && spawnList[spawnWave].minute == gm.minute)
            {
                // ���� ���� üũ
                if (command != null)
                {
                    if (spawnList[spawnWave].world == command.world)
                    {
                        flag = true;
                    }
                }
            }

            // ���� ��ġ�ϸ� ��ȯ
            if (flag)
            {
                // ������ �����ʿ��� ����
                spawnerList[spawnList[spawnWave].spawner].Spawn(spawnList[spawnWave].type, spawnList[spawnWave].num);
                spawnWave++;
            }
        }
    }

    // ���޷� ���
    void ResourceSupplyCalc()
    {
        int gtemp = 0;
        int mtemp = 0;
        int ftemp = 0;

        for (int i = 0; i < pointList.Count; i++)
        {
            switch(pointList[i].resource)
            {
                case Resource.Gold:
                    gtemp += pointList[i].resourceAmount;
                    break;
                case Resource.Magic:
                    mtemp += pointList[i].resourceAmount;
                    break;
                case Resource.Food:
                    ftemp += pointList[i].resourceAmount;
                    break;
            }
        }

        goldSupply = gtemp;
        magicSupply = mtemp;
        foodSupply = ftemp;
    }


    // ������������
    //void SettingSpawnData()
    //{
    //    spawnTxt = Instantiate(Resources.Load("Datas/" + "SpawnData") as TextAsset);

    //    // ���Ϳ� ������ �迭�� ũ�� ����
    //    string currentText = spawnTxt.text.Substring(0, spawnTxt.text.Length - 1);  // ������ ���� ����� �ֱ�
    //    string[] line = currentText.Split('\n');
    //    lineSize = line.Length;
    //    rowSize = line[0].Split('\t').Length;
    //    spawnData = new string[lineSize, rowSize];

    //    // �迭�� �ֱ�
    //    for(int i = 0; i < lineSize; i++)
    //    {
    //        // �� �ٿ��� ������ ������
    //        string[] row = line[i].Split('\t');
    //        for(int j = 0; j < row.Length; j++)
    //        {
    //            spawnData[i, j] = row[j];
    //        }
    //    }
    //}
}
