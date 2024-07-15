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

    // Start is called before the first frame update
    void Start()
    {
        Init();

        sm = GetComponent<SoundManager>();
    }

    void Update()
    {
        InputSystem();
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
        goList.LastOrDefault().GetComponent<Animator>().SetTrigger("Off");
        goList.RemoveAt(goList.Count - 1);
        sm.PlayEffectSound(sm.click);
    }
}


