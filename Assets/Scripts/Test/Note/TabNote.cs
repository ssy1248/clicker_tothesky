using UnityEngine;
using UnityEngine.EventSystems;

public class TabNote : SubTouchNote, IPointerDownHandler
{
    [Header("연결할 오브젝트")]
    public Transform outlineCircle; // 크기가 줄어들 외곽선 원의 Transform

    [Header("노트 설정")]
    public float shrinkSpeed = 1.0f;    // 줄어드는 속도
    public float initialScale = 3.0f;   // 외곽선 원의 시작 크기
    private float targetScale = 1.0f;  // 목표 크기 (안쪽 원의 크기)

    [Header("판정 범위 (작은 값일수록 정확해야 함)")]
    public float perfectThreshold = 0.1f;
    public float greatThreshold = 0.3f;
    public float goodThreshold = 0.5f;

    void Start()
    {
        // 시작 시 외곽선 원의 크기를 초기 크기로 설정
        outlineCircle.localScale = new Vector3(initialScale, initialScale, 1f);
    }

    void Update()
    {
        // 매 프레임마다 외곽선 원의 크기를 줄임
        outlineCircle.localScale -= new Vector3(shrinkSpeed, shrinkSpeed, 0) * Time.deltaTime;

        // 만약 외곽선 원이 안쪽 원보다 작아지면 (시간이 지나서 놓침)
        if (outlineCircle.localScale.x < targetScale)
        {
            // 부모의 ReportResult 함수를 통해 'Miss' 판정을 보고합니다.
            ReportResult(Judgement.Miss);
        }
    }

    // 이 오브젝트(TargetCircle)가 터치되었을 때 호출되는 함수
    public void OnPointerDown(PointerEventData eventData)
    {
        float difference = outlineCircle.localScale.x - targetScale;
        Judgement judgement;

        // 판정 로직
        if (difference <= perfectThreshold)
        {
            judgement = Judgement.Perfect;
        }
        else if (difference <= greatThreshold)
        {
            judgement = Judgement.Great;
        }
        else if (difference <= goodThreshold)
        {
            judgement = Judgement.Good;
        }
        else
        {
            judgement = Judgement.Bad;
        }

        Debug.Log(judgement.ToString()); // 콘솔에 판정 결과 출력

        // 부모의 ReportResult 함수를 통해 최종 판정 결과를 보고합니다.
        ReportResult(judgement);
    }
}
