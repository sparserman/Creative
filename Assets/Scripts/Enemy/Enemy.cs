using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public enum Team
{
    Blue = 0, Red
}
public enum E_State
{
    Move = 0,
    Attack,
    Defense,
    Retreat,
    Fixed
}

public class Enemy : MonoBehaviour
{
    GameManager gm;

    public List<GameObject> shotPos;

    // �����ڼ��� �þ�� ��Ÿ�
    float plusRange = 1;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Collider2D col;
    Stat stat;

    public GameObject target;
    Vector2 destination;

    public LayerMask ground;

    bool die;


    void Start()
    {
        gm = GameManager.GetInstance();

        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        stat = GetComponent<Stat>();

        gm.mobList.Add(gameObject);

        //attackRange += Random.RandomRange()
    }

    void FixedUpdate()
    {
        if (!die)
        {
            if (stat.state != E_State.Fixed)
            {
                Sense();
                MoveTo();
                DieMotion();
            }
            else
            {
                Die();
            }
        }
    }

    void MoveTo()
    {
        anim.SetBool("isShot", false);

        if(target == null)
        {
            return;
        }

        destination = target.transform.position;

        // �޸��� ����
        if (stat.state == E_State.Move)
        {
            stat.runValue = 0;
            anim.SetBool("isRun", false);
        }
        else if (stat.state == E_State.Attack)
        {
            stat.runValue = stat.runSpeed;
            anim.SetBool("isRun", true);
        }

        // ���� ��ȯ
        if(destination.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        // Ÿ�������� �̵�
        Vector2 temppos;
        if(Vector2.Distance(transform.position, destination) > stat.attackRange * plusRange && destination.x > transform.position.x)
        {
            plusRange = 1;

            // ���
            anim.SetBool("isMove", true);

            // ������ �̵�
            temppos = transform.position;
            temppos.x += 1 * (stat.moveSpeed * (1 + stat.runValue));
            transform.position = temppos;
        }
        else if(Vector2.Distance(transform.position, destination) > stat.attackRange * plusRange && destination.x < transform.position.x)
        {
            plusRange = 1;

            // ���
            anim.SetBool("isMove", true);

            // ���� �̵�
            temppos = transform.position;
            temppos.x -= 1 * (stat.moveSpeed * (1 + stat.runValue));
            transform.position = temppos;
        }
        // Ÿ���� ��Ÿ����� ���� ���� (attack)
        else
        {
            anim.SetBool("isShot", true);
            plusRange = 1.2f;
        }
        
        // Ÿ���� ���� �ִ� �� üũ
        if (target.transform.position.y > transform.position.y + 1.5f && !anim.GetBool("isJump"))
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
        // �Ʒ��� �ִ� �� üũ
        else if(target.transform.position.y < transform.position.y - 1.5f && !anim.GetBool("isJump"))
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
        else if(target.transform.position.y > transform.position.y - 0.6f)
        {
            stat.downJump = false;
        }

        // �� ����
        if (rigid.velocity.y > 0)
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

        
    }

    public void Shot()
    {
        if (target != null)
        {

            GameObject go = Instantiate(Resources.Load("Prefabs/" + "Bullet") as GameObject);

            Vector2 dir = (target.transform.position + new Vector3(0, Random.Range(-0.3f, 0.3f), 0)) - transform.position;
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
                    target = GameObject.Find("Player"); ;
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
                if (stat.team == Team.Red)
                {
                    target = GameObject.Find("Player");
                }
            }
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
            Destroy(gameObject, 10);
        }
    }

    void DieMotion()
    {
        if (stat.hp <= 0 && !die)
        {
            die = true;
            col.enabled = false;
            rigid.gravityScale = 0;
            anim.SetTrigger("isDie");
            if (gm.mobList != null)
            {
                gm.mobList.Remove(gameObject);
            }
        }
    }
}
