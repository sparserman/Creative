using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum STAGE
{
    MAIN = 0,
    LOBBY,
    WORLD_A = 11,
    WORLD_B
}

public class LoadingSceneManager : MonoBehaviour
{
    GameManager gm;

    public static string nextScene;

    public bool isLoading = true;
    [SerializeField] Slider progressBar;

    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI loadingText2;
    public Slider loadingSlider;

    private float alpha = -1f;
    private float value;
    private bool isTouchMsgActive = false;
    private bool isLoadingMsgActive = true;

    private List<string> loadingTextList = new List<string>();
    private int count = 0;
    public float curTime = 0;
    public float speed = 5f;

    float curFadeTime = 0;
    public GameObject fadeBG;

    // 바로 넘어가려면 true, 클릭 후 넘어가려면 false  (로딩창에 LoadingEnd_Txt도 수정해야함)
    bool isClick = true;

    AsyncOperation op;

    bool fadeFlag = false;
    int nextStageNum = 0;

    public static int currentStage = (int)STAGE.MAIN;

    private void Start()
    {
        gm = GameManager.GetInstance();
        gm.lm = this;

        if (isLoading)
        {
            StartCoroutine(LoadScene());
            loadingTextList.Add("Loading.");
            loadingTextList.Add("Loading..");
            loadingTextList.Add("Loading...");
        }
    }

    private void Update()
    {
        if (isLoading)
        {
            LoadingMsgAnim();
            TouchMsgAnim();

            LoadingClick();
        }

        FadeIn();
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }

    public void StartFadeIn(int num)
    {
        fadeFlag = true;
        nextStageNum = num;
        gm.goList.Clear();
        GameManager.GetInstance().timerOn = false;
    }

    public static void NowStage(int num)
    {
        switch (num)
        {
            case (int)STAGE.MAIN:
                currentStage = num;
                LoadScene("Mainmenu");
                break;
            case (int)STAGE.LOBBY:
                currentStage = num;
                LoadScene("Lobby");
                break;
            case (int)STAGE.WORLD_A:
                currentStage = num;
                LoadScene("World_A");
                break;
            case (int)STAGE.WORLD_B:
                currentStage = num;
                LoadScene("World_B");
                break;
        }
    }


    IEnumerator LoadScene()
    {
        yield return null;
        op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                progressBar.value = Mathf.Lerp(progressBar.value, op.progress, timer);
                if (progressBar.value >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.value = Mathf.Lerp(progressBar.value, 1f, timer);
                if (progressBar.value == 1.0f)
                {
                    isTouchMsgActive = true;
                    isLoadingMsgActive = false;
                    loadingText2.gameObject.SetActive(isLoadingMsgActive);
                    loadingSlider.gameObject.SetActive(false);
                    loadingText.gameObject.SetActive(isTouchMsgActive);
                    if (isClick)
                    {
                        fadeBG.gameObject.SetActive(true);
                        StartCoroutine(FadeInCoroutine());
                        yield break;
                   }
                }
            }
        }
    }

    private void LoadingClick()
    {
        if (isTouchMsgActive)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isClick = true;
            }
        }
    }

    void TouchMsgAnim()
    {
        if (isTouchMsgActive)
        {
            if (loadingText.color.a == 0)
            {
                alpha *= -1;
            }
            else if (loadingText.color.a == 1)
            {
                alpha *= -1;
            }
            value += alpha * Time.deltaTime;
            loadingText.color = Color.Lerp(new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), value);
        }
    }

    void LoadingMsgAnim()
    {
        if (isLoadingMsgActive)
        {
            curTime += Time.deltaTime * speed;
            if (curTime >= 1f)
            {
                loadingText2.text = loadingTextList[count++];
                if (count >= loadingTextList.Count)
                {
                    count = 0;
                }
                curTime = 0;
            }
        }
    }

    IEnumerator FadeInCoroutine()
    {
        while (true)
        {
            yield return null;
            curFadeTime += Time.deltaTime;
            fadeBG.GetComponent<Image>().color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), curFadeTime);
            if (curFadeTime >= 1)
            {
                curFadeTime = 0;
               
                op.allowSceneActivation = true;
                yield break;
            }
        }
    }

    void FadeIn()
    {
        if (fadeFlag)
        {
            curFadeTime += Time.deltaTime;

            float r = fadeBG.GetComponent<Image>().color.r;
            float g = fadeBG.GetComponent<Image>().color.g;
            float b = fadeBG.GetComponent<Image>().color.b;

            fadeBG.GetComponent<Image>().color = Color.Lerp(new Color(r, g, b, 0), new Color(r, g, b, 1), curFadeTime);
            if (curFadeTime >= 1)
            {
                curFadeTime = 0;
                NowStage(nextStageNum);
            }
        }
    }
}
