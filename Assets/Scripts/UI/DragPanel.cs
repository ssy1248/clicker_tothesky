using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragPanel : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static DragPanel Instance { get; private set; }

    [Header("스와이프 설정")]
    public float minSwipeDistance = 100f;

    private ChapterPanel currentTargetPanel;
    [SerializeField]
    private StageSelectPanel currentStagePanel;
    [SerializeField]
    private StagePanelManager stageManager;
    
    private Vector2 dragStartPosition;
    private Image raycastBlocker; // 이 패널의 Image 컴포넌트

    private void Awake()
    {
        if (Instance == null) { 
            Instance = this; 
        }
        else { 
            Destroy(gameObject); 
        }

        raycastBlocker = GetComponent<Image>();
        if (raycastBlocker == null) raycastBlocker = gameObject.AddComponent<Image>();
        raycastBlocker.raycastTarget = true;

        CanvasGroup cg = gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;              // 완전히 안 보이게
        cg.blocksRaycasts = true;   // 클릭/드래그는 받음
        cg.interactable = true;     // 이벤트 허용

        // 게임 시작 시에는 드래그 기능을 비활성화
        //DeactivateDrag();
    }

    public void ActivateDragChapterPanel(ChapterPanel targetPanel)
    {
        currentTargetPanel = targetPanel;
        currentStagePanel = null;   // 서로 배타적으로
        EnsureStageManager(null);
        SetRaycast(true);
    }

    public void DeactivateDragChapterPanel()
    {
        currentTargetPanel = null;
        MaybeDisableRaycast();
    }

    public void ActivateDragStagePanel(StageSelectPanel targetPanel)
    {
        currentStagePanel = targetPanel;
        currentTargetPanel = null;   // 서로 배타적으로
        // 매니저 참조 확보(인스펙터에서 넣어도 되고, 부모/씬에서 찾아도 됨)
        EnsureStageManager(targetPanel);
        SetRaycast(true);
    }

    public void DeactivateDragStagePanel()
    {
        currentStagePanel = null;
        MaybeDisableRaycast();
    }

    private void EnsureStageManager(StageSelectPanel fromPanel)
    {
        if (stageManager != null) return;

        if (fromPanel != null)
        {
            stageManager = fromPanel.GetComponentInParent<StagePanelManager>();
        }
        if (stageManager == null)
        {
            stageManager = FindObjectOfType<StagePanelManager>(true);
        }
    }

    private void SetRaycast(bool on)
    {
        if (raycastBlocker) raycastBlocker.raycastTarget = on;
    }

    // 둘 다 비활성화된 경우에만 막기
    private void MaybeDisableRaycast()
    {
        if (currentTargetPanel == null && currentStagePanel == null)
            SetRaycast(false);
    }

    /* ----------------------- 이벤트 핸들러 ----------------------- */

    public void OnPointerDown(PointerEventData eventData)
    {
        dragStartPosition = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 필요 시 로그
        // Debug.Log("드래그 시작");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 비워놔도 드래그 타깃 유지에 필요
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("드래그 끝");

        float verticalDistance = eventData.position.y - dragStartPosition.y;
        if (Mathf.Abs(verticalDistance) < minSwipeDistance) return;

        // 1) 챕터 화면인 경우
        if (currentTargetPanel != null)
        {
            if (verticalDistance > 0f) 
                currentTargetPanel.OnClickPrev();
            else 
                currentTargetPanel.OnClickNext();
            return;
        }

        // 2) 스테이지 선택 화면인 경우 (StagePanelManager 통해 호출)
        if (currentStagePanel != null && stageManager != null)
        {
            Debug.Log($"Stage drag 감지됨: {stageManager.name}");
            if (verticalDistance > 0f)
            {
                Debug.Log("이전 스테이지 실행");
                stageManager.ShowPreviousStage();
            }
            else
            {
                Debug.Log("다음 스테이지 실행");
                stageManager.ShowNextStage();
            }
        }
        else
        {
            Debug.Log($"Stage drag 실패: currentStagePanel={currentStagePanel}, stageManager={stageManager}");
        }
    }
}
