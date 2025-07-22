using UnityEngine;
using System.Collections.Generic;

// StageData 구조체는 그대로 사용합니다.
[System.Serializable]
public struct StageData
{
    public string stageName;
    public Sprite stageSprite;
    public float gameTime;
    public int clearDistance;
    public int clearScore;
    public int collectScore;
}

[System.Serializable]
public class ChapterData
{
    public string chapterName; // 예: "1학년", "2학년"
    // 각 챕터가 여러 개의 스테이지 데이터를 가집니다.
    public StageData[] stagesInChapter;
}

[CreateAssetMenu(fileName = "ChapterDatabase", menuName = "Game Data/Chapter Database")]
public class ChapterDatabase : ScriptableObject
{
    public ChapterData[] allChapterData;
}

// 엑스트라 타임이 모든 스테이지에 적용이 되는지
