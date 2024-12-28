using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public enum Team
{
    Blue = 0, Red
}
public enum E_State
{
    Move = 0,
    Attack,
    TempMove,
    Defense,
    Retreat,
    Fixed,
    Building,
    Player
}

[System.Serializable]
public class Stat
{
    public Team team;
    public E_State state;

    public int level = 1;

    public float maxHp = 10;
    public float hp = 10;
    public float maxMp = 10;
    public float mp = 0;

    public float shield = 0;

    public float ad = 1;    // attack damage
    public float attackSpeed = 1;
    public float attackRange = 5;
    public float shotSpeed = 0.1f;

    public float moveSpeed = 0.01f;
    public float runSpeed = 1.5f;  // Run으로 빨라질 값 (추가 %)
    public float runValue;  // Run으로 빨라진 값 (추가 %)

    public float jumpPower = 250;
    public bool downJump;
}

public class Enemy : MonoBehaviour
{
    GameManager gm;

    // 정보
    public MobInfo mobInfo;

    public List<GameObject> shotPos;

    // 공격자세중 늘어나는 사거리
    float plusRange = 1;    // 전투 시 늘어난 사거리
    float moveRange = 1.2f;  // 이동할 사거리
    float coverRange = 1.2f; // 엄폐 시 사거리
    float skillRange = 0;    // 스킬 사거리 증가량

    public float curRange = 1;      // 현재 공격 사거리

    public float curMoveSpeed = 1;      // 현재 이동속도

    // Bar
    public Image hpBar;
    public Image hpBack;
    public Image mpBar;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Collider2D col;

    // 타겟 관련
    public GameObject target;
    Vector2 destination;

    // 바닥 레이어마스크
    public LayerMask ground;

    // 죽었는 지
    bool die;

    // 커맨드
    GameObject command;

    // 방향 (true : 오른쪽)
    public bool dir;

    // 현재 엄폐물
    public Enemy cover;
    public List<Enemy> coverList;       // 엄폐중인 유닛
    public int coverNum;                // 엄폐가능인원
    public GameObject buildPoint;       // 빌드 포인트
    public GameObject coverShotObj;     // 건물의 포탑


    // 상태이상
    public float fire;          // 점화 지속시간
    public float fireDamage;    // 점화 데미지

    public float smoke;         // 연막 지속시간
    public float smokeRange = 1;    // 연막으로 인한 사거리 감소 (%로 변경하는 효과)
    public float smokeSpeed = 1;    // 연막으로 인한 이동속도 감소 (%로 변경하는 효과)


    // 드래그 중
    public bool spawnWaiting = false;   // 소환 대기 상태 (특수 병사)

    void Start()
    {
        gm = GameManager.GetInstance();

        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();


        command = GameObject.Find("Command");

        if (!spawnWaiting)
        {
            gm.mobList.Add(gameObject);
        }

        Init();

        // 체력 세팅
        if (hpBar != null)
        {
            HpSetting();
        }

        // 바리케이드라면
        if (mobInfo.stat.state == E_State.Fixed)
        {
            // 기본 병사들의 위치 재배치
            EnemyPositionChange();
        }

        // 사거리 편차
        mobInfo.stat.attackRange += Random.Range(0.21f, -0.21f);
    }


    void FixedUpdate()
    {
        if(!gm.timerOn)
        {
            anim.speed = 0;
            return;
        }
        anim.speed = 1;

        WaitingSystem();

        if (!spawnWaiting)
        {
            if (!die)
            {
                DieMotion();            // 죽는 모션
                DirectionCheck();       // 방향 체크

                // 플레이어가 아니면 적 탐지
                if (mobInfo.stat.state != E_State.Player)
                {
                    // 사거리 지속 변경
                    curRange = (mobInfo.stat.attackRange * plusRange + skillRange) * smokeRange;

                    // 이동속도 지속 변경
                    curMoveSpeed = (1 * mobInfo.stat.moveSpeed * mobInfo.stat.runValue) * smokeSpeed;

                    Sense();
                    SpecialSkill();

                    AttackSpeedUpdate();    // 공격 속도 적용
                    StateAction();          // 상태에 따라 행동
                }
                Debuff();               // 상태이상 효과적용

                if (hpBar != null)
                {
                    HpUpdate();         // 체력 이미지 변경
                }
                MpRegen();
            }
        }
    }

    void WaitingSystem()
    {
        if(spawnWaiting)
        {
            if (spriteRenderer && rigid)
            {
                Color c = spriteRenderer.color;
                spriteRenderer.color = new Color(c.r, c.g, c.b, 0.4f);
                rigid.gravityScale = 0;
            }
        }
        else if (!spawnWaiting)
        {
            if (spriteRenderer && rigid)
            {
                Color c = spriteRenderer.color;
                spriteRenderer.color = new Color(c.r, c.g, c.b, 1f);
                rigid.gravityScale = 2;
            }
        }
    }

    void StateAction()
    {
        switch (mobInfo.stat.state)
        {
            case E_State.Move:
                MoveTo();
                break;
            case E_State.Attack:
                MoveTo();
                break;
            case E_State.TempMove:
                ReturnDefense();
                MoveTo();
                break;
            case E_State.Defense:
                CoverSystem();
                break;
            case E_State.Retreat:
                CoverSystem();
                break;
            case E_State.Fixed:
                
                break;
            case E_State.Building:
                TowerAttack();
                break;
            case E_State.Player:

                break;
        }

    }

    void EnemyPositionChange()
    {
        for(int i = 0; i < gm.mobList.Count; i++)
        {
            if (gm.mobList[i].GetComponent<Enemy>() != null)
            {
                Enemy temp = gm.mobList[i].GetComponent<Enemy>();
                temp.cover = null;
                temp.coverList.Clear();
            }
        }
    }

    void ReturnDefense()
    {
        for(int i = 0;i < gm.mobList.Count; i++)
        {
            if(gm.mobList[i].GetComponent<Enemy>() != null)
            {
                if(gm.mobList[i].GetComponent<Enemy>().mobInfo.stat.state == E_State.Fixed)
                {
                    // 방향 지정
                    DirectionCheck();
                    mobInfo.stat.state = E_State.Defense;
                    break;
                }
            }

        }
    }

    void HpSetting()
    {
        if(mobInfo.stat.team == Team.Blue)
        {
            hpBar.color = new Color32(100, 255, 240, 255);
        }
        else
        {
            hpBar.color = new Color32(255, 100, 100, 255);
        }
    }
    
    void HpUpdate()
    {
        // 체력바 표현
        if ((mobInfo.stat.hp + mobInfo.stat.shield) / mobInfo.stat.maxHp > 1)
        {
            hpBar.fillAmount = (mobInfo.stat.hp + (mobInfo.stat.maxHp - (mobInfo.stat.hp + mobInfo.stat.shield))) / mobInfo.stat.maxHp;
        }
        else
        {
            hpBar.fillAmount = mobInfo.stat.hp / mobInfo.stat.maxHp;
        }


        // hpBarBack의 색깔수정
        if (mobInfo.stat.shield > 0)
        {
            hpBack.GetComponent<Image>().color = Color.white;
        }
        else
        {
            if (mobInfo.stat.team == Team.Red)
            {
                hpBack.color = Color.yellow;
            }
            else if (mobInfo.stat.team == Team.Blue)
            {
                hpBack.color = Color.red;
            }
        }

        // 감소량 표현
        if (mobInfo.stat.shield > 0)
        {
            hpBack.fillAmount = (mobInfo.stat.hp + mobInfo.stat.shield) / mobInfo.stat.maxHp;
        }
        else
        {
            if (hpBar.fillAmount < hpBack.fillAmount)
            {
                hpBack.fillAmount -= 0.005f;
            }
            else
            {
                hpBack.fillAmount = hpBar.fillAmount;
            }
        }

        // MP 표현
        if (mpBar != null)
        {
            mpBar.fillAmount = mobInfo.stat.mp / mobInfo.stat.maxMp;
        }
    }

    // 달리기 제어
    void RunControl()
    {
        // 달리기 제어 (일단 연구소 만들기전까진 걷기 기본)
        if (true)
        {
            mobInfo.stat.runValue = 1;
            anim.SetBool("isRun", false);
        }
        //else
        //{
        //    mobInfo.stat.runValue = mobInfo.stat.runSpeed;
        //    anim.SetBool("isRun", true);
        //}
    }

    void MoveTo()
    {
        anim.SetBool("isShot", false);

        if(target == null)
        {
            anim.SetBool("isMove", false);
            return;
        }

        destination = target.transform.position;

        if(destination.x >= transform.position.x)
        {
            dir = true;
        }
        else
        {
            dir = false; 
        }

        if (cover != null)
        {
            cover.coverList.Remove(this);
            cover = null;
        }

        RunControl();

        // 목표 이동
        TargetAttack(curRange, curRange, moveRange);

        JumpSystem();
        
    }

    // 목표 이동
    void TargetAttack(float p_right, float p_left, float p_range)
    {
        // 방향 전환
        if (destination.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        // 타겟이 null이 아닐때만
        if (target != null)
        {
            Vector2 targetPos = target.transform.position;
            // 타겟이 사거리내에 들어왔고 목적지가 타겟방향일때, 공속 체크도 함
            if //(Vector2.Distance(transform.position, targetPos) > mobInfo.stat.attackRange * plusRange && targetPos.x > transform.position.x)
            (transform.position.x + curRange >= targetPos.x && transform.position.x - curRange <= targetPos.x)
            {
                // 모션
                if (transform.position.x <= destination.x && dir)
                {
                    anim.SetBool("isShot", true);
                }
                else if (transform.position.x >= destination.x && !dir)
                {
                    anim.SetBool("isShot", true);
                }
                else
                {
                    anim.SetBool("isShot", false);
                    DestinationMove(p_right, p_left, p_range);
                }
            }
            // 목적지로 이동
            else
            {
                anim.SetBool("isShot", false);
                DestinationMove(p_right, p_left, p_range);
            }
        }
        // 목적지로 이동
        else
        {
            anim.SetBool("isShot", false);
            DestinationMove(p_right, p_left, p_range);
        }
    }

    void DestinationMove(float p_right, float p_left, float p_range)
    {
        Vector2 temppos;
        // 목적지로 이동
        if //(Vector2.Distance(transform.position, destination) > p_dis && destination.x > transform.position.x)
            (transform.position.x + p_right <= destination.x)
        {
            plusRange = 1;

            // 모션
            anim.SetBool("isMove", true);
            anim.SetBool("isShot", false);

            // 오른쪽 이동
            temppos = transform.position;
            temppos.x += curMoveSpeed;
            transform.position = temppos;
        }
        else if //(Vector2.Distance(transform.position, destination) > p_dis && destination.x < transform.position.x)
            (transform.position.x - p_left >= destination.x)
        {
            plusRange = 1;

            // 모션
            anim.SetBool("isMove", true);
            anim.SetBool("isShot", false);

            // 왼쪽 이동
            temppos = transform.position;
            temppos.x -= curMoveSpeed;
            transform.position = temppos;
        }
        // 목적지에 도착한 상태 (대기)
        else
        {
            if (dir)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
            anim.SetBool("isMove", false);
            anim.SetBool("isRun", false);

            // 디펜스면 사거리 미리 증가
            if (mobInfo.stat.state == E_State.Defense)
            {
                plusRange = p_range;
            }
        }
    }

    void JumpSystem()
    {
        // 땅 관통
        if (rigid.velocity.y > 0f)
        {
            col.enabled = false;
        }
        else
        {
            if (!mobInfo.stat.downJump)
            {
                col.enabled = true;

                // 착지
                if (Physics2D.BoxCast(transform.position, new Vector2(0.21f, 0.1f), 0, -transform.up, 0.6f, ground))
                {
                    anim.SetBool("isJump", false);
                }
            }
        }

        // 목적지가 위에 있는 지 체크
        if (destination.y > transform.position.y + 1.5f && !anim.GetBool("isJump"))
        {
            if (target != null)
            {
                // 타겟이 점프 상태인지
                if (target.GetComponent<Animator>() != null)
                {
                    if (!target.GetComponent<Animator>().GetBool("isJump"))
                    {
                        // 점프
                        Debug.DrawRay(transform.position, transform.up * 1.5f);
                        if (Physics2D.Raycast(transform.position, transform.up, 1.5f, ground))
                        {
                            rigid.AddForce(transform.up * mobInfo.stat.jumpPower);
                            anim.SetBool("isJump", true);
                        }
                    }
                }
            }
            else
            {
                // 점프
                Debug.DrawRay(transform.position, transform.up * 1.5f);
                if (Physics2D.Raycast(transform.position, transform.up, 1.5f, ground))
                {
                    rigid.AddForce(transform.up * mobInfo.stat.jumpPower);
                    anim.SetBool("isJump", true);
                }
            }
        }
        // 아래에 있는 지 체크
        else if (destination.y < transform.position.y - 1.5f && !anim.GetBool("isJump"))
        {
            if (target != null)
            {
                // 타겟이 점프 상태인지
                if (target.GetComponent<Animator>() != null)
                {
                    if (!target.GetComponent<Animator>().GetBool("isJump"))
                    {

                        mobInfo.stat.downJump = true;
                        col.enabled = false;
                        anim.SetBool("isJump", true);

                    }
                }
            }
            else
            {
                mobInfo.stat.downJump = true;
                col.enabled = false;
                anim.SetBool("isJump", true);
            }
        }
        else if (destination.y > transform.position.y - 0.6f)
        {
            mobInfo.stat.downJump = false;
        }
    }

    public void Shot()
    {
        if (target != null)
        {
            GameObject go = null;
            switch (mobInfo.nameText)
            {
                case "FireWizard":
                    {
                        // 불덩이
                        go = Instantiate(Resources.Load("Prefabs/Projectile/" + "FireBall") as GameObject);
                        Vector2 dir = target.transform.position - transform.position;
                        go.GetComponent<Bullet>().Init(mobInfo.stat.team, dir.normalized, mobInfo.stat.shotSpeed, mobInfo.stat.ad, target);
                    }
                    break;
                case "Tower":
                    {
                        // 큰 총알
                        go = Instantiate(Resources.Load("Prefabs/Projectile/" + "TowerBullet") as GameObject);
                        Vector2 dir = new Vector3(target.transform.position.x - transform.position.x, 0, 0);
                        go.GetComponent<Bullet>().Init(mobInfo.stat.team, dir.normalized, mobInfo.stat.shotSpeed, mobInfo.stat.ad, target);
                    }
                    break;
                case "Raider1":
                    {
                        // 샷건
                        for (int i = -2; i < 3; i++)
                        {
                            GameObject tempGo = Instantiate(Resources.Load("Prefabs/Projectile/" + "Bullet") as GameObject);
                            Vector2 dir = (target.transform.position + new Vector3(0, -0.18f * i, 0)) - transform.position;
                            tempGo.GetComponent<Bullet>().Init(mobInfo.stat.team, dir.normalized, mobInfo.stat.shotSpeed, mobInfo.stat.ad, target);

                            if (shotPos[0] != null && shotPos[1] != null)
                            {
                                if (!spriteRenderer.flipX)
                                {
                                    tempGo.transform.position = shotPos[0].transform.position;
                                }
                                else
                                {
                                    tempGo.transform.position = shotPos[1].transform.position;
                                }
                            }
                        }
                    }
                    break;
                case "Raider3":
                    {
                        // 그냥 데미지 주기
                        target.GetComponent<Enemy>().Damaged(mobInfo.stat.ad);
                    }
                    break;
                default:
                    {
                        // 일반 총알
                        go = Instantiate(Resources.Load("Prefabs/Projectile/" + "Bullet") as GameObject);
                        Vector2 dir = (target.transform.position + new Vector3(0, Random.Range(-0.07f, 0.07f), 0)) - transform.position;
                        go.GetComponent<Bullet>().Init(mobInfo.stat.team, dir.normalized, mobInfo.stat.shotSpeed, mobInfo.stat.ad, target);
                    }
                    break;
            }

            // 총알 오브젝트가 비어있으면 하지않음
            if (go != null)
            {
                if (shotPos[0] != null && shotPos[1] != null)
                {
                    if (!spriteRenderer.flipX)
                    {
                        go.transform.position = shotPos[0].transform.position;
                    }
                    else
                    {
                        go.transform.position = shotPos[1].transform.position;
                    }
                }
            }
        }
    }

    float attackTimer = 0;
    void AttackSpeedUpdate()
    {
        if (mobInfo.stat.state != E_State.Fixed && mobInfo.stat.state != E_State.Building && mobInfo.stat.state != E_State.Player)
        {
            float temp = 1f / mobInfo.stat.attackSpeed;

            if(temp >= 2.5f)
            {
                temp = 2.5f;
            }
            else if(temp <= 0.7f)
            {
                temp = 0.7f;
            }

            anim.SetFloat("AttackSpeed", temp);

            // 공격 상태인데 타이머가 음수라면
            if (anim.GetBool("isShot") && attackTimer < 0)
            {
                // 0으로 변경해서 타이머 작동
                attackTimer = 0;
            }

            // 타이머가 0보다 크고 공속보다 작아야 시간이 지나감
            if (attackTimer <= mobInfo.stat.attackSpeed && attackTimer >= 0)
            {
                attackTimer += Time.deltaTime;
            }
            // 공격 속도에 따라 트리거 켜주기
            if(attackTimer >= mobInfo.stat.attackSpeed)
            {
                anim.SetTrigger("isAttack");
                attackTimer = -1;
            }
        }
    }

    // 공격 타이머 리셋
    public void AttackTimerReset()
    {
        attackTimer = 0;
    }

    void Sense()
    {
        if (gm.mobList != null)
        {
            if (target != null)
            {
                // 현재 타겟이 리스트에 있는 지 체크
                bool flag = false;
                for (int i = 0; i < gm.mobList.Count; i++)
                {
                    if(gm.mobList[i] == target)
                    {
                        flag = true;
                    }
                }
                if(!flag)
                {
                    target = null;

                    anim.SetBool("isShot", false);
                    if (mobInfo.stat.team == Team.Red)
                    {
                        target = command;
                    }
                    else
                    {
                        for (int i = 0; i < gm.mobList.Count; i++)
                        {
                            if (gm.mobList[i].GetComponent<Enemy>().mobInfo.stat.team != mobInfo.stat.team)
                            {
                                target = gm.mobList[i];
                            }
                        }
                    }
                    return;
                }

                for (int i = 0; i < gm.mobList.Count; i++)
                {
                    if (Vector2.Distance(transform.position, gm.mobList[i].transform.position) <= Vector2.Distance(transform.position, target.transform.position))
                    {
                        if (gm.mobList[i].GetComponent<Enemy>().mobInfo.stat.team != mobInfo.stat.team)
                        {
                            target = gm.mobList[i];
                        }
                    }
                }

            }
            else
            {
                anim.SetBool("isShot", false);
                if (mobInfo.stat.team == Team.Red)
                {
                    target = command;
                }
                else
                {
                    for (int i = 0; i < gm.mobList.Count; i++)
                    {
                        if (gm.mobList[i].GetComponent<Enemy>().mobInfo.stat.team != mobInfo.stat.team)
                        {   
                            target = gm.mobList[i];
                        }
                    }
                }
            }
        }
    }

    void DirectionCheck()
    {
        // 커맨드 위치
        float cx = command.transform.position.x;

        // 커맨드 기준 방향 설정 (blue)
        if (mobInfo.stat.team == Team.Blue)
        {
            if (cx <= transform.position.x)
            {
                dir = true;
            }
            else
            {
                dir = false;
            }
        }
        else if(mobInfo.stat.team == Team.Red)
        {
            if (cx <= transform.position.x)
            {
                dir = false;
            }
            else
            {
                dir = true;
            }
        }
    }

    // 엄폐 순서
    public int count = 1;
    void CoverSystem()
    {
        RunControl();

        if (gm.mobList != null)
        {
            // 지정된 본인의 엄폐 건물이 없으면
            if (cover == null)
            {
                Enemy temp = null;
                for (int i = 0; i < gm.mobList.Count; i++)
                {
                    int balance = gm.rightMobNum - gm.leftMobNum;
                    if (gm.mobList[i].GetComponent<Enemy>())
                    {
                        Enemy e = gm.mobList[i].GetComponent<Enemy>();

                        // 건물일 때
                        if (e.GetComponent<Enemy>().mobInfo.stat.state == E_State.Fixed)
                        {
                            // 본인 팀이랑 같을 때
                            if(e.GetComponent<Enemy>().mobInfo.stat.team == mobInfo.stat.team)
                            {
                                // 최대 엄폐 가능 인원이 비었다면 본인의 엄폐로 추가
                                if (e.coverList.Count < e.coverNum)
                                {
                                    // 처음엔 무조건 입력
                                    if (temp == null)
                                    {
                                        temp = e;
                                    }
                                    // 가장 먼 엄폐물로 입력
                                    else
                                    {
                                        if (dir)
                                        {
                                            // 양쪽 아군의 균형이 맞지않으면 방향 전환 (반대에 빈자리가 있는 바리케이드가 있어야함)
                                            if (balance >= 2 && gm.leftBarricadeNum >= 1)
                                            {
                                                bool flag = true;
                                                Enemy enemy = null;
                                                // 가까운 아군이 가도록 코드를 추가해야함
                                                for(int j = 0; j < gm.mobList.Count; j++)
                                                {
                                                    if (gm.mobList[j].GetComponent<Enemy>() != null)
                                                    {
                                                        enemy = gm.mobList[j].GetComponent<Enemy>();
                                                        if(enemy.mobInfo.stat.team == mobInfo.stat.team && enemy.dir && enemy.mobInfo.stat.state == E_State.Defense)
                                                        {
                                                            if(enemy.transform.position.x < transform.position.x)
                                                            {
                                                                flag = false;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }

                                                // 본인이 가장 커맨드에 가까우면 변경
                                                if (flag)
                                                {
                                                    dir = false;
                                                    gm.rightMobNum--;
                                                    gm.leftMobNum++;
                                                }
                                            }
                                            else if (temp.transform.position.x < e.transform.position.x)
                                            {
                                                temp = e;
                                            }
                                        }
                                        else
                                        {

                                            // 방향 전환
                                            if (balance <= -2 && gm.rightBarricadeNum >= 1)
                                            {
                                                bool flag = true;
                                                Enemy enemy = null;
                                                // 가까운 아군이 가도록 코드를 추가해야함
                                                for (int j = 0; j < gm.mobList.Count; j++)
                                                {
                                                    if (gm.mobList[j].GetComponent<Enemy>() != null)
                                                    {
                                                        enemy = gm.mobList[j].GetComponent<Enemy>();
                                                        if (enemy.mobInfo.stat.team == mobInfo.stat.team && !enemy.dir && enemy.mobInfo.stat.state == E_State.Defense)
                                                        {
                                                            if (enemy.transform.position.x > transform.position.x)
                                                            {
                                                                flag = false;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }

                                                if (flag)
                                                {
                                                    dir = true;
                                                    gm.rightMobNum++;
                                                    gm.leftMobNum--;
                                                }
                                            }

                                            if (temp.transform.position.x > e.transform.position.x)
                                            {
                                                temp = e;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }

                // 지정된 엄폐물이 있으면
                if (temp != null)
                {
                    // 추가
                    temp.coverList.Add(this);
                    cover = temp;
                    destination = cover.transform.position;
                    count = cover.coverList.Count;
                }
                else
                {
                    mobInfo.stat.state = E_State.TempMove;
                }
            }
            else
            {
                // 현재 엄폐물이 리스트에 있는 지 체크
                bool flag = false;
                for (int i = 0; i < gm.mobList.Count; i++)
                {
                    if (gm.mobList[i].GetComponent<Enemy>().mobInfo.stat.team == mobInfo.stat.team)
                    {
                        if (gm.mobList[i] == cover.gameObject)
                        {
                            flag = true;
                        }
                    }
                }
                if (!flag)
                {
                    cover = null;
                    return;
                }

                destination = cover.transform.position;
                
                if (dir)
                {
                    destination.x = cover.transform.position.x - 0.3f * count;
                    TargetAttack(0.1f, 0, coverRange + count * 0.1f);
                    JumpSystem();
                }
                else
                {
                    destination.x = cover.transform.position.x + 0.3f * count;
                    TargetAttack(0, 0.1f, coverRange + count * 0.1f);
                    JumpSystem();
                }
            }
        }

    }

    float shotTimer = 0;
    void TowerAttack()
    {
        if (target != null)
        {
            Vector2 targetPos = target.transform.position;
            if (transform.position.x + mobInfo.stat.attackRange >= targetPos.x && transform.position.x - mobInfo.stat.attackRange <= targetPos.x)
            {
                shotTimer += Time.deltaTime;

                // 공속 확인
                if (shotTimer >= mobInfo.stat.attackSpeed)
                {
                    // 모션
                    Shot();
                    shotTimer = 0;
                }
            }
            else
            {
                shotTimer = 0;
            }
        }
    }

    void Init()
    {
        // 방향 지정
        DirectionCheck();

        float hp = 0;
        float mp = 0;
        float ad = 0;
        switch (mobInfo.nameText)
        {
            case "Soldier1":
                hp = gm.gi.soldier1Hp;
                mp = gm.gi.soldier1Mp;
                ad = gm.gi.soldier1Ad;
                break;
        }

        // 정보 입력
        mobInfo.stat.maxHp += mobInfo.stat.hp * hp;
        mobInfo.stat.hp += mobInfo.stat.hp * hp;
        mobInfo.stat.maxMp += mobInfo.stat.mp * mp;
        mobInfo.stat.mp += mobInfo.stat.mp * mp;
        mobInfo.stat.ad += mobInfo.stat.ad * ad;

        if (mobInfo.stat.team == Team.Blue)
        {
            GameInfoMobUpdate();
        }
        

    }

    public void GameInfoMobUpdate()
    {
        // 오른쪽 인원 수, 왼쪽 인원 수, 빈 오 왼 바리케이드 수
        int r = 0, l = 0, rb = 0, lb = 0;
        for (int i = 0; i < gm.mobList.Count; i++)
        {
            if (gm.mobList[i].GetComponent<Enemy>() != null)
            {
                Enemy temp = gm.mobList[i].GetComponent<Enemy>();
                if (temp.mobInfo.stat.team == Team.Blue && temp.dir)
                {
                    if (temp.mobInfo.stat.state == E_State.Defense)
                    {
                        r++;
                    }
                    else if (temp.mobInfo.stat.state == E_State.Fixed)
                    {
                        if (temp.coverList.Count < gm.gi.coverNum)
                        {
                            rb++;
                        }
                    }
                }
                else if (temp.mobInfo.stat.team == Team.Blue && !temp.dir)
                {
                    if (temp.mobInfo.stat.state == E_State.Defense)
                    {
                        l++;
                    }
                    else if (temp.mobInfo.stat.state == E_State.Fixed)
                    {
                        if (temp.coverList.Count < gm.gi.coverNum)
                        {
                            lb++;
                        }
                    }
                }
            }
        }
        // 게임 정보에 넣기
        gm.rightMobNum = r;
        gm.leftMobNum = l;
        gm.rightBarricadeNum = rb;
        gm.leftBarricadeNum = lb;
    }

    // 기본 병사 소환용
    public void EnemySpawn(string p_name)
    {
        mobInfo.nameText = p_name;
        mobInfo.stat.state = E_State.Defense;
    }

    // 특수 병사 소환용
    public void EnemySpawn(MobInfo p_mobInfo)
    {
        mobInfo.nameText = p_mobInfo.nameText;
        spawnWaiting = true;
        mobInfo.stat = p_mobInfo.stat;
    }

    void Debuff()
    {
        // 점화
        if (fire > 0)
        {
            float damage = fireDamage;
            Damaged(damage);

            fire -= Time.deltaTime;
        }
    }

    void Die()
    {
        if (mobInfo.stat.hp <= 0)
        {
            if (gm.mobList != null)
            {
                gm.mobList.Remove(gameObject);
            }

            // 바리케이드라면
            if (mobInfo.stat.state == E_State.Fixed)
            {
                // 빌드 포인트 재생성 코루틴
                if (m_BulidPointRespawnCrt != null)
                {
                    StopCoroutine(m_BulidPointRespawnCrt);
                    m_BulidPointRespawnCrt = null;
                }
                m_BulidPointRespawnCrt = StartCoroutine(BuildPointRespawnCrt());
            }
            else if (mobInfo.stat.state == E_State.Player)
            {
                // 플레이어 부활 코루틴
                if (m_PlayerRespawnCrt != null)
                {
                    StopCoroutine(m_PlayerRespawnCrt);
                    m_PlayerRespawnCrt = null;
                }
                m_PlayerRespawnCrt = StartCoroutine(PlayerRespawnCrt());
            }
            else
            {
                Destroy(gameObject, 10);
            }

            
        }
    }

    Coroutine m_BulidPointRespawnCrt = null;
    IEnumerator BuildPointRespawnCrt()
    {
        float t = 0;
        while (true)
        {
            t += Time.deltaTime;
            if (t >= 10f)
            {
                // 빌드 포인트 생성
                buildPoint.SetActive(true);
                Destroy(gameObject);
                break;
            }
            yield return null;
        }
    }

    Coroutine m_PlayerRespawnCrt = null;
    IEnumerator PlayerRespawnCrt()
    {
        float t = 0;
        while (true)
        {
            gameObject.GetComponent<Player>().respawnTimer.text = Mathf.Round(10 + gm.gi.respawnTime - t).ToString();

            t += Time.deltaTime;
            if (t >= 10f + gm.gi.respawnTime)
            {
                // 플레이어 부활
                gm.player.die = false;
                gm.player.freeze = false;
                hpBar.transform.parent.gameObject.SetActive(true);
                mobInfo.stat.hp = mobInfo.stat.maxHp;
                anim.Play("Player_Idle_Anim");
                gameObject.transform.position = command.transform.position + new Vector3(0, -0.6f, 0);
                die = false;
                col.enabled = true;
                rigid.gravityScale = 2;
                gm.player.respawnTimer.transform.parent.gameObject.SetActive(false);
                gm.player.cMode = false;
                gm.mobList.Add(gameObject);

                break;
            }
            yield return null;
        }
    }


    void DieMotion()
    {
        if (mobInfo.stat.hp <= 0 && !die)
        {
            if(mobInfo.stat.state == E_State.Player)
            {
                GetComponent<Player>().freeze = true;
                gm.player.respawnTimer.transform.parent.gameObject.SetActive(true);
                gm.player.cMode = true;
                gm.player.respawnTimer.text = "";
                gm.player.die = true;
            }

            if (mobInfo.stat.team == Team.Blue)
            {
                GameInfoMobUpdate();
            }

            // 건물용
            anim.enabled = true;

            die = true;
            //col.enabled = false;

            if(rigid != null)
            {
                rigid.gravityScale = 0;
            }

            if(mobInfo.stat.state != E_State.Fixed && mobInfo.stat.state != E_State.Building)
            {
                anim.SetTrigger("isDie");
            }
           

            // hp가 없는게 아니라면
            if (hpBar != null)
            {
                hpBar.GetComponentInParent<Animator>().SetTrigger("Off");
                
            }
            // mp가 없는게 아니라면
            if (mpBar != null)
            {
                mpBar.GetComponentInParent<Animator>().SetTrigger("Off");
            }

            if (gm.mobList != null)
            {
                gm.mobList.Remove(gameObject);
                if(cover != null)
                {
                    // 바리케이드 순서 정렬
                    if (cover.coverList.Count > 1)
                    {
                        cover.coverList.Remove(this);
                        for(int i = 0; i < cover.coverList.Count; i++)
                        {
                            if(cover.coverList[i].count > count)
                            {
                                cover.coverList[i].count--;
                            }
                        }
                    }
                }
            }
        }
    }

    // 
    void MpRegen()
    {
        if (mpBar != null)
        {
            // 연구소로 스킬 사용가능상태 만들어야함
            if (!anim.GetBool("isSkill"))
            {
                mobInfo.stat.mp += Time.deltaTime;
            }
        }
    }

    void SpecialSkill()
    {
        // 연구소로 스킬 사용가능상태 만들어야함
        if(mobInfo.stat.mp >= mobInfo.stat.maxMp)
        {
            anim.SetBool("isSkill", true);
            skillRange = 0.4f;
            mobInfo.stat.mp = mobInfo.stat.maxMp;
        }
    }

    public void UseSkill()
    {
        if (target != null)
        {
            GameObject go = null;
            switch (mobInfo.nameText)
            {
                case "Soldier1":
                    {

                    }
                    break;
                case "Gangster1":
                    {
                        
                    }
                    break;
                case "FireWizard":
                    {
                        go = Instantiate(Resources.Load("Prefabs/Projectile/" + "FireBall") as GameObject);
                        Vector2 dir = target.transform.position - transform.position;
                        go.GetComponent<Bullet>().Init(mobInfo.stat.team, dir.normalized, mobInfo.stat.shotSpeed, mobInfo.stat.ad * 1.5f, target);
                        go.transform.localScale = new Vector3(3, 3, 1);
                    }
                    break;
            }

            if (shotPos[0] != null && shotPos[1] != null && go != null)
            {
                if (!spriteRenderer.flipX)
                {
                    go.transform.position = shotPos[0].transform.position;
                }
                else
                {
                    go.transform.position = shotPos[1].transform.position;
                }
            }

            mobInfo.stat.mp = 0;
            anim.SetBool("isSkill", false);
            skillRange = 0f;
        }
    }

    public void Damaged(float damage)
    {
        if (mobInfo.stat.shield > 0)
        {
            if (mobInfo.stat.shield >= damage)
            {
                mobInfo.stat.shield -= damage;
                damage = 0;
            }
            else
            {
                damage -= mobInfo.stat.shield;
                mobInfo.stat.shield = 0;
            }
        }

        mobInfo.stat.hp -= damage;
    }
}
