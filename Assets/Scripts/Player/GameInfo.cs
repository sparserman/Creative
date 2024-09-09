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


public class GameInfo : MonoBehaviour
{
    GameManager gm;

    public bool firstLobby = true;  // ù ���� �κ� ���� Ȯ�ο�


    // �ڿ� ����
    public int eternium = 100;    // �⺻ �ڿ�    ( ���� ��ȭ �� �ǹ� ��ȭ, ��ġ )
    public int eterniumSupply = 0;      // �ð� �� ���޷�

    public int MP = 0;  // �Ŵ��� ����Ʈ  (ex. �ڿ� ���޷� ����, Ư�� �Ŵ��� ��ȭ)
    
    public int CP = 0;  // Ư�� ����Ʈ   (ex. ����ź ����, �δ�� ����, �ٸ�����Ʈ ��ž ��)

    // Ư�� ����
    public int coverNum = 2; // �ٸ����̵� ���� �ο� ��
    public int population = 1;  // �ٸ����̵�� �α�
    public float spawnTime = 2;    // ��ȯ ��� �ð�

    public float soldier1Hp = 0.0f;                 // Soldier1 hp % up
    public float soldier1Mp = 0.0f;                 // Soldier1 mp % up
    public float soldier1Ad = 0.0f;                 // Soldier1 ad % up
    public float soldier1AttackSpeed = 0.0f;        // Soldier1 attackSpeed % up

    public float fireWizardHp = 0.0f;               // FireWizard hp % up
    public float fireWizardMp = 0.0f;               // FireWizard mp % up
    public float fireWizardAd = 0.0f;               // FireWizard ad % up
    public float fireWizardAttackSpeed = 0.0f;      // FireWizard attackSpeed % up
    public float fireTime = 3.0f;                   // ��ȭ ���� �ð� (Ư�� ��ȭ�� ����)


    // �κ� ����
    public List<ManagerInfo> managerList = new List<ManagerInfo>();

    public List<Point> pointList = new List<Point>();


    // ���� A
    public bool worldADestroy = false;    // �ı� ����
    [SerializeField]
    public List<BarricadeInfo> barricadeAList = new List<BarricadeInfo>();   // �ٸ����̵� ����
    public int maxPopulationA = 0;      // �ִ� �α�
    public int curPopulationA = 0;      // ���� �α�


    // ���� ����
    TextAsset spawnTxt;
    string[,] spawnData;
    int lineSize, rowSize;
    int spawnWave;         // ���� ���̺�

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
        SettingSpawnData();
    }

    void Update()
    {
        SpawnTimer();
    }

    // 0. world, 1. day, 2. hour, 3. minute, 4. type, 5. num, 6. spawner
    void SpawnTimer()
    {
        if(command != null)
        {
            if (spawnerList.Count > spawnWave)
            {
                // ���� ����� �ð� Ȯ��
                if (spawnData[spawnWave, 0] == command.world
                    && spawnData[spawnWave, 1] == gm.day.ToString()
                    && spawnData[spawnWave, 2] == gm.hour.ToString()
                    && spawnData[spawnWave, 3] == gm.minute.ToString())
                {
                    // ����
                    spawnerList[int.Parse(spawnData[spawnWave, 6])].Spawn(spawnData[spawnWave, 4], int.Parse(spawnData[spawnWave, 5]));
                    spawnWave++;

                }
            }
        }
    }

    // ������������
    void SettingSpawnData()
    {
        spawnTxt = Instantiate(Resources.Load("Datas/" + "SpawnData") as TextAsset);

        // ���Ϳ� ������ �迭�� ũ�� ����
        string currentText = spawnTxt.text.Substring(0, spawnTxt.text.Length - 1);  // ������ ���� ����� �ֱ�
        string[] line = currentText.Split('\n');
        lineSize = line.Length;
        rowSize = line[0].Split('\t').Length;
        spawnData = new string[lineSize, rowSize];

        // �迭�� �ֱ�
        for(int i = 0; i < lineSize; i++)
        {
            // �� �ٿ��� ������ ������
            string[] row = line[i].Split('\t');
            for(int j = 0; j < row.Length; j++)
            {
                spawnData[i, j] = row[j];
            }
        }
    }
}
