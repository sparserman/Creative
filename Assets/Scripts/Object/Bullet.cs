using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{ 
    Default,
    FireBall
}

public class Bullet : MonoBehaviour
{
    GameManager gm;
    Animator anim;

    Team team;

    Vector2 dir;
    float speed;
    float damage;

    GameObject target;

    public BulletType type;

    // 상태이상 부여체크
    bool debuff = false;


    void Start()
    {
        gm = GameManager.GetInstance();
        anim = gameObject.GetComponent<Animator>();

        // 나중에 시간 수정할수도
        Destroy(gameObject, 3f);
    }

    void FixedUpdate()
    {
        if (!gm.timerOn)
        {
            return;
        }

        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == target)
        {
            if (team != target.GetComponent<Enemy>().stat.team)
            {
                target.GetComponent<Enemy>().Damaged(damage);

                SpecialAbility();
            }
        }

    }


    void SpecialAbility()
    {
        switch(type)
        {
            case BulletType.Default:
                Destroy(gameObject);
                break;
            case BulletType.FireBall:
                for(int i = 0; i < gm.mobList.Count; i++)
                {
                    // 근처 적에게 모두 데미지
                    if (gm.mobList[i].GetComponent<Enemy>().stat.team != team && Vector2.Distance(transform.position, gm.mobList[i].transform.position) <= transform.localScale.x * 0.5f)
                    {
                        gm.mobList[i].GetComponent<Enemy>().fire = gm.gi.fireTime;
                        gm.mobList[i].GetComponent<Enemy>().fireDamage = damage * 0.01f;
                        GameObject go = Instantiate(Resources.Load("Prefabs/" + "IgnitionParticle") as GameObject);
                        go.transform.position = transform.position;
                        go.transform.SetParent(gm.mobList[i].transform, false);
                        go.transform.localPosition = new Vector3(0, 0, 0);
                       
                        Destroy(go, gm.gi.fireTime);
                    }
                }
                anim.SetTrigger("isDestroy");
                debuff = true;
                break;
        }
    }

    void FireDestroy()
    {
        Destroy(gameObject);
    }

    void Move()
    {
        if (!debuff)
        {
            Vector2 temppos = transform.position;
            transform.position = temppos + dir * speed;
        }
    }

    public void Init(Team p_team, Vector2 p_dir, float p_speed, float p_damage, GameObject p_target)
    {
        team = p_team;
        dir = p_dir;
        speed = p_speed;
        damage = p_damage;
        target = p_target;

        if(dir.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }
}
