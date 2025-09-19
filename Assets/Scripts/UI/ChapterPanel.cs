using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChapterPanel : BasePanel
{
    public override UIPanelType TypeOfPanel => UIPanelType.CHAPTER_PANEL;

    [Header("UI 요소 연결")]
    public Image background; // 배경 이미지
    public Image chapterImage;
    //public Image conditionFilledImage;
    public Button startButton;
    public Button nextButton;
    public Button prevButton;
    public Button backButton;
    // 텍스트필드 추가 해서 나중에 챕터 이름들 나오면 챕터 데이터베이스에 이름 추가해서 연결

    [Header("챕터 데이터")]
    public ChapterDatabase chapterDatabase; // 챕터 정보가 담긴 데이터베이스

    [SerializeField]
    private int currentChapterIndex = 0;

    private void UpdateUI()
    {
        // 1. 현재 챕터 데이터를 데이터베이스에서 직접 가져옴
        ChapterData currentChapter = chapterDatabase.allChapterData[currentChapterIndex];

        // 2. 챕터 이미지를 업데이트
        background.sprite = currentChapter.ChapterBackgroundImage;
        chapterImage.sprite = currentChapter.chapterImage;

        // 3. 컨디션 게이지 업데이트
        //conditionFilledImage.fillAmount = GlobalVariable.Instance.chapterConditions[currentChapterIndex];
    }

    public void OnClickNext()
    {
        currentChapterIndex++;
        // chapterDatabase의 챕터 개수를 기준
        if (currentChapterIndex >= chapterDatabase.allChapterData.Length)
        {
            currentChapterIndex = 0; // 마지막 챕터에서 처음으로 순환
        }

        Debug.Log($"Next 버튼 클릭. 현재 챕터 인덱스: {currentChapterIndex}");

        UpdateUI();
    }

    public void OnClickPrev()
    {
        currentChapterIndex--;
        // 첫 챕터에서 누르면 마지막 챕터로 순환하도록 변경
        if (currentChapterIndex < 0)
        {
            currentChapterIndex = chapterDatabase.allChapterData.Length - 1;
        }

        Debug.Log($"Prev 버튼 클릭. 현재 챕터 인덱스: {currentChapterIndex}");

        UpdateUI();
    }

    public void OnClickStart()
    {
        // 1. 잠금 여부 확인 (도전 가능한 최고 스테이지는 PlayerClearRound에 저장됨)
        // PlayerClearRound를 (챕터,스테이지)로 변환하여 현재 챕터가 잠겼는지 확인해야 함
        // 여기서는 간단하게, "총 클리어한 스테이지 수"가 "현재 챕터까지 필요한 스테이지 수"보다 적은지 확인
        int requiredStages = 0;
        for (int i = 0; i < currentChapterIndex; i++)
        {
            requiredStages += chapterDatabase.allChapterData[i].stagesInChapter.Length;
        }

        if (requiredStages > GlobalVariable.Instance.PlayerClearRound)
        {
            Debug.Log("이전 챕터를 클리어해주세요");
            GameObject.Find("PopUpUIManager").GetComponent<PopUpUIManager>().AlertPopUpUIShow();
            return;
        }

        // 이번 세션에서 아직 이 챕터를 시작한 적이 없을 때만 컨디션을 감소시킵니다.
        if (GlobalVariable.Instance.lastStartedChapter != currentChapterIndex)
        {
            // 현재 챕터를 "이번에 시작한 챕터"로 기록합니다.
            GlobalVariable.Instance.lastStartedChapter = currentChapterIndex;
        }

        // 현재 패널 닫고 StagePanel 열기
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.STAGE_PANEL, currentChapterIndex);
    }

    public void OnClickBack()
    {
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.LOGO_PANEL);
    }

    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);
        this.gameObject.SetActive(true);
        DragPanel.Instance?.ActivateDragChapterPanel(this);

        // GlobalVariable에서 "도전 가능한 최고 스테이지"의 통합 인덱스를 가져옵니다.
        int latestUnlockedFlatIndex = GlobalVariable.Instance.PlayerClearRound;

        // ChapterDatabase의 헬퍼 함수를 이용해 (챕터, 스테이지) 인덱스로 변환합니다.
        var unlockedIndices = chapterDatabase.GetChapterStageFromFlatIndex(latestUnlockedFlatIndex);

        // UI 초기화를 위해 현재 챕터 인덱스를 설정합니다.
        currentChapterIndex = unlockedIndices.chapter;

        UpdateUI();
    }

    public override void OnResume()
    {
        base.OnResume();
        DragPanel.Instance?.ActivateDragChapterPanel(this);
    }

    public override void OnPause()
    {
        DragPanel.Instance?.DeactivateDragChapterPanel();
        base.OnPause();
    }

    public override void OnClose()
    {
        DragPanel.Instance?.DeactivateDragChapterPanel();
        base.OnClose();
        this.gameObject.SetActive(false);
    }

    public void OnClickSettingBtn()
    {
        // 설정 패널 열기
        GameObject.Find("PopUpUIManager").GetComponent<PopUpUIManager>().SettingPopUpUIShow();
    }

    public void OnClickMemoryPanel()
    {
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.MEMORY_PANEL);
    }
}
