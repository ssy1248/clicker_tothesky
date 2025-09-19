using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragPanelManager : MonoBehaviour
{
    public static DragPanelManager Instance { get; private set; }

    [Header("스와이프 설정")]
    public float minSwipeDistance = 100f;

    private ChapterPanel currentTargetPanel;
    private Vector2 dragStartPosition;
    private Image raycastBlocker; // 이 패널의 Image 컴포넌트

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        raycastBlocker = GetComponent<Image>();
        // 게임 시작 시에는 드래그 기능을 비활성화
        DeactivateDrag();
    }

    /// <summary>
    /// ChapterPanel이 나타날 때 호출하여 드래그 기능을 활성화합니다.
    /// </summary>
    public void ActivateDrag(ChapterPanel targetPanel)
    {
        currentTargetPanel = targetPanel;
        // Image 컴포넌트의 raycastTarget을 켜서 드래그를 감지할 수 있게 함
        raycastBlocker.raycastTarget = true;
    }

    /// <summary>
    /// 다른 패널이 나타날 때 호출하여 드래그 기능을 비활성화합니다.
    /// </summary>
    public void DeactivateDrag()
    {
        currentTargetPanel = null;
        // raycastTarget을 꺼서 다른 버튼들의 클릭을 방해하지 않도록 함
        raycastBlocker.raycastTarget = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentTargetPanel != null)
        {
            dragStartPosition = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentTargetPanel == null) return;

        float verticalDistance = eventData.position.y - dragStartPosition.y;

        if (verticalDistance > minSwipeDistance)
        {
            currentTargetPanel.OnClickPrev();
        }
        else if (verticalDistance < -minSwipeDistance)
        {
            currentTargetPanel.OnClickNext();
        }
    }
}
