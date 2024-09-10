using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
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

public class Enemy : MonoBehaviour
{
    GameManager gm;

    public List<GameObject> shotPos;

    // 공격자세중 늘어나는 사거리
    public float plusRange = 1;    // 현재 늘어난 사거리
    public float moveRange = 1.2f;
    public float coverRange = 1.2f;

    // 공격 자세

    public Image hpBar;
    public Image hpBack;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Collider2D col;
    public Stat stat;

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

    public EnemyType type;

    // 상태이상
    public float fire;          // 걸린 점화 지속시간
    public float fireDamage;    // 점화 데미지

    void Start()
    {
        gm = GameManager.GetInstance();

        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        stat = GetComponent<Stat>();


        command = GameObject.Find("Command");

        gm.mobList.Add(gameObject);

        Init();

        if (hpBar != null)
        {
            HpSetting();
        }


        if (stat.state == E_State.Fixed)
        {
            EnemyPositionChange();
        }
    }


    void FixedUpdate()
    {
        if (!die)
        {
            DieMotion();

            if (stat.state != E_State.Player)
            {
                Sense();
            }
            AttackSpeedUpdate();
            StateAction();

            Debuff();

            if (hpBar != null)
            {
                HpUpdate();
            }
        }
    }

    void StateAction()
    {
        switch (stat.state)
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
                if(gm.mobList[i].GetComponent<Enemy>().stat.state == E_State.Fixed)
                {
                    // 방향 지정
                    DirectionCheck();
                    stat.state = E_State.Defense;
                    break;
                }
            }

        }
    }

    void HpSetting()
    {
        if(stat.team == Team.Blue)
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
        if ((stat.hp + stat.shield) / stat.maxHp > 1)
        {
            hpBar.fillAmount = (stat.hp + (stat.maxHp - (stat.hp + stat.shield))) / stat.maxHp;
        }
        else
        {
            hpBar.fillAmount = stat.hp / stat.maxHp;
        }


        // hpBarBack의 색깔수정
        if (stat.shield > 0)
        {
            hpBack.GetComponent<Image>().color = Color.white;
        }
        else
        {
            if (stat.team == Team.Red)
            {
                hpBack.color = Color.yellow;
            }
            else if (stat.team == Team.Blue)
            {
                hpBack.color = Color.red;
            }
        }

        // 감소량 표현
        if (stat.shield > 0)
        {
            hpBack.fillAmount = (stat.hp + stat.shield) / stat.maxHp;
        }
        else
        {
            if (hpBar.fillAmount < hpBack.fillAmount)
            {
                hpBack.fillAmount -= 0.005f;
            }
            else
            {
                hpBack.fillAmount = hpBack.fillAmount;
            }
        }
    }

    // 달리기 제어
    void RunControl()
    {
        // 달리기 제어
        if (stat.state == E_State.Move)
        {
            stat.runValue = 0;
            anim.SetBool("isRun", false);
        }
        else if (stat.state == E_State.Attack || stat.state == E_State.Retreat)
        {
            stat.runValue = stat.runSpeed;
            anim.SetBool("isRun", true);
        }
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
        TargetAttack(stat.attackRange * plusRange, stat.attackRange * plusRange, moveRange);

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
            if //(Vector2.Distance(transform.position, targetPos) > stat.attackRange * plusRange && targetPos.x > transform.position.x)
            (transform.position.x + stat.attackRange * plusRange >= targetPos.x && transform.position.x - stat.attackRange * plusRange <= targetPos.x)
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
            temppos.x += 1 * (stat.moveSpeed * (1 + stat.runValue));
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
            temppos.x -= 1 * (stat.moveSpeed * (1 + stat.runValue));
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
            if (stat.state == E_State.Defense)
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
            if (!stat.downJump)
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
                            rigid.AddForce(transform.up * stat.jumpPower);
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
                    rigid.AddForce(transform.up * stat.jumpPower);
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

                        stat.downJump = true;
                        col.enabled = false;
                        anim.SetBool("isJump", true);

                    }
                }
            }
            else
            {
                stat.downJump = true;
                col.enabled = false;
                anim.SetBool("isJump", true);
            }
        }
        else if (destination.y > transform.position.y - 0.6f)
        {
            stat.downJump = false;
        }
    }

    public void Shot()
    {
        if (target != null)
        {
            GameObject go = null;
            switch (type)
            {
                case EnemyType.Soldier1:
                case EnemyType.Gangster1:
                    {
                        go = Instantiate(Resources.Load("Prefabs/" + "Bullet") as GameObject);
                        Vector2 dir = (target.transform.position + new Vector3(0, Random.Range(-0.07f, 0.07f), 0)) - transform.position;
                        go.GetComponent<Bullet>().Init(stat.team, dir.normalized, stat.shotSpeed, stat.ad, target);
                    }
                    break;
                case EnemyType.FireWizard:
                    {
                        go = Instantiate(Resources.Load("Prefabs/" + "FireBall") as GameObject);
                        Vector2 dir = target.transform.position - transform.position;
                        go.GetComponent<Bullet>().Init(stat.team, dir.normalized, stat.shotSpeed, stat.ad, target);
                    }
                    break;
                case EnemyType.Tower:
                    {
                        go = Instantiate(Resources.Load("Prefabs/" + "TowerBullet") as GameObject);
                        Vector2 dir = new Vector3(target.transform.position.x - transform.position.x, 0, 0);
                        go.GetComponent<Bullet>().Init(stat.team, dir.normalized, stat.shotSpeed, stat.ad, target);
                    }
                    break;
            }

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

    void AttackSpeedUpdate()
    {
        if (stat.state != E_State.Fixed && stat.state != E_State.Building)
        {
            anim.SetFloat("AttackSpeed", stat.attackSpeed);
        }
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
                    if (stat.team == Team.Red)
                    {
                        target = command;
                    }
                    else
                    {
                        for (int i = 0; i < gm.mobList.Count; i++)
                        {
                            if (gm.mobList[i].GetComponent<Stat>().team != stat.team)
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
                        if (gm.mobList[i].GetComponent<Stat>().team != stat.team)
                        {
                            target = gm.mobList[i];
                        }
                    }
                }

            }
            else
            {
                anim.SetBool("isShot", false);
                if (stat.team == Team.Red)
                {
                    target = command;
                }
                else
                {
                    for (int i = 0; i < gm.mobList.Count; i++)
                    {
                        if (gm.mobList[i].GetComponent<Stat>().team != stat.team)
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
        if (stat.team == Team.Blue)
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
        else if(stat.team == Team.Red)
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
                    int balance = gm.gi.rightMobNum - gm.gi.leftMobNum;
                    if (gm.mobList[i].GetComponent<Enemy>())
                    {
                        Enemy e = gm.mobList[i].GetComponent<Enemy>();

                        // 건물일 때
                        if (e.GetComponent<Stat>().state == E_State.Fixed)
                        {
                            // 본인 팀이랑 같을 때
                            if(e.GetComponent<Stat>().team == stat.team)
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
                                            if (balance >= 2 && gm.gi.leftBarricadeNum >= 1)
                                            {
                                                bool flag = true;
                                                Enemy enemy = null;
                                                // 가까운 아군이 가도록 코드를 추가해야함
                                                for(int j = 0; j < gm.mobList.Count; j++)
                                                {
                                                    if (gm.mobList[j].GetComponent<Enemy>() != null)
                                                    {
                                                        enemy = gm.mobList[j].GetComponent<Enemy>();
                                                        if(enemy.stat.team == stat.team && enemy.dir && enemy.stat.state == E_State.Defense)
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
                                                    gm.gi.rightMobNum--;
                                                    gm.gi.leftMobNum++;
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
                                            if (balance <= -2 && gm.gi.rightBarricadeNum >= 1)
                                            {
                                                bool flag = true;
                                                Enemy enemy = null;
                                                // 가까운 아군이 가도록 코드를 추가해야함
                                                for (int j = 0; j < gm.mobList.Count; j++)
                                                {
                                                    if (gm.mobList[j].GetComponent<Enemy>() != null)
                                                    {
                                                        enemy = gm.mobList[j].GetComponent<Enemy>();
                                                        if (enemy.stat.team == stat.team && !enemy.dir && enemy.stat.state == E_State.Defense)
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
                                                    gm.gi.rightMobNum++;
                                                    gm.gi.leftMobNum--;
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
                    stat.state = E_State.TempMove;
                }
            }
            else
            {
                // 현재 엄폐물이 리스트에 있는 지 체크
                bool flag = false;
                for (int i = 0; i < gm.mobList.Count; i++)
                {
                    if (gm.mobList[i].GetComponent<Stat>().team == stat.team)
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
            if (transform.position.x + stat.attackRange >= targetPos.x && transform.position.x - stat.attackRange <= targetPos.x)
            {
                shotTimer += Time.deltaTime;

                // 공속 확인
                if (shotTimer >= stat.attackSpeed)
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
        switch (type)
        {
            case EnemyType.Soldier1:
                hp = gm.gi.soldier1Hp;
                mp = gm.gi.soldier1Mp;
                ad = gm.gi.soldier1Ad;
                break;
        }

        stat.maxHp += stat.hp * hp;
        stat.hp += stat.hp * hp;
        stat.maxMp += stat.mp * mp;
        stat.mp += stat.mp * mp;
        stat.ad += stat.ad * ad;

        if (stat.team == Team.Blue)
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
                if (temp.stat.team == Team.Blue && temp.dir)
                {
                    if (temp.stat.state == E_State.Defense)
                    {
                        r++;
                    }
                    else if (temp.stat.state == E_State.Fixed)
                    {
                        if (temp.coverList.Count < gm.gi.coverNum)
                        {
                            rb++;
                        }
                    }
                }
                else if (temp.stat.team == Team.Blue && !temp.dir)
                {
                    if (temp.stat.state == E_State.Defense)
                    {
                        l++;
                    }
                    else if (temp.stat.state == E_State.Fixed)
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
        gm.gi.rightMobNum = r;
        gm.gi.leftMobNum = l;
        gm.gi.rightBarricadeNum = rb;
        gm.gi.leftBarricadeNum = lb;
    }

    public void EnemySpawn(EnemyType p_type)
    {
        type = p_type;
    }


    void Die()
    {
        if (stat.hp <= 0)
        {
            if (gm.mobList != null)
            {
                gm.mobList.Remove(gameObject);
            }

            // 바리케이드라면
            if (stat.state == E_State.Fixed)
            {
                if (m_BulidPointRespawnCrt != null)
                {
                    StopCoroutine(m_BulidPointRespawnCrt);
                    m_BulidPointRespawnCrt = null;
                }
                m_BulidPointRespawnCrt = StartCoroutine(BuildPointRespawnCrt());
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

    void Debuff()
    {
        // 점화
        if(fire > 0)
        {
            float damage = fireDamage;
            if (stat.shield > 0)
            {
                if (stat.shield >= damage)
                {
                    stat.shield -= damage;
                    damage = 0;
                }
                else
                {
                    damage -= stat.shield;
                    stat.shield = 0;
                }
            }

            stat.hp -= damage;

            fire -= Time.deltaTime;
        }
    }

    void DieMotion()
    {
        if (stat.hp <= 0 && !die)
        {
            if(stat.state == E_State.Player)
            {
                GetComponent<Player>().freeze = true;
            }

            if (stat.team == Team.Blue)
            {
                GameInfoMobUpdate();
            }

            // 건물용
            anim.enabled = true;

            die = true;
            col.enabled = false;

            if(rigid != null)
            {
                rigid.gravityScale = 0;
            }

            if(stat.state != E_State.Fixed)
            {
                anim.SetTrigger("isDie");
            }
           

            // hp가 없는게 아니라면
            if (hpBar != null)
            {
                hpBar.GetComponentInParent<Animator>().SetTrigger("Off");
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
}
