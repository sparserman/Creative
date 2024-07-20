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

    public List<GameObject> goList;
    SoundManager sm;

    static public int day = 0;
    static public int hour = 0;
    static public int minute = 0;
    static float seconds = 0;

    public LoadingSceneManager lm;

    static bool timerOn = false;

    // 플레이어 관련 스탯
    public static float maxhp;
    public static float hp;
    public static float maxmp;
    public static float mp;

    public static float ad;
    public static float attackSpeed;

    public static float moveSpeed;
    public static float runSpeed;

    public static float jumpPower;

    // Start is called before the first frame update
    void Start()
    {
        Init();

        sm = GetComponent<SoundManager>();
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
            //@Managers 가 존재하는지 확인
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
}


