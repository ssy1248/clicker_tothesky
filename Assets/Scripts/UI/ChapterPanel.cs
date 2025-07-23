using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterPanel : BasePanel
{
    public override UIPanelType TypeOfPanel => UIPanelType.CHAPTER_PANEL;

    [Header("UI 요소 연결")]
    public TextMeshProUGUI chapterText;
    public Image chapterImage;
    public Image conditionFilledImage;
    public Button startButton;
    public Button nextButton;
    public Button prevButton;
    public Button backButton;

    [Header("챕터 데이터")]
    public List<Sprite> chapterImages; // 인스펙터에서 챕터 이미지들을 순서대로 할당
    public ChapterDatabase chapterDatabase; // 챕터 정보가 담긴 데이터베이스

    [SerializeField]
    private int currentChapterIndex = 0;

    private void Awake() // Start 대신 Awake 사용 권장
    {
        // 버튼 리스너는 한 번만 등록하면 됩니다.
        startButton.onClick.AddListener(OnClickStart);
        nextButton.onClick.AddListener(OnClickNext);
        prevButton.onClick.AddListener(OnClickPrev);
        backButton.onClick.AddListener(OnClickBack);
    }

    private void InitializeChapterConditions()
    {
        int totalChapters = chapterDatabase.allChapterData.Length;
        // GlobalVariable의 리스트 크기가 실제 챕터 수와 다르면, 새로 만들고 1로 채움
        if (GlobalVariable.Instance.chapterConditions.Count != totalChapters)
        {
            GlobalVariable.Instance.chapterConditions.Clear();
            for (int i = 0; i < totalChapters; i++)
            {
                GlobalVariable.Instance.chapterConditions.Add(1f); // 기본값은 1 (Full)
            }
        }
    }

    private void UpdateUI()
    {
        // 챕터 이미지 및 텍스트 업데이트
        chapterImage.sprite = chapterImages[currentChapterIndex];
        chapterText.text = $"{currentChapterIndex + 1} Chapter";

        // 컨디션 게이지 업데이트
        conditionFilledImage.fillAmount = GlobalVariable.Instance.chapterConditions[currentChapterIndex];
    }

    public void OnClickNext()
    {
        currentChapterIndex++;
        // 마지막 챕터를 넘어가면 처음 챕터로 순환
        if (currentChapterIndex >= chapterImages.Count)
        {
            currentChapterIndex = 0;
        }
        UpdateUI();
    }

    public void OnClickPrev()
    {
        // 첫 챕터(인덱스 0)에서는 아무 효과 없음
        if (currentChapterIndex > 0)
        {
            currentChapterIndex--;
            UpdateUI();
        }
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
            // 여기에 "잠김" 팝업 UI를 띄우는 로직을 추가할 수 있습니다.
            return;
        }

        // 이번 세션에서 아직 이 챕터를 시작한 적이 없을 때만 컨디션을 감소시킵니다.
        if (GlobalVariable.Instance.lastStartedChapter != currentChapterIndex)
        {
            // 1. 현재 챕터를 "이번에 시작한 챕터"로 기록합니다.
            GlobalVariable.Instance.lastStartedChapter = currentChapterIndex;

            // 2. 현재 챕터의 컨디션 값을 0.2 깎습니다.
            GlobalVariable.Instance.chapterConditions[currentChapterIndex] -= 0.2f;

            // 3. 다른 모든 챕터의 컨디션 값은 1로 초기화합니다.
            for (int i = 0; i < GlobalVariable.Instance.chapterConditions.Count; i++)
            {
                if (i != currentChapterIndex)
                {
                    GlobalVariable.Instance.chapterConditions[i] = 1f;
                }
            }
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

        InitializeChapterConditions();

        // GlobalVariable에서 "도전 가능한 최고 스테이지"의 통합 인덱스를 가져옵니다.
        int latestUnlockedFlatIndex = GlobalVariable.Instance.PlayerClearRound;

        // ChapterDatabase의 헬퍼 함수를 이용해 (챕터, 스테이지) 인덱스로 변환합니다.
        var unlockedIndices = chapterDatabase.GetChapterStageFromFlatIndex(latestUnlockedFlatIndex);

        // UI 초기화를 위해 현재 챕터 인덱스를 설정합니다.
        currentChapterIndex = unlockedIndices.chapter;

        UpdateUI();
    }

    public override void OnClose()
    {
        base.OnClose();
        this.gameObject.SetActive(false);
    }

    public void OnClickMemoryPanel()
    {
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.MEMORY_PANEL);
    }
}
