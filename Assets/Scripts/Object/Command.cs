using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command : MonoBehaviour
{
    bool check;

    public GameObject potalUI;
    Animator anim;

    void Start()
    {
        anim = potalUI.GetComponent<Animator>();
    }

    void LateUpdate()
    {
        InputSystem();
    }

    public void OpenPotalUI()
    {
        potalUI.gameObject.SetActive(true);
        GameManager.GetInstance().player.freeze = true;
    }

    public void UsePotal()
    {

    }

    void InputSystem()
    {
        // 상호작용 키
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(check && !potalUI.gameObject.activeSelf)
            {
                OpenPotalUI();
            }
            else if (potalUI.gameObject.activeSelf)
            {
                GameManager.GetInstance().lm.StartFadeIn((int)STAGE.LOBBY);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.X))
        {
            if (potalUI.gameObject.activeSelf)
            {
                anim.SetTrigger("Off");
                GameManager.GetInstance().player.freeze = false;
            }
        }
    }

    public void OffPotalUI()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
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
