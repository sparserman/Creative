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

    // �����ڼ��� �þ�� ��Ÿ�
    public float plusRange = 1;    // ���� �þ ��Ÿ�
    public float moveRange = 1.2f;
    public float coverRange = 1.2f;

    // ���� �ڼ�

    public Image hpBar;
    public Image hpBack;

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

    public EnemyType type;

    // �����̻�
    public float fire;          // �ɸ� ��ȭ ���ӽð�
    public float fireDamage;    // ��ȭ ������

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
                hpBack.fillAmount = hpBack.fillAmount;
            }
        }
    }

    // �޸��� ����
    void RunControl()
    {
        // �޸��� ����
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

        // ��ǥ �̵�
        TargetAttack(stat.attackRange * plusRange, stat.attackRange * plusRange, moveRange);

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
            (transform.position.x + stat.attackRange * plusRange >= targetPos.x && transform.position.x - stat.attackRange * plusRange <= targetPos.x)
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
            temppos.x += 1 * (stat.moveSpeed * (1 + stat.runValue));
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
            temppos.x -= 1 * (stat.moveSpeed * (1 + stat.runValue));
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
                        if (e.GetComponent<Stat>().state == E_State.Fixed)
                        {
                            // ���� ���̶� ���� ��
                            if(e.GetComponent<Stat>().team == stat.team)
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

            // �ٸ����̵���
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
                // ���� ����Ʈ ����
                buildPoint.SetActive(true);
                Destroy(gameObject);
                break;
            }
            yield return null;
        }
    }

    void Debuff()
    {
        // ��ȭ
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

            // �ǹ���
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
           

            // hp�� ���°� �ƴ϶��
            if (hpBar != null)
            {
                hpBar.GetComponentInParent<Animator>().SetTrigger("Off");
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
}
