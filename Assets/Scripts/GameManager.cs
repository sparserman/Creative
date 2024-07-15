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
            //@Managers �� �����ϴ��� Ȯ��
            GameObject go = GameObject.Find("GameManager");
            //������ ����
            if (go == null)
            {
                go = new GameObject { name = "GameManager" };
            }
            if (go.GetComponent<GameManager>() == null)
            {
                go.AddComponent<GameManager>();
            }
            //�������� �ʵ��� ����

            // �̰� ����� �� ��ȯ�� ��Ȱ����
            // (�� �ڵ带 Ȱ��ȭ �� ������ ���� �� �� �ʱ�ȭ�� �ȵǴ� ������ �־���)
            DontDestroyOnLoad(go);

            //instance �Ҵ�
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


