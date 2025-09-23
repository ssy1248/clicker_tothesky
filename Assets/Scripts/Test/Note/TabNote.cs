using UnityEngine;
using UnityEngine.EventSystems;

public class TabNote : SubTouchNote, IPointerDownHandler
{
    [Header("연결할 오브젝트")]
    public Transform outlineCircle; // 크기가 줄어들 외곽선 원의 Transform

    [Header("노트 설정")]
    public float shrinkSpeed = 1.0f;    // 줄어드는 속도
    public float initialScale = 4.5f;   // 외곽선 원의 시작 크기
    private float targetScale = 1.5f;  // 목표 크기 (안쪽 원의 크기)

    [Header("시간 판정(ms)")]
    public float perfectMs = 100f;
    public float goodMs = 200f;

    [Header("루프 설정")]
    [Tooltip("진행할 사이클 수(0 이하면 무한 루프)")]
    public int cyclesToPlay = 0;              // 0 or 음수면 무한
    [Tooltip("사이클 사이 잠깐 멈춤(연출용)")]
    public float interCycleDelay = 0.05f;

    // 내부
    float spawnTime;        // 생성 시각
    float targetMoment;     // targetScale에 도달하는 이상적 시각(초)
    int cycleIndex = 0;
    bool activeCycle = false;                // 사이클 진행 중인지
    bool missTriggeredThisCycle = false;     // 중복 Miss 방지

    // 외부 매니저 참조(피버/점수 적용용)
    GuageManager gauge;     // 피버 게이지용
    ScoreManager score;     // 점수
    ComboManager combo;   // 콤보

    void Awake()
    {
        gauge = FindFirstObjectByType<GuageManager>();
        score = FindFirstObjectByType<ScoreManager>();
        combo = FindFirstObjectByType<ComboManager>();
    }

    void OnEnable()
    {
        StartNextCycle(immediate: true);
    }

    void Update()
    {
        if (!activeCycle) 
            return;

        // 스케일 감소
        var s = outlineCircle.localScale;
        s -= new Vector3(shrinkSpeed, shrinkSpeed, 0) * Time.deltaTime;
        outlineCircle.localScale = s;

        // target 이후 goodMs가 지나면 자동 Miss
        if (!missTriggeredThisCycle && Time.time - targetMoment > goodMs / 1000f)
        {
            missTriggeredThisCycle = true;
            ApplyJudgement(Judgement.Miss);
        }
    }

    // 이 오브젝트(TargetCircle)가 터치되었을 때 호출되는 함수
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!activeCycle) 
            return;

        // 현재 클릭 시간과 이상적 시각 차이(절대값)
        float delta = Mathf.Abs(Time.time - targetMoment); // 초
        float deltaMs = delta * 1000f;

        Judgement j;
        if (deltaMs <= perfectMs) 
            j = Judgement.Perfect;
        else if (deltaMs <= goodMs) 
            j = Judgement.Good;
        else 
            j = Judgement.Miss;

        ApplyJudgement(j);
    }

    void ApplyJudgement(Judgement j)
    {
        // 점수 / 피버 적용
        switch (j)
        {
            case Judgement.Perfect:
                score?.AddScore(100);
                gauge?.AddFeverPercent(0.02f); // +2%
                combo?.AddCombo();
                break;
            case Judgement.Good:
                score?.AddScore(70);
                gauge?.AddFeverPercent(0.01f); // +1%
                combo?.AddCombo();
                break;
            default: // Miss
                score?.AddScore(-30);
                gauge?.AddFeverPercent(-0.04f); // -4%
                combo?.ResetCombo();
                break;
        }

        // 부모에 보고(필요하면 유지)
        ReportResult(j);

        // 다음 사이클로
        activeCycle = false;
        Invoke(nameof(StartNextCycle), interCycleDelay);
    }

    void StartNextCycle() => StartNextCycle(immediate: false);

    void StartNextCycle(bool immediate)
    {
        // 사이클 종료 수 제한 체크 (0 이하면 무한 루프)
        if (cyclesToPlay > 0 && cycleIndex >= cyclesToPlay)
        {
            Destroy(gameObject);
            return;
        }

        cycleIndex++;
        missTriggeredThisCycle = false;

        // 스케일/타이밍 초기화
        outlineCircle.localScale = new Vector3(initialScale, initialScale, 1f);
        spawnTime = Time.time;
        targetMoment = spawnTime + (initialScale - targetScale) / Mathf.Max(0.0001f, shrinkSpeed);

        activeCycle = true;
    }
}
