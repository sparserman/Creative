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

    public bool firstLobby = true;  // 첫 게임 로비 진입 확인용


    // 자원 관련
    public int gold = 100;      // 기본 자원 3가지    ( 기본 병사 강화[수류탄 장착], 건물 강화 및 배치 )
    public int magic = 0;       // 특수 자원         ( 특수 병사 소환 및 특수 병사 강화 )
    public int food = 0;        // 지속 소비 자원     ( 인구 수 유지, 관리율 관련 )

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
    TextAsset spawnTxt;
    string[,] spawnData;
    int lineSize, rowSize;
    int spawnWave;         // 스폰 웨이브

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
                // 현재 월드와 시간 확인
                if (spawnData[spawnWave, 0] == command.world
                    && spawnData[spawnWave, 1] == gm.day.ToString()
                    && spawnData[spawnWave, 2] == gm.hour.ToString()
                    && spawnData[spawnWave, 3] == gm.minute.ToString())
                {
                    // 생성
                    spawnerList[int.Parse(spawnData[spawnWave, 6])].Spawn(spawnData[spawnWave, 4], int.Parse(spawnData[spawnWave, 5]));
                    spawnWave++;

                }
            }
        }
    }

    // 스폰정보세팅
    void SettingSpawnData()
    {
        spawnTxt = Instantiate(Resources.Load("Datas/" + "SpawnData") as TextAsset);

        // 엔터와 탭으로 배열의 크기 조정
        string currentText = spawnTxt.text.Substring(0, spawnTxt.text.Length - 1);  // 마지막 엔터 지우고 넣기
        string[] line = currentText.Split('\n');
        lineSize = line.Length;
        rowSize = line[0].Split('\t').Length;
        spawnData = new string[lineSize, rowSize];

        // 배열에 넣기
        for(int i = 0; i < lineSize; i++)
        {
            // 한 줄에서 탭으로 나누기
            string[] row = line[i].Split('\t');
            for(int j = 0; j < row.Length; j++)
            {
                spawnData[i, j] = row[j];
            }
        }
    }
}
