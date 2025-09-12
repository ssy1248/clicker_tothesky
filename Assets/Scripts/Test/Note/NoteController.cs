using UnityEngine;
using UnityEngine.EventSystems;

public class NoteController : MonoBehaviour, IPointerDownHandler
{
    [Header("연결할 오브젝트")]
    public Transform outlineCircle; // 크기가 줄어들 외곽선 원의 Transform

    [Header("노트 설정")]
    public float shrinkSpeed = 1.0f;    // 줄어드는 속도
    public float initialScale = 3.0f;   // 외곽선 원의 시작 크기
    private float targetScale = 1.0f; // 목표 크기 (안쪽 원의 크기)

    [Header("판정 범위 (작은 값일수록 정확해야 함)")]
    public float perfectThreshold = 0.1f; // Perfect 판정 범위 (목표 크기 + 0.1)
    public float greatThreshold = 0.3f;   // Great 판정 범위
    public float goodThreshold = 0.5f;    // Good 판정 범위

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
            Debug.Log("MISS"); // 판정: MISS
            Destroy(gameObject); // 노트 오브젝트 제거
        }
    }

    // 이 오브젝트(TargetCircle)가 터치되었을 때 호출되는 함수
    public void OnPointerDown(PointerEventData eventData)
    {
        // 현재 외곽선 크기와 목표 크기의 차이를 계산
        float difference = outlineCircle.localScale.x - targetScale;

        // 판정 로직
        if (difference <= perfectThreshold)
        {
            Debug.Log("PERFECT");
        }
        else if (difference <= greatThreshold)
        {
            Debug.Log("GREAT");
        }
        else if (difference <= goodThreshold)
        {
            Debug.Log("GOOD");
        }
        else
        {
            // 너무 빨리 눌렀을 경우
            Debug.Log("BAD");
        }

        // 판정이 끝나면 노트 오브젝트 제거
        Destroy(gameObject);
    }
}
