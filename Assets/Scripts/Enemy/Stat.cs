using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    public Team team;
    public E_State state;

    public float maxhp = 10;
    public float hp = 10;
    public float maxmp = 0;
    public float mp = 0;

    public float ad = 1;    // attack damage
    public float attackSpeed;
    public float attackRange = 5;
    public float shotSpeed = 0.1f;

    public float moveSpeed = 0.01f;
    public float runSpeed = 1.5f;  // ���� ���س��� ��
    public float runValue;  // �Էµ� �߰� %

    public float jumpPower = 250;
    public bool downJump;



    void Start()
    {
        attackRange += Random.Range(0.15f, -0.15f);
    }

    void Update()
    {
        
    }
}
