using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BuildPoint : MonoBehaviour
{
    bool check;

    GameManager gm;
    GameObject UI;
    Animator anim;

    void Start()
    {
        gm = GameManager.GetInstance();
    }

    void Update()
    {
        InputSystem();

        if (UI != null)
        {
            // ī�޶� ũ�� ����
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 3, Time.deltaTime * gm.player.controlSpeed);
        }
    }

    public void OpenUI()
    {
        //UI.gameObject.SetActive(true);

        UI = Instantiate(Resources.Load("Prefabs/" + "BuildPointUI") as GameObject);
        UI.transform.position = transform.position + new Vector3(0, 0.75f, 200);
        anim = UI.GetComponent<Animator>();

        GameManager.GetInstance().player.freeze = true;
    }

    void InputSystem()
    {
        // ��ȣ�ۿ� Ű
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (check && UI == null)
            {
                OpenUI();
            }
            else if (UI != null)
            {
                if (gm.gi.gold >= 10)
                {
                    // �ٸ����̵� ����
                    GameObject go = Instantiate(Resources.Load("Prefabs/" + "Barricade") as GameObject);
                    go.transform.position = transform.position;
                    go.GetComponent<Enemy>().coverNum = gm.gi.coverNum;
                    go.GetComponent<Enemy>().buildPoint = gameObject;

                    // ���� ����Ʈ �����
                    gameObject.SetActive(false);
                    anim.SetTrigger("Off");
                    gm.gi.gold -= 10;
                }
                else
                {
                    anim.SetTrigger("Error");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (UI != null)
            {
                anim.SetTrigger("Off");
            }
        }
    }

    public void OffUI()
    {
        Destroy(gameObject);
        GameManager.GetInstance().player.freeze = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            check = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            check = false;
        }
    }
}
