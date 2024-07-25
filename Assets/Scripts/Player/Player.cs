using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    Stat stat;

    public float cameraSpeed;
    public Vector2 center;
    public Vector2 size;
    float height;
    float width;

    GameManager gm;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;

    public LayerMask ground;

    int playerLayer, groundLayer, enemyLayer;

    // 행동 정지
    public bool freeze;

    void Start()
    {
        gm = GameManager.GetInstance();
        gm.player = this;

        gm.timerOn = true;

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        stat = GetComponent<Stat>();

        playerLayer = LayerMask.NameToLayer("Player");
        groundLayer = LayerMask.NameToLayer("Ground");

        height = Camera.main.orthographicSize;
        width = height * Screen.width / Screen.height;

        gm.mobList.Add(gameObject);
    }

    void Update()
    {
        if (!freeze)
        {
            InputSystem();
            
            DownJump();
        }
        GroundCheck();
        AnimStop();
    }

    private void FixedUpdate()
    {
        if (!freeze)
        {
            PlayerMove();

            
        }
        CameraMove();
    }

    void AnimStop()
    {
        if(freeze)
        {
            anim.SetBool("isMove", false);
        }
    }

    void InputSystem()
    {
        // 달리기 모드
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!anim.GetBool("isRun"))
            {
                anim.SetBool("isRun", true);
                stat.runValue = stat.runSpeed;
            }
            else
            {
                anim.SetBool("isRun", false);
                stat.runValue = 0;
            }
        }

        // 점프
        if (!anim.GetBool("isJump"))
        {
            // 하단 점프
            if (Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.X) && transform.position.y > -2.0f)
            {
                stat.downJump = true;
            }
            // 점프
            else if (Input.GetKeyDown(KeyCode.X))
            {
                rigid.AddForce(transform.up * (stat.jumpPower + (stat.runValue * 100)));
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

    void PlayerMove()
    {
        Vector3 temppos = new Vector3(0,0,0);

        // 이동
        temppos.x = Input.GetAxisRaw("Horizontal") * (stat.moveSpeed * (1 + stat.runValue));
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

    }

    // 플레이어 정보 저장 (게임매니저에)
    public void SavePlayerState()
    {
        GameManager.maxhp = stat.maxhp;
        GameManager.hp = stat.hp;
        GameManager.maxmp = stat.maxmp;
        GameManager.mp = stat.mp;
        GameManager.ad = stat.ad;
        GameManager.attackSpeed = stat.attackSpeed;
        GameManager.moveSpeed = stat.moveSpeed;
        GameManager.runSpeed = stat.runSpeed;
        GameManager.jumpPower = stat.jumpPower;
    }

    void GroundCheck()
    {
        if (rigid.velocity.y < 0)
        {
            if (!stat.downJump)
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

        if (!stat.downJump)
        {
            if (rigid.velocity.y > 0)
            {
                Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);
            }
            else
            {
                Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);
            }
        }

    }

    void DownJump()
    {
        if (stat.downJump)
        {
            Debug.Log("다운점프");
            if (Physics2D.BoxCast(transform.position, new Vector2(0.21f, 0.1f), 0, -transform.up, 0.6f, ground))
            {
                Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);
                anim.SetBool("isJump", true);
            }
            else
            {
                Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);
                stat.downJump = false;
            }
        }
    }

    void CameraMove()
    {
        Vector3 temppos = Camera.main.transform.position;

        temppos = Vector3.Lerp(temppos, transform.position, Time.deltaTime * cameraSpeed);
        temppos.z = -10;
        Camera.main.transform.position = temppos;

        float Ix = size.x * 0.5f - width;
        float clampX = Mathf.Clamp(Camera.main.transform.position.x, -Ix + center.x, Ix + center.x);

        float Iy = size.y * 0.5f - height;
        float clampY = Mathf.Clamp(Camera.main.transform.position.y, -Iy + center.y, Iy + center.y);

        Camera.main.transform.position = new Vector3(clampX, clampY, -10f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
}
