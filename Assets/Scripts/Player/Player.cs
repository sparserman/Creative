using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxhp;
    public float hp;
    public float maxmp;
    public float mp;

    public float ad;    // attack damage
    public float attackSpeed;

    public float moveSpeed;
    public float runSpeed;
    float runValue;

    public float jumpPower;
    bool downJump;

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;

    public LayerMask ground;

    int playerLayer, groundLayer, enemyLayer;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerLayer = LayerMask.NameToLayer("Player");
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    void Update()
    {
        PlayerMove();
        GroundCheck();
        DownJump();
    }

    void PlayerMove()
    {
        Vector3 temppos = new Vector3(0,0,0);

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(!anim.GetBool("isRun"))
            {
                anim.SetBool("isRun", true);
                runValue = runSpeed;
            }
            else
            {
                anim.SetBool("isRun", false);
                runValue = 0;
            }
        }
        

        // 이동
        temppos.x = Input.GetAxisRaw("Horizontal") * (moveSpeed * (1 + runValue));
        transform.position = transform.position + temppos;
        // 방향전환
        if(Input.GetAxisRaw("Horizontal") == 1)
        {
            spriteRenderer.flipX = false;
        }
        else if(Input.GetAxisRaw("Horizontal") == -1)
        {
            spriteRenderer.flipX = true;
        }


        // 점프
        if (!anim.GetBool("isJump"))
        {
            // 하단 점프
            if(Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.X))
            {
                downJump = true;
            }
            // 점프
            else if (Input.GetKeyDown(KeyCode.X))
            {
                rigid.AddForce(transform.up * (jumpPower + (runValue * 100)));
                anim.SetBool("isJump", true);
            }

            // 이동 모션
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                anim.SetBool("isMove", true);
            }
            else
            {
                anim.SetBool("isMove", false);
            }
        }
    }

    // 플레이어 정보 저장 (게임매니저에)
    public void SavePlayerState()
    {
        GameManager.maxhp = maxhp;
        GameManager.hp = hp;
        GameManager.maxmp = maxmp;
        GameManager.mp = mp;
        GameManager.ad = ad;
        GameManager.attackSpeed = attackSpeed;
        GameManager.moveSpeed = moveSpeed;
        GameManager.runSpeed = runSpeed;
        GameManager.jumpPower = jumpPower;
    }

    void GroundCheck()
    {
        if (rigid.velocity.y < 0)
        {
            if (!downJump)
            {
                if(Physics2D.BoxCast(transform.position, new Vector2(0.21f, 0.1f), 0, -transform.up, 0.6f, ground))
                {
                    anim.SetBool("isJump", false);
                }

                // 이 방식은 땅 끝을 밟을 시 착지 버그 발생
                //Debug.DrawRay(transform.position, -transform.up * 0.6f);
                //if (Physics2D.Raycast(transform.position, -transform.up, 0.6f, ground))
                //{
                //    anim.SetBool("isJump", false);
                //}
            }
        }

        if(rigid.velocity.y > 0)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);
        }
    }

    void DownJump()
    {
        if (downJump)
        {
            Debug.DrawRay(transform.position, -transform.up * 0.6f, new Color(1, 0, 0));
            if (Physics2D.Raycast(transform.position, -transform.up, 0.6f, ground))
            {
                Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);
                anim.SetBool("isJump", true);
            }
            else
            {
                Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);
                downJump = false;
            }
        }
    }
}
