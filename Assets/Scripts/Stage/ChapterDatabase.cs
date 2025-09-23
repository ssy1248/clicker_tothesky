using UnityEngine;
using System.Collections.Generic;

// 어떤 미니게임이 있는지 종류를 정의합니다.
public enum SubTouchType
{
    None,       // 미니게임 없음
    QuickTap,   // 빠르게 탭하기 -> 체육
    Touch,      // 화면 터치하기 -> 국어
    Memory,     // 기억력 게임 -> 수학
    // ... 나중에 새로운 미니게임 추가 ...
}

[System.Serializable]
public struct MiniGameConfig
{
    public SubTouchType type;       // 미니게임 종류
    public int successScore;        // 성공 점수
    public float holdingTime;      // 미니게임 유지 시간(초). 0이 되면 실패
}

[System.Serializable]
public struct StageData
{
    public string stageName; // 스테이지 이름
    public Sprite stageSprite; // 스테이지 이미지
    public float gameTime; // 게임 시간 (초 단위)
    public int clearScore; // 클리어 점수
    public int collectScore; // 수집품 획득 점수

    [Header("서브터치 설정")]
    public List<MiniGameConfig> miniGames;
    public int miniGameSpawnCount;      // 스테이지 당 등장 횟수
    public float miniGameSpawnPercentage; // 스테이지 당 등장 확률 (0~100)
}

[System.Serializable]
public class ChapterData
{
    public string chapterName;
    public Sprite ChapterBackgroundImage; // 챕터 배경 이미지
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
