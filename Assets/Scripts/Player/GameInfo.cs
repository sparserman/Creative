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

    public bool firstLobby = true;  // 첫 게임 로비 진입 확인용


    // 자원 관련
    public int gold = 100;      // 기본 자원 3가지    ( 기본 병사 강화[수류탄 장착], 건물 강화 및 배치 )
    public int magic = 100;       // 특수 자원         ( 특수 병사 소환 및 특수 병사 강화 )
    public int food = 100;        // 지속 소비 자원     ( 인구 수 유지, 관리율 관련 )

    public int goldSupply = 0;      // 시간 당 수급량
    public int magicSupply = 0;
    public int foodSupply = 0;

    public int MP = 0;  // 관리자 포인트  (ex. 관리자의 자원 수급량 증가, 관리자 강화)
    
    public int CP = 0;  // 특성 포인트   (ex. 플레이어 강화, 인구 수 증가, 바리게이트 포탑 등)

    // 특성 관련

    // 병사 최대 인구
    public int population = 4;

    // 바리케이드
    public int coverNum = 2; // 바리케이드 수용 인원 수

    // 모든 병사
    public float spawnTime = 2;    // 소환 대기 시간

    // 솔져1
    public float soldier1Hp = 0.0f;                 // Soldier1 hp % up
    public float soldier1Mp = 0.0f;                 // Soldier1 mp % up
    public float soldier1Ad = 0.0f;                 // Soldier1 ad % up
    public float soldier1AttackSpeed = 0.0f;        // Soldier1 attackSpeed % up

    // 불 마법사
    public float fireWizardHp = 0.0f;               // FireWizard hp % up
    public float fireWizardMp = 0.0f;               // FireWizard mp % up
    public float fireWizardAd = 0.0f;               // FireWizard ad % up
    public float fireWizardAttackSpeed = 0.0f;      // FireWizard attackSpeed % up
    public float fireTime = 3.0f;                   // 점화 지속 시간 (특성 강화시 생김)

    // 플레이어
    public float respawnTime = 0.0f;                // 부활 시간 감소량


    // 로비 관련
    public List<ManagerInfo> managerList = new List<ManagerInfo>();     // 매니저 리스트

    public List<Point> pointList = new List<Point>();                   // 지역 리스트

    [SerializeField]
    public List<MobInfo> specialMobList = new List<MobInfo>();             // 특수병사 리스트


    // 스폰 정보
    //TextAsset spawnTxt;
    //string[,] spawnData;
    //int lineSize, rowSize;
    //int spawnWave;         // 스폰 웨이브

    // 스폰 정보
    public SpawnDB spawnDB;
    public List<SpawnData> spawnList = new List<SpawnData>();
    public SpawnDBType typeDB;
    public int branch;          // 스폰 분기
    public int spawnWave;       // 스폰 웨이브

    // 현재 월드 정보
    public Command command;
    public List<EnemySpawner> spawnerList = new List<EnemySpawner>();
    public int rightMobNum;    // 오른쪽 몹 숫자
    public int leftMobNum;     // 왼쪽 몹 숫자
    public int rightBarricadeNum;   // 오른쪽 바리케이드 숫자
    public int leftBarricadeNum;    // 왼쪽 바리케이드 숫자

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
            if (spawnList[spawnWave].day == gm.day && spawnList[spawnWave].hour == gm.hour && spawnList[spawnWave].minute == gm.minute)
            {
                // 현재 월드 체크
                if (command != null)
                {
                    if (spawnList[spawnWave].world == command.world)
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


    // 스폰정보세팅
    //void SettingSpawnData()
    //{
    //    spawnTxt = Instantiate(Resources.Load("Datas/" + "SpawnData") as TextAsset);

    //    // 엔터와 탭으로 배열의 크기 조정
    //    string currentText = spawnTxt.text.Substring(0, spawnTxt.text.Length - 1);  // 마지막 엔터 지우고 넣기
    //    string[] line = currentText.Split('\n');
    //    lineSize = line.Length;
    //    rowSize = line[0].Split('\t').Length;
    //    spawnData = new string[lineSize, rowSize];

    //    // 배열에 넣기
    //    for(int i = 0; i < lineSize; i++)
    //    {
    //        // 한 줄에서 탭으로 나누기
    //        string[] row = line[i].Split('\t');
    //        for(int j = 0; j < row.Length; j++)
    //        {
    //            spawnData[i, j] = row[j];
    //        }
    //    }
    //}
}
