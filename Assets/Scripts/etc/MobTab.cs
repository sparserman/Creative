using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;


public class MobTab : MonoBehaviour
{
    GameManager gm;
    Animator anim;

    public MobInfo mobInfo;

    public LayerMask mask;
    public LayerMask panelMask;

    public string nameText;
    public TextMeshProUGUI mobName;
    public TextMeshProUGUI level;
    public Image image;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI ad;
    public TextMeshProUGUI timer;

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

    // 위치
    public int num;

    void Start()
    {
        gm = GameManager.GetInstance();
        anim = GetComponent<Animator>();

        Init();
        ResourceImageDrawCheck();
        
    }

    void Update()
    {
        ClickCheck();           // 클릭 확인
        FieldColorChange();     // 필드 색 변경
        MoveTab();              // 탭 자리이동
        TimerUpdate();          // 대기시간 표시
    }

    void ClickCheck()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray2D ray = new Ray2D(pos, Vector2.zero);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1, mask);
        RaycastHit2D panelHit = Physics2D.Raycast(ray.origin, ray.direction, 1, panelMask);


        Vector3 vec = Vector3.zero;
        if (go != null)
        {
            // 몹 위치를 마우스에 맞춰 이동
            go.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            vec = go.transform.position;
            vec.z = 0;
        }

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
                    go.GetComponent<Enemy>().stat = mobInfo.stat;
                    go.GetComponent<Enemy>().enemyType = mobInfo.type;
                    go.GetComponent<Enemy>().skillOn = mobInfo.skillOn;

                    // 필드 패널 보이기
                    fieldPanel.SetActive(true);
                }
                else if (Input.GetMouseButtonUp(0) && go != null)
                {
                    // 소환 취소
                    SpawnCancel();
                }
            }
            else if(Input.GetMouseButtonUp(0) && go != null)
            {
                // 소환 취소
                SpawnCancel();
            }
        }
        // 패널에 닿았다면
        else if (panelHit && go != null)
        {
            // 패널에 마우스가 있다면
            if (panelHit.collider.gameObject == fieldPanel)
            {
                // 몹 위치를 바닥에 붙이기
                vec.y = -2.4f;
            }

            // 패널안에 마우스가 있다면
            if (Input.GetMouseButtonUp(0))
            {
                // 커맨드 패널이면
                if (panelHit.collider.gameObject == panel)
                {
                    // 소환 취소
                    SpawnCancel();
                }
                // 필드 패널이면
                else if (panelHit.collider.gameObject == fieldPanel)
                {

                    if (ResourceCheck() && TimerCheck() && !gm.player.die)
                    {
                        // 자원 계산
                        gm.gi.gold -= mobInfo.gold;
                        gm.gi.magic -= mobInfo.magic;
                        gm.gi.food -= mobInfo.food;

                        // 이미 소환되어있다면
                        if(SpawnCheck())
                        {
                            // 생성되어있는 병사 삭제
                            GameObject temp = CurSpawnObject();
                            gm.mobList.Remove(temp);
                            Destroy(temp);
                        }

                        // 소환
                        go.GetComponent<Enemy>().spawnWaiting = false;
                        gm.mobList.Add(go);

                        // 스탯 리셋
                        mobInfo.ResetStat();


                        // 탭 위치 정리
                        for (int i = 0; i < panel.GetComponent<CommandPanel>().tabList.Count; i++)
                        {
                            panel.GetComponent<CommandPanel>().tabList[i].GetComponent<MobTab>().TabPositionChange(num);
                        }

                        // 현재 탭을 탭 리스트에서 지우기
                        panel.GetComponent<CommandPanel>().tabList.Remove(gameObject);
                        // 대기 큐에 넣기
                        mobInfo.curWaitingTime = mobInfo.waitingTime;   // 대기 시간 적용 후
                        panel.GetComponent<CommandPanel>().waitingTab.Enqueue(mobInfo);
                        Destroy(gameObject);
                    }
                    else
                    {
                        // 소환 취소
                        SpawnCancel();
                    }

                    // 필드 패널 숨기기
                    fieldPanel.SetActive(false);
                }
            }
        }
        // 이상한 곳이라면
        else
        {
            if (Input.GetMouseButtonUp(0) && go != null)
            {
                // 소환 취소
                SpawnCancel();
            }
        }

        if(go != null)
        {
            go.transform.position = vec;
        }
    }

    void Init()
    {
        // 정보 입력
        nameText = mobInfo.nameText;
        mobName.text = mobInfo.mobName;
        level.text = "Lv." + mobInfo.stat.level.ToString();
        image.sprite = mobInfo.sprite;

        hp.text = mobInfo.stat.maxHp.ToString();
        ad.text = mobInfo.stat.ad.ToString();

        // 자원 소모값 입력
        gold.text = mobInfo.gold.ToString();
        magic.text = mobInfo.magic.ToString();
        food.text = mobInfo.food.ToString();
    }

    void ResourceImageDrawCheck()
    {
        // 소모하지않는 자원 이미지와 텍스트는 숨기기
        if(mobInfo.gold == 0)
        {
            goldImage.gameObject.SetActive(false);
            gold.gameObject.SetActive(false);
        }
        if (mobInfo.magic == 0)
        {
            magicImage.gameObject.SetActive(false);
            magic.gameObject.SetActive(false);
        }
        if (mobInfo.food == 0)
        {
            foodImage.gameObject.SetActive(false);
            food.gameObject.SetActive(false);
        }
    }

    bool ResourceCheck()
    {
        if(mobInfo.gold <= gm.gi.gold)
        {
            if (mobInfo.magic <= gm.gi.magic)
            {
                if (mobInfo.food <= gm.gi.food)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void FieldColorChange()
    {
        if(gm.player.die)
        {
            // 빨강
            if (go != null)
            {
                fieldPanel.GetComponent<SpriteRenderer>().color = new Color32(255, 100, 100, 40);
            }
            GetComponent<Image>().color = new Color32(200, 70, 70, 170);
        }
        else if (SpawnCheck())
        {
            // 주황
            if (go != null)
            {
                fieldPanel.GetComponent<SpriteRenderer>().color = new Color32(250, 255, 100, 40);
            }
            GetComponent<Image>().color = new Color32(200, 200, 70, 170);
        }
        else if (ResourceCheck() && TimerCheck())
        {
            // 초록
            if (go != null)
            {
                // 필드 색깔 변경
                fieldPanel.GetComponent<SpriteRenderer>().color = new Color32(100, 255, 200, 40);
            }
            // 탭 색깔 변경
            GetComponent<Image>().color = new Color32(70, 200, 160, 170);
        }
        else
        {
            // 빨강
            if (go != null)
            {
                fieldPanel.GetComponent<SpriteRenderer>().color = new Color32(255, 100, 100, 40);
            }
            GetComponent<Image>().color = new Color32(200, 70, 70, 170);
        }
    }

    void SpawnCancel()
    {
        // 취소
        anim.SetTrigger("isCancel");
        Destroy(go);

        // 필드 패널 숨기기
        fieldPanel.SetActive(false);
    }

    // 탭 자리 잡기
    void MoveTab()
    {
        // 목표 위치
        Vector3 pos = panel.GetComponent<CommandPanel>().posList[num].transform.position;

        // 본인
        Vector3 temppos = transform.position;

        // 자리로 천천히 이동
        temppos = Vector3.Lerp(temppos, pos, Time.deltaTime * 5f);
        transform.position = temppos;
    }

    // 탭 위치 변경
    void TabPositionChange(int p_num)
    {
        if (num > p_num)
        {
            num--;
        }
    }

    bool TimerCheck()
    {
        if(mobInfo.curWaitingTime > 0)
        {
            return false;
        }
        return true;
    }

    void TimerUpdate()
    {
        timer.text = Mathf.Round(mobInfo.curWaitingTime).ToString();
        if(mobInfo.curWaitingTime <= 0)
        {
            timer.gameObject.SetActive(false);
        }
    }

    // 본인이 소환되어있는 지 체크
    bool SpawnCheck()
    {
        for (int i = 0; i < gm.mobList.Count; i++)
        {
            if (gm.mobList[i].GetComponent<Enemy>().stat == mobInfo.stat)
            {
                return true;
            }
        }
        return false;
    }

    // 현재 소환되어있는 오브젝트
    GameObject CurSpawnObject()
    {
        for (int i = 0; i < gm.mobList.Count; i++)
        {
            if (gm.mobList[i].GetComponent<Enemy>().stat == mobInfo.stat)
            {
                return gm.mobList[i];
            }
        }
        return null;
    }
}
