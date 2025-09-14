using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliceNote : SubTouchNote, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Slice Note 전용")]
    [Tooltip("잘려나갈 위쪽 부분의 Transform")]
    public Transform topHalf;
    [Tooltip("잘려나갈 아래쪽 부분의 Transform")]
    public Transform bottomHalf;

    [Header("판정 설정")]
    [Tooltip("성공으로 인정될 최소 드래그 거리 (픽셀)")]
    public float minSliceDistance = 50f;

    [Header("애니메이션 설정")]
    [Tooltip("잘려나갈 때 각 부분이 이동할 거리")]
    public float sliceMoveAmount = 100f;
    [Tooltip("잘려나가는 애니메이션 지속 시간(초)")]
    public float animationDuration = 0.3f;

    private Vector2 startDragPosition;
    private bool isSliced = false; // 중복 성공 방지 플래그

    /// <summary>
    /// 드래그를 시작했을 때 호출됩니다.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 아직 슬라이스되지 않았다면 드래그 시작 위치를 기록합니다.
        if (!isSliced)
        {
            startDragPosition = eventData.position;
        }
    }

    /// <summary>
    /// 드래그하는 동안 계속 호출됩니다.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // 아직 슬라이스되지 않았고, 이 컴포넌트가 활성화 상태일 때만 판정합니다.
        if (!isSliced && this.enabled)
        {
            // 시작 위치로부터의 수평 이동 거리를 계산합니다.
            float dragDistance = Mathf.Abs(eventData.position.x - startDragPosition.x);

            // 이동 거리가 성공 기준을 넘었으면
            if (dragDistance >= minSliceDistance)
            {
                isSliced = true; // 성공 플래그를 켜서 중복 실행을 막습니다.

                // 추가적인 드래그 이벤트를 막기 위해 이 컴포넌트를 비활성화합니다.
                this.enabled = false;

                // 잘려나가는 애니메이션 코루틴을 시작합니다.
                StartCoroutine(SliceAnimationCoroutine());
            }
        }
    }

    /// <summary>
    /// 드래그를 끝냈을 때(손을 뗐을 때) 호출됩니다.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // 손을 뗐는데 아직 슬라이스되지 않았다면 실패입니다.
        if (!isSliced)
        {
            ReportResult(Judgement.Miss);
        }
    }

    /// <summary>
    /// 양쪽으로 잘려나가는 애니메이션을 처리하는 코루틴
    /// </summary>
    private IEnumerator SliceAnimationCoroutine()
    {
        Vector3 topOriginalPos = topHalf.localPosition;
        Vector3 bottomOriginalPos = bottomHalf.localPosition;

        // 목표 위치 계산 (하나는 왼쪽, 하나는 오른쪽으로)
        Vector3 topTargetPos = topOriginalPos + new Vector3(-sliceMoveAmount, 0, 0);
        Vector3 bottomTargetPos = bottomOriginalPos + new Vector3(sliceMoveAmount, 0, 0);

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            // 시간에 따라 부드럽게 위치를 보간(Lerp)합니다.
            topHalf.localPosition = Vector3.Lerp(topOriginalPos, topTargetPos, elapsedTime / animationDuration);
            bottomHalf.localPosition = Vector3.Lerp(bottomOriginalPos, bottomTargetPos, elapsedTime / animationDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 애니메이션이 끝난 후, 'Perfect' 판정을 보고합니다.
        ReportResult(Judgement.Perfect);
    }
}
