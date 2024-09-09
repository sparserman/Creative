using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    public Team team;
    public E_State state;

    public float maxHp = 10;
    public float hp = 10;
    public float maxMp = 0;
    public float mp = 0;

    public float shield = 0;

    public float ad = 1;    // attack damage
    public float attackSpeed = 1;
    public float attackRange = 5;
    public float shotSpeed = 0.1f;

    public float moveSpeed = 0.01f;
    public float runSpeed = 1.5f;  // 값을 정해놓는 곳
    public float runValue;  // 입력될 추가 %

    public float jumpPower = 250;
    public bool downJump;



    void Start()
    {
        attackRange += Random.Range(0.12f, -0.12f);
    }

}
