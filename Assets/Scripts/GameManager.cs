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
    // 플레이어 관련 스탯
    public float maxhp;
    public float hp;
    public float maxmp;
    public float mp;

    public float ad;
    public float attackSpeed;

    public float moveSpeed;
    public float runSpeed;

    public float jumpPower;

    // 게임 정보
    public GameInfo gi;

    // 현재 월드 정보
    public Command command;
    public List<EnemySpawner> spawnerList;
    public int rightMobNum;    // 오른쪽 몹 숫자
    public int leftMobNum;     // 왼쪽 몹 숫자
    public int rightBarricadeNum;   // 오른쪽 바리케이드 숫자
    public int leftBarricadeNum;    // 왼쪽 바리케이드 숫자

    // 스폰 정보
    public SpawnDB spawnDB;
    public List<SpawnData> spawnList = new List<SpawnData>();
    public SpawnDBType typeDB;
    public int branch;          // 스폰 분기
    public int spawnWave;       // 스폰 웨이브

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

        // 시간
        SetDate();

        // 스폰 타이머
        SpawnTimer();
        // 자원 수급량 계산
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
            //GameManager가 존재하는지 확인
            GameObject go = GameObject.Find("GameManager");
            //없으면 생성
            if (go == null)
            {
                go = new GameObject { name = "GameManager" };
            }
            if (go.GetComponent<GameManager>() == null)
            {
                go.AddComponent<GameManager>();
            }
            //없어지지 않도록 해줌

            // 이게 없어야 씬 전환이 원활해짐
            // (이 코드를 활성화 시 전투씬 입장 할 때 초기화가 안되는 오류가 있었음)
            DontDestroyOnLoad(go);

            //instance 할당
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
        // 자원 수집
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


    // 스폰 리스트에 입력
    void SpawnListSetting()
    {
        spawnDB = Instantiate(Resources.Load("Datas/" + "SpawnDB") as SpawnDB);

        List<SpawnDBEntity> listDB = null;
        // DB 정보 세팅
        switch (typeDB)
        {
            case SpawnDBType.Tutorial:
                listDB = spawnDB.Tutorial;
                break;
        }

        if (listDB != null)
        {
            // 리스트안을 전부 비우고
            spawnList.Clear();

            // DB 정보 입력
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
            // 현재 시간 체크
            if (spawnList[spawnWave].day == gi.day && spawnList[spawnWave].hour == gi.hour && spawnList[spawnWave].minute == gi.minute)
            {
                // 현재 월드 체크
                if (command != null)
                {
                    if (spawnList[spawnWave].world == command.worldName)
                    {
                        flag = true;
                    }
                }
            }

            // 전부 일치하면 소환
            if (flag)
            {
                // 지정한 스포너에서 생성
                spawnerList[spawnList[spawnWave].spawner].Spawn(spawnList[spawnWave].type, spawnList[spawnWave].num);
                spawnWave++;
            }
        }
    }

    // 수급량 계산
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


