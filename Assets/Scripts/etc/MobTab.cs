using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class MobTab : MonoBehaviour
{
    GameManager gm;
    Animator anim;

    public LayerMask mask;
    public LayerMask panelMask;

    public string nameText;
    public TextMeshProUGUI mobName;
    public TextMeshProUGUI level;
    public Image image;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI ad;

    // 소모 자원
    public TextMeshProUGUI gold;
    public Image goldImage;
    public TextMeshProUGUI magic;
    public Image magicImage;
    public TextMeshProUGUI food;
    public Image foodImage;

    GameObject go;

    public GameObject panel;
    public GameObject fieldPanel;

    void Start()
    {
        gm = GameManager.GetInstance();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        ClickCheck();
    }

    void ClickCheck()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray2D ray = new Ray2D(pos, Vector2.zero);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1, mask);
        RaycastHit2D panelHit = Physics2D.Raycast(ray.origin, ray.direction, 1, panelMask);


        // MobTab에 마우스가 닿았는데
        if (hit)
        {
            // 본인 오브젝트면
            if (hit.collider.gameObject == gameObject)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    anim.SetTrigger("isClick");
                    go = Instantiate(Resources.Load("Prefabs/Mob/" + nameText) as GameObject);
                    go.GetComponent<Enemy>().spawnWaiting = true;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    anim.SetTrigger("isCancel");
                    Destroy(go);
                }
            }
        }
        // 패널에 닿았다면
        else if(panelHit)
        {
            // 패널안에 마우스가 있다면
            if (Input.GetMouseButtonUp(0))
            {
                // 커맨드 패널이면
                if (panelHit.collider.gameObject == panel)
                {
                    // 취소
                    anim.SetTrigger("isCancel");
                    Destroy(go);
                }
                // 필드 패널이면
                else if(panelHit.collider.gameObject == fieldPanel)
                {
                    // 소환
                    go.GetComponent<Enemy>().spawnWaiting = false;
                    gm.mobList.Add(go);

                    Destroy(gameObject);
                }
            }
        }
        // 이상한 곳이라면
        else
        {
            if (Input.GetMouseButtonUp(0) && go != null)
            {
                // 오류 메세지와 취소
                anim.SetTrigger("isCancel");
                Destroy(go);
            }
        }
    }
}
