using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public int eternium = 0;    // 기본 자원    ( 유닛 강화 및 건물 강화, 배치 )
    public int MP = 0;  // 매니저 포인트  (ex. 자원 수급량 증가, 특정 매니저 강화)
    
    public int CP = 0;  // 특성 포인트   (ex. 수류탄 장착, 부대수 증가, 바리게이트 포탑 등)

    public int coverNum = 2; // 바리케이드 수용 인원 수
    public float spawnTime = 10f;    // 소환 대기 시간
    public float soldier1Hp = 0.0f;   // 솔져1 hp % up
    public float soldier1Mp = 0.0f;   // 솔져1 mp % up
    public float soldier1Ad = 0.0f;   // 솔져1 ad % up



    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
