using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager GetInstance() { Init(); return instance; }

    public List<GameObject> goList = new List<GameObject>();
    public SoundManager sm;
    public List<GameObject> mobList = new List<GameObject>();

    float second = 0;

    public LoadingSceneManager lm;

    public bool timerOn = false;

    public Player player;
    // �÷��̾� ���� ����
    public float maxhp;
    public float hp;
    public float maxmp;
    public float mp;

    public float ad;
    public float attackSpeed;

    public float moveSpeed;
    public float runSpeed;

    public float jumpPower;

    // ���� ����
    public GameInfo gi;

    // ���� ���� ����
    public Command command;
    public List<EnemySpawner> spawnerList;
    public int rightMobNum;    // ������ �� ����
    public int leftMobNum;     // ���� �� ����
    public int rightBarricadeNum;   // ������ �ٸ����̵� ����
    public int leftBarricadeNum;    // ���� �ٸ����̵� ����

    // ���� ����
    public SpawnDB spawnDB;
    public List<SpawnData> spawnList = new List<SpawnData>();
    public SpawnDBType typeDB;
    public int branch;          // ���� �б�
    public int spawnWave;       // ���� ���̺�

    void Awake()
    {
        Init();
    }

    private void Start()
    {
        if (GetComponent<SoundManager>() == null)
        {
            sm = gameObject.AddComponent<SoundManager>();
        }
        else
        {
            sm = GetComponent<SoundManager>();
        }

        SpawnListSetting();
    }

    void Update()
    {
        InputSystem();

        // �ð�
        SetDate();

        // ���� Ÿ�̸�
        SpawnTimer();
        // �ڿ� ���޷� ���
        ResourceSupplyCalc();
    }

    void InputSystem()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            MenuOff();
        }
    }

    static void Init()
    {
        if (instance == null)
        {
            //GameManager�� �����ϴ��� Ȯ��
            GameObject go = GameObject.Find("GameManager");
            //������ ����
            if (go == null)
            {
                go = new GameObject { name = "GameManager" };
            }
            if (go.GetComponent<GameManager>() == null)
            {
                go.AddComponent<GameManager>();
            }
            //�������� �ʵ��� ����

            // �̰� ����� �� ��ȯ�� ��Ȱ����
            // (�� �ڵ带 Ȱ��ȭ �� ������ ���� �� �� �ʱ�ȭ�� �ȵǴ� ������ �־���)
            DontDestroyOnLoad(go);

            //instance �Ҵ�
            instance = go.GetComponent<GameManager>();
        }
    }

    public void MenuOff(bool p_flag = true)
    {
        if (goList.Count > 0)
        {
            goList.LastOrDefault().GetComponent<Animator>().SetTrigger("Off");
            goList.Remove(goList.LastOrDefault());
            if (sm != null && p_flag)
            {
                sm.PlayEffectSound(sm.click);
            }
        }
    }

    void SetDate()
    {
        if (timerOn)
        {
            second += Time.deltaTime;
            if (second >= 1)
            {
                gi.minute++;
                second = 0;
                if (gi.minute >= 60)
                {
                    // 
                    OneMinutePlay();

                    gi.hour++;
                    gi.minute = 0;
                    if (gi.hour >= 24)
                    {
                        gi.day++;
                        gi.hour = 0;
                    }
                }
            }

        }
    }

    public void OneMinutePlay()
    {
        // �ڿ� ����
        for (int i = 0; i < gi.pointList.Count; i++)
        {
            gi.pointList[i].ResourceCollection();
        }

        //
    }

    public void WorldInfoUpdate()
    {
        for (int i = 0; i < goList.Count; i++)
        {
            if(goList[i].GetComponent<WorldInfo>() != null)
            {
                goList[i].GetComponent<WorldInfo>().ManagerUpdate();
            }
        }
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
            if (spawnList[spawnWave].day == gi.day && spawnList[spawnWave].hour == gi.hour && spawnList[spawnWave].minute == gi.minute)
            {
                // ���� ���� üũ
                if (command != null)
                {
                    if (spawnList[spawnWave].world == command.worldName)
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

        for (int i = 0; i < gi.pointList.Count; i++)
        {
            switch (gi.pointList[i].resource)
            {
                case Resource.Gold:
                    gtemp += gi.pointList[i].resourceAmount;
                    break;
                case Resource.Magic:
                    mtemp += gi.pointList[i].resourceAmount;
                    break;
                case Resource.Food:
                    ftemp += gi.pointList[i].resourceAmount;
                    break;
            }
        }

        gi.goldSupply = gtemp;
        gi.magicSupply = mtemp;
        gi.foodSupply = ftemp;
    }

    public void GameSave()
    {
        string jsonData = JsonUtility.ToJson(GameManager.GetInstance().gi);
        string path = Application.dataPath + "/gameInfo.json";
        File.WriteAllText(path, jsonData);
    }
}


