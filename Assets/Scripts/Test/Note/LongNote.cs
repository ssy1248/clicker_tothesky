using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongNote : SubTouchNote, IPointerDownHandler, IPointerUpHandler
{
    [Header("Long Note 전용")]
    [Tooltip("채워질 외곽선 이미지")]
    public Image circleOutline;
    [Tooltip("fillAmount가 1이 되기까지 걸리는 시간(초)")]
    public float fillDuration = 1.5f;

    private bool isHolding = false;

    private void Start()
    {
        // 시작할 때 외곽선은 비어있도록 초기화
        if (circleOutline != null)
        {
            circleOutline.fillAmount = 0;
        }
    }

    private void Update()
    {
        // 버튼을 꾹 누르고 있는 동안
        if (isHolding && circleOutline != null)
        {
            // 정해진 시간(fillDuration)에 걸쳐 fillAmount를 1까지 채움
            circleOutline.fillAmount += Time.deltaTime / fillDuration;

            // fillAmount가 1 이상이 되면 성공 처리
            if (circleOutline.fillAmount >= 1f)
            {
                Debug.Log("Perfect!");
                isHolding = false; // 중복 호출 방지
                ReportResult(Judgement.Perfect); // 부모에게 Perfect 성공을 알림
            }
        }
    }

    /// <summary>
    /// 이 UI 요소를 누르기 시작했을 때 호출됩니다.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down");
        isHolding = true;
    }

    /// <summary>
    /// 이 UI 요소에서 손을 뗐을 때 호출됩니다.
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Pointer Up");
        isHolding = false;

        // 손을 뗐는데 아직 fillAmount가 1이 안됐다면 실패 처리
        if (circleOutline != null && circleOutline.fillAmount < 1f)
        {
            Debug.Log("Failed!");
            ReportResult(Judgement.Bad); // 부모에게 실패를 알림
        }
    }
}
