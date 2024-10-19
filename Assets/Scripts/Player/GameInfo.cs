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
    public bool firstLobby = true;  // 첫 게임 로비 진입 확인용

    // 시간
    public int day = 0;
    public int hour = 0;
    public int minute = 0;

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

    public List<PointInfo> pointList = new List<PointInfo>();                   // 지역 리스트

    [SerializeField]
    public List<MobInfo> specialMobList = new List<MobInfo>();             // 특수병사 리스트


    void InfoReset()
    {
        string path = Path.Combine(Application.dataPath, "InitInfo.json");
        string jsonData = File.ReadAllText(path);
        GameManager.GetInstance().gi = JsonUtility.FromJson<GameInfo>(jsonData);
    }
}
