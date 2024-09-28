using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager GetInstance() { Init(); return instance; }

    public List<GameObject> goList = new List<GameObject>();
    public SoundManager sm;
    public List<GameObject> mobList = new List<GameObject>();

    public int day = 0;
    public int hour = 0;
    public int minute = 0;
    float seconds = 0;

    public LoadingSceneManager lm;

    public bool timerOn = false;

    public Player player;
    // 플레이어 관련 스탯
    public float maxhp;
    public float hp;
    public float maxmp;
    public float mp;

    public float ad;
    public float attackSpeed;

    public float moveSpeed;
    public float runSpeed;

    public float jumpPower;

    // 게임 정보
    public GameInfo gi;

    void Awake()
    {
        Init();

        sm = GetComponent<SoundManager>();
        if (GetComponent<GameInfo>() == null)
        {
            gi = gameObject.AddComponent<GameInfo>();
        }
        else
        {
            gi = GetComponent<GameInfo>();
        }
    }

    void Update()
    {
        InputSystem();

        // 시간
        SetDate();
    }

    void InputSystem()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            MenuOff();
        }
    }

    static void Init()
    {
        if (instance == null)
        {
            //GameManager가 존재하는지 확인
            GameObject go = GameObject.Find("GameManager");
            //없으면 생성
            if (go == null)
            {
                go = new GameObject { name = "GameManager" };
            }
            if (go.GetComponent<GameManager>() == null)
            {
                go.AddComponent<GameManager>();
            }
            //없어지지 않도록 해줌

            // 이게 없어야 씬 전환이 원활해짐
            // (이 코드를 활성화 시 전투씬 입장 할 때 초기화가 안되는 오류가 있었음)
            DontDestroyOnLoad(go);

            //instance 할당
            instance = go.GetComponent<GameManager>();
        }
    }

    public void MenuOff()
    {
        if (goList.Count > 0)
        {
            goList.LastOrDefault().GetComponent<Animator>().SetTrigger("Off");
            goList.Remove(goList.LastOrDefault());
            if (sm != null)
            {
                sm.PlayEffectSound(sm.click);
            }
        }
    }

    void SetDate()
    {
        if (timerOn)
        {
            seconds += Time.deltaTime;
            if (seconds >= 1)
            {
                minute++;
                seconds = 0;
                if (minute >= 60)
                {
                    // 
                    OneMinutePlay();

                    hour++;
                    minute = 0;
                    if (hour >= 24)
                    {
                        day++;
                        hour = 0;
                    }
                }
            }

        }
    }

    public void OneMinutePlay()
    {
        // 자원 수집
        for (int i = 0; i < gi.pointList.Count; i++)
        {
            gi.pointList[i].ResourceCollection();
        }

        //
    }

    public void WorldInfoUpdate()
    {
        for (int i = 0; i < goList.Count; i++)
        {
            if(goList[i].GetComponent<WorldInfo>() != null)
            {
                goList[i].GetComponent<WorldInfo>().ManagerUpdate();
            }
        }
    }
}


