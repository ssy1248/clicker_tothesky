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
    // 각 챕터가 여러 개의 스테이지 데이터를 가집니다.
    public StageData[] stagesInChapter;
}

[CreateAssetMenu(fileName = "ChapterDatabase", menuName = "Game Data/Chapter Database")]
public class ChapterDatabase : ScriptableObject
{
    public ChapterData[] allChapterData;
}
