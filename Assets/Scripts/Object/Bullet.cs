using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Team team;

    Vector2 dir;
    float speed;
    float damage;

    GameObject target;


    void Start()
    {
        Destroy(gameObject, 3f);
    }

    void FixedUpdate()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == target)
        {
            if (team != target.GetComponent<Stat>().team)
            {
                target.GetComponent<Stat>().hp -= damage;
                Destroy(gameObject);
            }
        }
        //else if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
        //{
        //    if (team != collision.gameObject.GetComponent<Stat>().team)
        //    {
        //        collision.gameObject.GetComponent<Stat>().hp -= damage;
        //        Destroy(gameObject);
        //    }
        //}

    }

    void Move()
    {
        Vector2 temppos = transform.position;
        transform.position = temppos + dir * speed;
    }

    public void Init(Team p_team, Vector2 p_dir, float p_speed, float p_damage, GameObject p_target)
    {
        team = p_team;
        dir = p_dir;
        speed = p_speed;
        damage = p_damage;
        target = p_target;
    }
}
