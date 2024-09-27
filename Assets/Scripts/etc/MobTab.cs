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

    // �Ҹ� �ڿ�
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


        // MobTab�� ���콺�� ��Ҵµ�
        if (hit)
        {
            // ���� ������Ʈ��
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
        // �гο� ��Ҵٸ�
        else if(panelHit)
        {
            // �гξȿ� ���콺�� �ִٸ�
            if (Input.GetMouseButtonUp(0))
            {
                // Ŀ�ǵ� �г��̸�
                if (panelHit.collider.gameObject == panel)
                {
                    // ���
                    anim.SetTrigger("isCancel");
                    Destroy(go);
                }
                // �ʵ� �г��̸�
                else if(panelHit.collider.gameObject == fieldPanel)
                {
                    // ��ȯ
                    go.GetComponent<Enemy>().spawnWaiting = false;
                    gm.mobList.Add(go);

                    Destroy(gameObject);
                }
            }
        }
        // �̻��� ���̶��
        else
        {
            if (Input.GetMouseButtonUp(0) && go != null)
            {
                // ���� �޼����� ���
                anim.SetTrigger("isCancel");
                Destroy(go);
            }
        }
    }
}
