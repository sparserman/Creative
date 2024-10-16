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

public enum EnemyType
{
    Soldier1, Soldier2, Soldier3,
    Gangster1, Gangster2, Gangster3,
    FireWizard, Tower, Barricade,
    LightningWizard, WandererWizard,
    Raider1, Raider2, Raider3
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
    public float runSpeed = 1.5f;  // Run���� ������ �� (�߰� %)
    public float runValue;  // Run���� ������ �� (�߰� %)

    public float jumpPower = 250;
    public bool downJump;
}

public class Enemy : MonoBehaviour
{
    GameManager gm;

    // �� Ÿ��
    public EnemyType enemyType;

    public List<GameObject> shotPos;

    // �����ڼ��� �þ�� ��Ÿ�
    public float plusRange = 1;    // ���� �� �þ ��Ÿ�
    public float moveRange = 1.2f;  // �̵��� ��Ÿ�
    public float coverRange = 1.2f; // ���� �� ��Ÿ�
    public float skillRange = 0;    // ��ų ��Ÿ� ������

    public float curRange = 1;      // ���� ���� ��Ÿ�

    public float curMoveSpeed = 1;      // ���� �̵��ӵ�

    // Bar
    public Image hpBar;
    public Image hpBack;
    public Image mpBar;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Collider2D col;
    public Stat stat;

    // Ÿ�� ����
    public GameObject target;
    Vector2 destination;

    // �ٴ� ���̾��ũ
    public LayerMask ground;

    // �׾��� ��
    bool die;

    // Ŀ�ǵ�
    GameObject command;

    // ���� (true : ������)
    public bool dir;

    // ���� ����
    public Enemy cover;
    public List<Enemy> coverList;       // �������� ����
    public int coverNum;                // ���󰡴��ο�
    public GameObject buildPoint;       // ���� ����Ʈ
    public GameObject coverShotObj;     // �ǹ��� ��ž


    // �����̻�
    public float fire;          // ��ȭ ���ӽð�
    public float fireDamage;    // ��ȭ ������

    public float smoke;         // ���� ���ӽð�
    public float smokeRange = 1;    // �������� ���� ��Ÿ� ���� (%�� �����ϴ� ȿ��)
    public float smokeSpeed = 1;    // �������� ���� �̵��ӵ� ���� (%�� �����ϴ� ȿ��)



    // �巡�� ��
    public bool spawnWaiting = false;   // ��ȯ ��� ���� (Ư�� ����)

    // ��ų�� ��� ������ ��
    public bool skillOn = false;

    // �޸��� ���
    public bool runOn = false;

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

        // ü�� ����
        if (hpBar != null)
        {
            HpSetting();
        }

        // �ٸ����̵���
        if (stat.state == E_State.Fixed)
        {
            // �⺻ ������� ��ġ ���ġ
            EnemyPositionChange();
        }

        // ��Ÿ� ����
        stat.attackRange += Random.Range(0.21f, -0.21f);
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
                DieMotion();            // �״� ���
                DirectionCheck();       // ���� üũ

                // �÷��̾ �ƴϸ� �� Ž��
                if (stat.state != E_State.Player)
                {
                    // ��Ÿ� ���� ����
                    curRange = (stat.attackRange * plusRange + skillRange) * smokeRange;

                    // �̵��ӵ� ���� ����
                    curMoveSpeed = (1 * stat.moveSpeed * stat.runValue) * smokeSpeed;

                    Sense();
                    SpecialSkill();

                    AttackSpeedUpdate();    // ���� �ӵ� ����
                    StateAction();          // ���¿� ���� �ൿ
                }
                Debuff();               // �����̻� ȿ������

                if (hpBar != null)
                {
                    HpUpdate();         // ü�� �̹��� ����
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
                    // ���� ����
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
        // ü�¹� ǥ��
        if ((stat.hp + stat.shield) / stat.maxHp > 1)
        {
            hpBar.fillAmount = (stat.hp + (stat.maxHp - (stat.hp + stat.shield))) / stat.maxHp;
        }
        else
        {
            hpBar.fillAmount = stat.hp / stat.maxHp;
        }


        // hpBarBack�� �������
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

        // ���ҷ� ǥ��
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
                hpBack.fillAmount = hpBar.fillAmount;
            }
        }

        // MP ǥ��
        if (mpBar != null)
        {
            mpBar.fillAmount = stat.mp / stat.maxMp;
        }
    }

    // �޸��� ����
    void RunControl()
    {
        // �޸��� ����
        if (!runOn)
        {
            stat.runValue = 1;
            anim.SetBool("isRun", false);
        }
        else
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

        // ��ǥ �̵�
        TargetAttack(curRange, curRange, moveRange);

        JumpSystem();
        
    }

    // ��ǥ �̵�
    void TargetAttack(float p_right, float p_left, float p_range)
    {
        // ���� ��ȯ
        if (destination.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        // Ÿ���� null�� �ƴҶ���
        if (target != null)
        {
            Vector2 targetPos = target.transform.position;
            // Ÿ���� ��Ÿ����� ���԰� �������� Ÿ�ٹ����϶�, ���� üũ�� ��
            if //(Vector2.Distance(transform.position, targetPos) > stat.attackRange * plusRange && targetPos.x > transform.position.x)
            (transform.position.x + curRange >= targetPos.x && transform.position.x - curRange <= targetPos.x)
            {
                // ���
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
            // �������� �̵�
            else
            {
                anim.SetBool("isShot", false);
                DestinationMove(p_right, p_left, p_range);
            }
        }
        // �������� �̵�
        else
        {
            anim.SetBool("isShot", false);
            DestinationMove(p_right, p_left, p_range);
        }
    }

    void DestinationMove(float p_right, float p_left, float p_range)
    {
        Vector2 temppos;
        // �������� �̵�
        if //(Vector2.Distance(transform.position, destination) > p_dis && destination.x > transform.position.x)
            (transform.position.x + p_right <= destination.x)
        {
            plusRange = 1;

            // ���
            anim.SetBool("isMove", true);
            anim.SetBool("isShot", false);

            // ������ �̵�
            temppos = transform.position;
            temppos.x += curMoveSpeed;
            transform.position = temppos;
        }
        else if //(Vector2.Distance(transform.position, destination) > p_dis && destination.x < transform.position.x)
            (transform.position.x - p_left >= destination.x)
        {
            plusRange = 1;

            // ���
            anim.SetBool("isMove", true);
            anim.SetBool("isShot", false);

            // ���� �̵�
            temppos = transform.position;
            temppos.x -= curMoveSpeed;
            transform.position = temppos;
        }
        // �������� ������ ���� (���)
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

            // ���潺�� ��Ÿ� �̸� ����
            if (stat.state == E_State.Defense)
            {
                plusRange = p_range;
            }
        }
    }

    void JumpSystem()
    {
        // �� ����
        if (rigid.velocity.y > 0f)
        {
            col.enabled = false;
        }
        else
        {
            if (!stat.downJump)
            {
                col.enabled = true;

                // ����
                if (Physics2D.BoxCast(transform.position, new Vector2(0.21f, 0.1f), 0, -transform.up, 0.6f, ground))
                {
                    anim.SetBool("isJump", false);
                }
            }
        }

        // �������� ���� �ִ� �� üũ
        if (destination.y > transform.position.y + 1.5f && !anim.GetBool("isJump"))
        {
            if (target != null)
            {
                // Ÿ���� ���� ��������
                if (target.GetComponent<Animator>() != null)
                {
                    if (!target.GetComponent<Animator>().GetBool("isJump"))
                    {
                        // ����
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
                // ����
                Debug.DrawRay(transform.position, transform.up * 1.5f);
                if (Physics2D.Raycast(transform.position, transform.up, 1.5f, ground))
                {
                    rigid.AddForce(transform.up * stat.jumpPower);
                    anim.SetBool("isJump", true);
                }
            }
        }
        // �Ʒ��� �ִ� �� üũ
        else if (destination.y < transform.position.y - 1.5f && !anim.GetBool("isJump"))
        {
            if (target != null)
            {
                // Ÿ���� ���� ��������
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
            switch (enemyType)
            {
                case EnemyType.Soldier1:
                case EnemyType.Soldier2:
                case EnemyType.Soldier3:
                case EnemyType.Raider2:
                case EnemyType.Gangster1:
                    {
                        go = Instantiate(Resources.Load("Prefabs/Projectile/" + "Bullet") as GameObject);
                        Vector2 dir = (target.transform.position + new Vector3(0, Random.Range(-0.07f, 0.07f), 0)) - transform.position;
                        go.GetComponent<Bullet>().Init(stat.team, dir.normalized, stat.shotSpeed, stat.ad, target);
                    }
                    break;
                case EnemyType.FireWizard:
                    {
                        go = Instantiate(Resources.Load("Prefabs/Projectile/" + "FireBall") as GameObject);
                        Vector2 dir = target.transform.position - transform.position;
                        go.GetComponent<Bullet>().Init(stat.team, dir.normalized, stat.shotSpeed, stat.ad, target);
                    }
                    break;
                case EnemyType.Tower:
                    {
                        go = Instantiate(Resources.Load("Prefabs/Projectile/" + "TowerBullet") as GameObject);
                        Vector2 dir = new Vector3(target.transform.position.x - transform.position.x, 0, 0);
                        go.GetComponent<Bullet>().Init(stat.team, dir.normalized, stat.shotSpeed, stat.ad, target);
                    }
                    break;
                case EnemyType.Raider1:
                    {
                        // ����
                        for (int i = -2; i < 3; i++)
                        {
                            GameObject tempGo = Instantiate(Resources.Load("Prefabs/Projectile/" + "Bullet") as GameObject);
                            Vector2 dir = (target.transform.position + new Vector3(0, -0.18f * i, 0)) - transform.position;
                            tempGo.GetComponent<Bullet>().Init(stat.team, dir.normalized, stat.shotSpeed, stat.ad, target);

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
                case EnemyType.Raider3:
                    {
                        // �׳� ������ �ֱ�
                        target.GetComponent<Enemy>().Damaged(stat.ad);
                    }
                    break;
            }

            // �Ѿ� ������Ʈ�� ��������� ��������
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
        if (stat.state != E_State.Fixed && stat.state != E_State.Building && stat.state != E_State.Player)
        {
            float temp = 1f / stat.attackSpeed;

            if(temp >= 2.5f)
            {
                temp = 2.5f;
            }
            else if(temp <= 0.7f)
            {
                temp = 0.7f;
            }

            anim.SetFloat("AttackSpeed", temp);

            // ���� �����ε� Ÿ�̸Ӱ� �������
            if (anim.GetBool("isShot") && attackTimer < 0)
            {
                // 0���� �����ؼ� Ÿ�̸� �۵�
                attackTimer = 0;
            }

            // Ÿ�̸Ӱ� 0���� ũ�� ���Ӻ��� �۾ƾ� �ð��� ������
            if (attackTimer <= stat.attackSpeed && attackTimer >= 0)
            {
                attackTimer += Time.deltaTime;
            }
            // ���� �ӵ��� ���� Ʈ���� ���ֱ�
            if(attackTimer >= stat.attackSpeed)
            {
                anim.SetTrigger("isAttack");
                attackTimer = -1;
            }
        }
    }

    // ���� Ÿ�̸� ����
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
                // ���� Ÿ���� ����Ʈ�� �ִ� �� üũ
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
                            if (gm.mobList[i].GetComponent<Enemy>().stat.team != stat.team)
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
                        if (gm.mobList[i].GetComponent<Enemy>().stat.team != stat.team)
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
                        if (gm.mobList[i].GetComponent<Enemy>().stat.team != stat.team)
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
        // Ŀ�ǵ� ��ġ
        float cx = command.transform.position.x;

        // Ŀ�ǵ� ���� ���� ���� (blue)
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

    // ���� ����
    public int count = 1;
    void CoverSystem()
    {
        RunControl();

        if (gm.mobList != null)
        {
            // ������ ������ ���� �ǹ��� ������
            if (cover == null)
            {
                Enemy temp = null;
                for (int i = 0; i < gm.mobList.Count; i++)
                {
                    int balance = gm.gi.rightMobNum - gm.gi.leftMobNum;
                    if (gm.mobList[i].GetComponent<Enemy>())
                    {
                        Enemy e = gm.mobList[i].GetComponent<Enemy>();

                        // �ǹ��� ��
                        if (e.GetComponent<Enemy>().stat.state == E_State.Fixed)
                        {
                            // ���� ���̶� ���� ��
                            if(e.GetComponent<Enemy>().stat.team == stat.team)
                            {
                                // �ִ� ���� ���� �ο��� ����ٸ� ������ ����� �߰�
                                if (e.coverList.Count < e.coverNum)
                                {
                                    // ó���� ������ �Է�
                                    if (temp == null)
                                    {
                                        temp = e;
                                    }
                                    // ���� �� ���󹰷� �Է�
                                    else
                                    {
                                        if (dir)
                                        {
                                            // ���� �Ʊ��� ������ ���������� ���� ��ȯ (�ݴ뿡 ���ڸ��� �ִ� �ٸ����̵尡 �־����)
                                            if (balance >= 2 && gm.gi.leftBarricadeNum >= 1)
                                            {
                                                bool flag = true;
                                                Enemy enemy = null;
                                                // ����� �Ʊ��� ������ �ڵ带 �߰��ؾ���
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

                                                // ������ ���� Ŀ�ǵ忡 ������ ����
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

                                            // ���� ��ȯ
                                            if (balance <= -2 && gm.gi.rightBarricadeNum >= 1)
                                            {
                                                bool flag = true;
                                                Enemy enemy = null;
                                                // ����� �Ʊ��� ������ �ڵ带 �߰��ؾ���
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

                // ������ ������ ������
                if (temp != null)
                {
                    // �߰�
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
                // ���� ������ ����Ʈ�� �ִ� �� üũ
                bool flag = false;
                for (int i = 0; i < gm.mobList.Count; i++)
                {
                    if (gm.mobList[i].GetComponent<Enemy>().stat.team == stat.team)
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

                // ���� Ȯ��
                if (shotTimer >= stat.attackSpeed)
                {
                    // ���
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
        // ���� ����
        DirectionCheck();

        float hp = 0;
        float mp = 0;
        float ad = 0;
        switch (enemyType)
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
        // ������ �ο� ��, ���� �ο� ��, �� �� �� �ٸ����̵� ��
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
        // ���� ������ �ֱ�
        gm.gi.rightMobNum = r;
        gm.gi.leftMobNum = l;
        gm.gi.rightBarricadeNum = rb;
        gm.gi.leftBarricadeNum = lb;
    }

    public void EnemySpawn(EnemyType p_type)
    {
        enemyType = p_type;
    }

    void Debuff()
    {
        // ��ȭ
        if (fire > 0)
        {
            float damage = fireDamage;
            Damaged(damage);

            fire -= Time.deltaTime;
        }
    }

    void Die()
    {
        if (stat.hp <= 0)
        {
            if (gm.mobList != null)
            {
                gm.mobList.Remove(gameObject);
            }

            // �ٸ����̵���
            if (stat.state == E_State.Fixed)
            {
                // ���� ����Ʈ ����� �ڷ�ƾ
                if (m_BulidPointRespawnCrt != null)
                {
                    StopCoroutine(m_BulidPointRespawnCrt);
                    m_BulidPointRespawnCrt = null;
                }
                m_BulidPointRespawnCrt = StartCoroutine(BuildPointRespawnCrt());
            }
            else if (stat.state == E_State.Player)
            {
                // �÷��̾� ��Ȱ �ڷ�ƾ
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
                // ���� ����Ʈ ����
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
                // �÷��̾� ��Ȱ
                gm.player.die = false;
                gm.player.freeze = false;
                hpBar.transform.parent.gameObject.SetActive(true);
                stat.hp = stat.maxHp;
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
        if (stat.hp <= 0 && !die)
        {
            if(stat.state == E_State.Player)
            {
                GetComponent<Player>().freeze = true;
                gm.player.respawnTimer.transform.parent.gameObject.SetActive(true);
                gm.player.cMode = true;
                gm.player.respawnTimer.text = "";
                gm.player.die = true;
            }

            if (stat.team == Team.Blue)
            {
                GameInfoMobUpdate();
            }

            // �ǹ���
            anim.enabled = true;

            die = true;
            //col.enabled = false;

            if(rigid != null)
            {
                rigid.gravityScale = 0;
            }

            if(stat.state != E_State.Fixed && stat.state != E_State.Building)
            {
                anim.SetTrigger("isDie");
            }
           

            // hp�� ���°� �ƴ϶��
            if (hpBar != null)
            {
                hpBar.GetComponentInParent<Animator>().SetTrigger("Off");
                
            }
            // mp�� ���°� �ƴ϶��
            if (mpBar != null)
            {
                mpBar.GetComponentInParent<Animator>().SetTrigger("Off");
            }

            if (gm.mobList != null)
            {
                gm.mobList.Remove(gameObject);
                if(cover != null)
                {
                    // �ٸ����̵� ���� ����
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
            if (!anim.GetBool("isSkill") && skillOn)
            {
                stat.mp += Time.deltaTime;
            }
        }
    }

    void SpecialSkill()
    {
        if(stat.mp >= stat.maxMp && skillOn)
        {
            anim.SetBool("isSkill", true);
            skillRange = 0.4f;
            stat.mp = stat.maxMp;
        }
    }

    public void UseSkill()
    {
        if (target != null)
        {
            GameObject go = null;
            switch (enemyType)
            {
                case EnemyType.Soldier1:
                    {

                    }
                    break;
                case EnemyType.Gangster1:
                    {
                        
                    }
                    break;
                case EnemyType.FireWizard:
                    {
                        go = Instantiate(Resources.Load("Prefabs/Projectile/" + "FireBall") as GameObject);
                        Vector2 dir = target.transform.position - transform.position;
                        go.GetComponent<Bullet>().Init(stat.team, dir.normalized, stat.shotSpeed, stat.ad * 1.5f, target);
                        go.transform.localScale = new Vector3(3, 3, 1);
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

            stat.mp = 0;
            anim.SetBool("isSkill", false);
            skillRange = 0f;
        }
    }

    public void Damaged(float damage)
    {
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
    }
}
