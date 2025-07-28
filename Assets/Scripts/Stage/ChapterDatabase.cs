using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct StageData
{
    public string stageName; // 스테이지 이름
    public Sprite stageSprite; // 스테이지 이미지
    public float gameTime; // 게임 시간 (초 단위)
    public int clearDistance; // 클리어 거리
    public int clearScore; // 클리어 점수
    public int collectScore; // 수집품 획득 점수
}

[System.Serializable]
public class ChapterData
{
    public string chapterName;
    public Sprite chapterImage; // 챕터 이미지
    // 각 챕터가 여러 개의 스테이지 데이터를 가집니다.
    public StageData[] stagesInChapter;
}

[CreateAssetMenu(fileName = "ChapterDatabase", menuName = "Game Data/Chapter Database")]
public class ChapterDatabase : ScriptableObject
{
    public ChapterData[] allChapterData;

    public (int chapter, int stage) GetChapterStageFromFlatIndex(int flatIndex)
    {
        if (allChapterData == null) return (-1, -1);

        int accumulatedStages = 0;
        for (int i = 0; i < allChapterData.Length; i++)
        {
            int stagesInThisChapter = allChapterData[i].stagesInChapter.Length;
            if (flatIndex < accumulatedStages + stagesInThisChapter)
            {
                return (i, flatIndex - accumulatedStages);
            }
            accumulatedStages += stagesInThisChapter;
        }

        // 모든 스테이지를 클리어한 경우, 마지막 챕터의 마지막 스테이지를 반환
        if (allChapterData.Length > 0)
        {
            int lastChapter = allChapterData.Length - 1;
            int lastStage = allChapterData[lastChapter].stagesInChapter.Length - 1;
            return (lastChapter, lastStage);
        }

        return (-1, -1);
    }
}
