using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_State
{
    Move = 0,
    Attack,
    Defense,
    Retreat
}

public class Enemy : MonoBehaviour
{
    public E_State state;

    public float hp;

    public float ad;
    public float attackSpeed;
    public float attackRange;

    public float moveSpeed;
    public float runSpeed;
    float runValue;

    Animator anim;
    SpriteRenderer spriteRenderer;

    public GameObject target;
    Vector2 destination;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //attackRange += Random.RandomRange()
    }

    void Update()
    {
        MoveTo();
    }

    void MoveTo()
    {
        destination = target.transform.position;

        // �޸��� ����
        if (state == E_State.Move)
        {
            runValue = 0;
            anim.SetBool("isRun", false);
        }
        else if (state == E_State.Attack)
        {
            runValue = runSpeed;
            anim.SetBool("isRun", true);
        }

        // Ÿ�������� �̵�
        Vector2 temppos;
        if(destination.x - attackRange > transform.position.x)
        {
            // ���
            anim.SetBool("isWalk", true);

            // ������ �̵�
            temppos = transform.position;
            temppos.x += 1 * (moveSpeed * (1 + runValue));
            transform.position = temppos;
            
            // ������ȯ
            spriteRenderer.flipX = false;
        }
        else if(destination.x + attackRange < transform.position.x)
        {
            // ���
            anim.SetBool("isWalk", true);

            // ���� �̵�
            temppos = transform.position;
            temppos.x -= 1 * (moveSpeed * (1 + runValue));
            transform.position = temppos;

            // ������ȯ
            spriteRenderer.flipX = true;
        }
    }
}
