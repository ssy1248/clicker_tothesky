using NUnit.Framework;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StagePanelManager : MonoBehaviour
{
    [Header("UI 요소 연결")]
    public Image stageRoundNumberImage; // 월(1, 2, 3...)을 표시할 Image 컴포넌트

    [Header("챕터 데이터베이스")]
    public ChapterDatabase chapterDatabase;

    [Header("스테이지 데이터")]
    private int currentChapterIndex = 0;
    private int currentStageIndex = 0;

    // 챕터 패널에서 넘겨온 챕터를 임시로 저장
    private int? _chapterOverride;

    void Start()
    {
        ResetToProgress();
    }

    void OnEnable()
    {
        // 패널이 다시 활성화될 때마다 반드시 리셋
        ResetToProgress(_chapterOverride);
        _chapterOverride = null; // 한 번 쓰고 비움
    }

    public void SetOverrideChapterFromCaller(int chapterIndex)
    {
        _chapterOverride = Mathf.Clamp(chapterIndex, 0, chapterDatabase.allChapterData.Length - 1);
    }

    public void ResetToProgress(int? chapterOverride = null)
    {
        // 1) 기본은 유저 진행도 기준
        int latestFlat = GlobalVariable.Instance.PlayerClearRound;
        var (chapter, stage) = GetChapterStageFromFlatIndex(latestFlat);

        // 2) 챕터 패널이 넘긴 챕터가 있으면 그 챕터의 0번 스테이지로 강제 설정
        if (chapterOverride.HasValue)
        {
            chapter = chapterOverride.Value;
            stage = 0;
        }

        currentChapterIndex = chapter;
        currentStageIndex = stage;

        UpdateStageUI();
    }

    // 통합 인덱스를 (챕터, 스테이지) 튜플로 변환하는 헬퍼 함수
    private (int chapter, int stage) GetChapterStageFromFlatIndex(int flatIndex)
    {
        if (chapterDatabase == null) return (0, 0);

        int accumulatedStages = 0;
        for (int i = 0; i < chapterDatabase.allChapterData.Length; i++)
        {
            int stagesInThisChapter = chapterDatabase.allChapterData[i].stagesInChapter.Length;
            if (flatIndex < accumulatedStages + stagesInThisChapter)
            {
                return (i, flatIndex - accumulatedStages);
            }
            accumulatedStages += stagesInThisChapter;
        }

        // 모든 스테이지를 클리어한 경우, 마지막 챕터의 마지막 스테이지를 반환
        int lastChapter = chapterDatabase.allChapterData.Length - 1;
        int lastStage = chapterDatabase.allChapterData[lastChapter].stagesInChapter.Length - 1;
        return (lastChapter, lastStage);
    }

    public void ShowNextStage()
    {
        currentStageIndex++;
        // 현재 챕터의 스테이지 개수를 넘어갔는지 확인
        if (currentStageIndex >= chapterDatabase.allChapterData[currentChapterIndex].stagesInChapter.Length)
        {
            currentStageIndex = 0; // 스테이지 인덱스는 0으로 리셋
            currentChapterIndex++; // 다음 챕터로 이동

            // 챕터 인덱스가 전체 챕터 개수를 넘어가면 처음으로 순환
            if (currentChapterIndex >= chapterDatabase.allChapterData.Length)
            {
                currentChapterIndex = 0;
            }
        }
        UpdateStageUI();
    }

    public void ShowPreviousStage()
    {
        currentStageIndex--;
        // 현재 스테이지 인덱스가 0보다 작은지 확인
        if (currentStageIndex < 0)
        {
            currentChapterIndex--; // 이전 챕터로 이동

            // 챕터 인덱스가 0보다 작아지면 마지막 챕터로 순환
            if (currentChapterIndex < 0)
            {
                currentChapterIndex = chapterDatabase.allChapterData.Length - 1;
            }
            // 이전 챕터의 마지막 스테이지로 인덱스 설정
            currentStageIndex = chapterDatabase.allChapterData[currentChapterIndex].stagesInChapter.Length - 1;
        }
        UpdateStageUI();
    }

    // UI를 업데이트하는 함수
    private void UpdateStageUI()
    {
        // 1. 현재 챕터 데이터를 가져옵니다.
        ChapterData currentChapter = chapterDatabase.allChapterData[currentChapterIndex];
        // 2. 현재 챕터에서 현재 스테이지 데이터를 가져옵니다.
        StageData currentStage = currentChapter.stagesInChapter[currentStageIndex];

        // 3. UI에 반영합니다.
        //chapterNameText.text = currentChapter.chapterName;
        stageRoundNumberImage.sprite = currentStage.stageSprite;
    }

    // "START" 버튼을 눌렀을 때 호출될 함수
    public void StartGame()
    {
        // 현재 (챕터, 스테이지)를 통합 인덱스로 변환
        int selectedFlatIndex = GetFlatIndexFromChapterStage(currentChapterIndex, currentStageIndex);

        // 잠금 여부 확인
        if (selectedFlatIndex > GlobalVariable.Instance.PlayerClearRound)
        {
            Debug.Log("이 스테이지는 아직 잠겨있습니다!");
            PopUpUIManager.Instance.AlertPopUpUIShow("이전 스테이지를 클리어해주세요", 34);
            return;
        }

        // 스테이지 정보 세팅 (통합 인덱스를 넘겨줌)
        GlobalVariable.Instance.SetupStage(selectedFlatIndex, chapterDatabase); // db 타입도 변경

        SceneManager.LoadScene("ShopScene");
    }

    // (챕터, 스테이지)를 통합 인덱스로 변환하는 헬퍼 함수
    private int GetFlatIndexFromChapterStage(int chapter, int stage)
    {
        int flatIndex = 0;
        for (int i = 0; i < chapter; i++)
        {
            flatIndex += chapterDatabase.allChapterData[i].stagesInChapter.Length;
        }
        flatIndex += stage;
        return flatIndex;
    }
}

