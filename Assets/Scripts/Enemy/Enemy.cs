using System.Collections;
using System.Collections.Generic;
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
    Building
}

public class Enemy : MonoBehaviour
{
    GameManager gm;

    public List<GameObject> shotPos;

    // �����ڼ��� �þ�� ��Ÿ�
    public float plusRange = 1;    // ���� �þ ��Ÿ�
    public float moveRange = 1.2f;
    public float coverRange = 1.2f;

    public Image hpBar;
    public Image hpBack;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Collider2D col;
    Stat stat;

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
    bool dir;

    // ���� ����
    public Enemy cover;
    // �������� ����
    public List<Enemy> coverList;
    // ���󰡴��ο�
    public int coverNum;

    EnemyType type;

    void Start()
    {
        gm = GameManager.GetInstance();

        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        stat = GetComponent<Stat>();


        command = GameObject.Find("Command");

        //attackRange += Random.RandomRange()

        gm.mobList.Add(gameObject);

        Init();

        if (hpBar != null)
        {
            HpSetting();
        }
    }


    void FixedUpdate()
    {
        if (!die)
        {
            // ���� ����
            DirectionCheck();

            DieMotion();

            if (stat.state != E_State.Fixed && stat.state != E_State.Building)
            {
                Sense();
                StateAction();
            }

            if(hpBar != null)
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
            case E_State.Building:
                break;
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

        if (cover != null)
        {
            cover.coverList.Remove(this);
            cover = null;
        }

        RunControl();

        // ��ǥ �̵�
        DestinationMove(stat.attackRange * plusRange, moveRange);

        JumpSystem();
        
    }

    // ��ǥ �̵�
    void DestinationMove(float p_dis, float p_range)
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

        // Ÿ�������� �̵�
        Vector2 temppos;
        if //(Vector2.Distance(transform.position, destination) > p_dis && destination.x > transform.position.x)
            (transform.position.x + p_dis <= destination.x)
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
            (transform.position.x - p_dis >= destination.x)
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
        // Ÿ���� ��Ÿ����� ���� ���� (attack)
        else
        {
            if(dir)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
            anim.SetBool("isMove", false);
            anim.SetBool("isRun", false);
            // Ÿ���� null�� �ƴҶ���
            if (target != null)
            {
                Vector2 targetPos = target.transform.position;
                // ���潺�� ��Ÿ� �̸� ����
                if (stat.state == E_State.Defense)
                {
                    plusRange = p_range;
                }

                if //(Vector2.Distance(transform.position, targetPos) > stat.attackRange * plusRange && targetPos.x > transform.position.x)
                    (transform.position.x + stat.attackRange * plusRange <= targetPos.x)
                {
                    anim.SetBool("isShot", false);
                }
                else if //(Vector2.Distance(transform.position, targetPos) > stat.attackRange * plusRange && targetPos.x < transform.position.x)
                    (transform.position.x - stat.attackRange * plusRange >= targetPos.x)
                {
                    anim.SetBool("isShot", false);
                }
                else
                {
                    anim.SetBool("isShot", true);
                    plusRange = p_range;
                }
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

            GameObject go = Instantiate(Resources.Load("Prefabs/" + "Bullet") as GameObject);

            Vector2 dir = (target.transform.position + new Vector3(0, Random.Range(-0.15f, 0.15f), 0)) - transform.position;
            go.GetComponent<Bullet>().Init(stat.team, dir.normalized, stat.shotSpeed, stat.ad, target);

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
    int count = 1;
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
                                            if (command.transform.position.x < e.transform.position.x)
                                            {
                                                temp = e;
                                            }
                                        }
                                        else
                                        {
                                            if (command.transform.position.x > e.transform.position.x)
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
                    DestinationMove(0.1f, coverRange + count * 0.1f);
                    JumpSystem();
                }
                else
                {
                    destination.x = cover.transform.position.x + 0.3f * count;
                    DestinationMove(0.1f, coverRange + count * 0.1f);
                    JumpSystem();
                }
            }
        }

    }

    void Init()
    {
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
                m_BulidPointRespawnCrt = StartCoroutine(BuildPointRespawn());
            }
            else
            {
                Destroy(gameObject, 10);
            }

            
        }
    }

    Coroutine m_BulidPointRespawnCrt = null;
    IEnumerator BuildPointRespawn()
    {
        float t = 0;
        while (true)
        {
            t += Time.deltaTime;
            if (t >= 10f)
            {
                // ���� ����Ʈ ����
                GameObject go = Instantiate(Resources.Load("Prefabs/" + "BuildPoint") as GameObject);
                go.transform.position = transform.position;
                Destroy(gameObject);
                break;
            }
            yield return null;
        }
    }

    void DieMotion()
    {
        if (stat.hp <= 0 && !die)
        {
            // �ǹ���
            anim.enabled = true;

            die = true;
            col.enabled = false;
            rigid.gravityScale = 0;

            anim.SetTrigger("isDie");

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
