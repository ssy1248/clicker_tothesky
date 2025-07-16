using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryPanel : BasePanel
{
    public override UIPanelType TypeOfPanel => UIPanelType.MEMORY_PANEL;

    [Header("UI 요소 연결")]
    public Image[] memorySlots = new Image[6];  // 수집품 이미지를 표시할 6개의 UI Image 슬롯
    public TextMeshProUGUI collectNameText;     // 수집품 이름을 표시할 텍스트
    public TextMeshProUGUI collectDescText;     // 수집품 설명을 표시할 텍스트
    public Button nextPageButton;
    public Button prevPageButton;

    [Header("데이터 및 리소스")]
    public StageDatabase stageDatabase; // 모든 스테이지 정보가 담긴 데이터베이스
    public Sprite unknownItemSprite;    // 아직 수집하지 못한 아이템을 표시할 '?' 스프라이트

    public string unknownItemName;
    public string unknownItemDescription;

    // 정렬된 전체 수집품 목록을 담을 리스트
    private List<CollectScriptableObject> masterCollectibleList;
    private int currentPage = 0;
    private int maxPages = 0;

    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);
        this.gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        this.gameObject.SetActive(false);
    }

    private void Start()
    {
        InitializeCollection();
        UpdateDisplay();

        // 버튼에 리스너 연결
        nextPageButton.onClick.AddListener(ShowNextPage);
        prevPageButton.onClick.AddListener(ShowPreviousPage);
    }

    /// <summary>
    /// StageDatabase를 기반으로 모든 수집품의 마스터 리스트를 생성하고 정렬합니다.
    /// </summary>
    private void InitializeCollection()
    {
        masterCollectibleList = new List<CollectScriptableObject>();

        // StageDatabase의 모든 스테이지를 순회하며
        foreach (StageData stage in stageDatabase.allStageData)
        {
            // 각 스테이지에 포함된 모든 수집품을 마스터 리스트에 추가
            masterCollectibleList.AddRange(stage.collectiblesInStage);
        }

        // 전체 페이지 수 계산 (한 페이지에 6개 기준)
        if (masterCollectibleList.Count > 0)
        {
            maxPages = (masterCollectibleList.Count - 1) / 6;
        }
    }

    /// <summary>
    /// 현재 페이지에 맞게 UI를 업데이트합니다.
    /// </summary>
    private void UpdateDisplay()
    {
        // 6개의 UI 슬롯을 순회
        for (int i = 0; i < memorySlots.Length; i++)
        {
            int itemIndex = currentPage * 6 + i;

            // 해당 인덱스가 실제 아이템 목록 범위 안에 있는지 확인
            if (itemIndex < masterCollectibleList.Count)
            {
                memorySlots[i].gameObject.SetActive(true);

                // 1. 표시할 아이템 데이터를 가져옵니다.
                CollectScriptableObject itemData = masterCollectibleList[itemIndex];

                // 2. 이 아이템을 플레이어가 수집했는지 확인합니다.
                bool isCollected = GlobalVariable.Instance.collectedItems.Exists(ci => ci.itemId == itemData.CollectId);

                // 3. 수집 여부에 따라 스프라이트를 다르게 표시합니다.
                if (isCollected)
                {
                    memorySlots[i].sprite = itemData.CollectSprite;
                }
                else
                {
                    memorySlots[i].sprite = unknownItemSprite;
                }
            }
            else
            {
                // 해당 슬롯에 표시할 아이템이 없으면 비활성화
                memorySlots[i].gameObject.SetActive(false);
            }
        }

        // 페이지 버튼 활성화/비활성화
        prevPageButton.interactable = (currentPage > 0);
        nextPageButton.interactable = (currentPage < maxPages);
    }

    public void ShowNextPage()
    {
        if (currentPage < maxPages)
        {
            currentPage++;
            UpdateDisplay();
        }
    }

    public void ShowPreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateDisplay();
        }
    }

    public void ReturnButton()
    {
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.STAGE_PANEL);
    }
}
