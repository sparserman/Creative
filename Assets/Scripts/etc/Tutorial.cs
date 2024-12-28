using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public List<DialogueSystem> dialogSystemList;
    public int index;

    public int selectBranch = 1000;

    public bool nextChapter = false;

    IEnumerator Start()
    {
        // dialogueSystem에서 Tutorial을 관리하기위해 저장
        for(int i = 0 ; i < dialogSystemList.Count; i++)
        {
            dialogSystemList[i].SetDialogController(gameObject);
        }

        // 모든 대화가 끝날때까지 루프
        while(index < dialogSystemList.Count)
        {
            for (int i = 0; i < dialogSystemList[index].DBNameList.Count; i++)
            {
                dialogSystemList[index].CharacterSetting(dialogSystemList[index].DBNameList[i]);
            }
            // 첫 대사 시작 및 모든 대사 끝날 때까지 대기
            yield return new WaitUntil(() => dialogSystemList[index].UpdateDialog());

            int loopNum = PlusIndex();

            while (true)
            {
                if(index >= dialogSystemList.Count)
                {
                    // 마지막 대사가 끝나고 1초 후 (패널 사라지는 애니메이션 대기시간)
                    yield return new WaitForSeconds(1f);
                    // 패널끄기
                    dialogSystemList[0].dialogPanel.SetActive(false);
                    // 시간 흐르게하기
                    GameManager.GetInstance().timerOn = true;
                    break;
                }

                // 다음 브렌치 대화 진행 대기
                yield return new WaitUntil(() => dialogSystemList[index - loopNum].UpdateDialog());

                break;
            }
        }
    }

    // branch값이 1000단위로 이동하기
    int PlusIndex()
    {
        int loop = 1;
        while (index < dialogSystemList.Count)
        {
            // 현재 대화가 선택지로 빠지려하거나 빠진 대화가 아니라면
            if (dialogSystemList[index].branch == selectBranch)
            {
                selectBranch -= selectBranch % 1000;
                selectBranch += 1000;
                return loop;
            }
            else if (index + 1 < dialogSystemList.Count)
            {
                if (dialogSystemList[++index].branch == selectBranch)
                {
                    selectBranch -= selectBranch % 1000;
                    selectBranch += 1000;
                    return loop;
                }
            }
            else
            {
                index++;
                return loop;
            }

            loop++;
        }

        return loop;
    }
}
