using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public List<DialogueSystem> dialogSystemList;
    public int index;

    public bool nextChapter = false;

    IEnumerator Start()
    {
        while(index < dialogSystemList.Count)
        {
            for (int i = 0; i < dialogSystemList[index].DBNameList.Count; i++)
            {
                dialogSystemList[index].CharacterSetting(dialogSystemList[index].DBNameList[i]);
            }
            // 첫 대사 시작 및 모든 대사 끝날 때까지 대기
            yield return new WaitUntil(() => dialogSystemList[index].UpdateDialog());

            // 튜토리얼 진행할 지 확인
            nextChapter = dialogSystemList[index].nextChapter;

            index++;

            while(true)
            {
                if(index < dialogSystemList.Count)
                {
                    // 마지막 대사가 끝나고 1초 후
                    yield return new WaitForSeconds(1f);
                    // 패널끄기
                    dialogSystemList[index].dialogPanel.SetActive(false);
                    // 시간 흐르게하기
                    GameManager.GetInstance().timerOn = true;
                    break;
                }

                if(nextChapter)
                {
                    nextChapter = false;
                    break;
                }

                yield return null;
            }

            // 초기화
            dialogSystemList[index].nextCheck = false;
            dialogSystemList[index].nextChapter = false;
        }
    }


}
