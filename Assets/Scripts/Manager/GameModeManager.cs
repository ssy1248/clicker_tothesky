using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;

    [Header("UI 모음")]
    private GameViewManager gameViewManager;

    [Header("스프라이트 모음 & 오브젝트 모음")]
    [SerializeField]
    GameObject DoorObject;

    // 애니메이션용 설정
    [Header("애니메이션 설정")]
    [SerializeField, Range(0f, 1f)]
    private float doorShowThresholdRatio = 0.2f;   // 체크포인트 거리의 몇 퍼센트에서 문 나타나기 시작
    private bool hasDoorOpenStarted = false;
    // 원래 값 보관용
    private Vector3 doorOriginalScale;
    private Vector3 doorOriginalPosition;
    private Vector3 doorStartScale;
    [SerializeField] 
    private Vector3 doorTargetScale = new Vector3(0.2f, 0.18f, 1f);

    private GuageManager guageManager;

    [Header("챕터 데이터베이스")]
    public ChapterDatabase chapterDatabase;

    [Header("아이템 효과 변수")]
    public float SpeedItemPlus = 0;

    // 시간 기반 진행도
    private float totalStageTime;     // 스테이지 총 시간
    private float remainingTime;      // 남은 시간(초)
    private bool isRunning;          // 타이머 진행 중인지


    private bool _isFeverTime = false;
    public bool IsFeverTime => _isFeverTime;

    private void HandleFeverStart() => _isFeverTime = true;
    private void HandleFeverEnd() => _isFeverTime = false;

    [SerializeField] private bool enableDebugFeverKeys = true;

    private void OnEnable()
    {
        GuageImageAlpha.OnFeverStart += HandleFeverStart;
        GuageImageAlpha.OnFeverEnd += HandleFeverEnd;
    }

    private void OnDisable()
    {
        GuageImageAlpha.OnFeverStart -= HandleFeverStart;
        GuageImageAlpha.OnFeverEnd -= HandleFeverEnd;
    }

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            // Instance가 비어있으면 자기 자신을 할당
            Instance = this;
        }
        else if (Instance != this)
        {
            // 새로 생긴 이 오브젝트는 파괴하여 단 하나만 존재하도록 보장
            Debug.LogWarning("GameModeManager의 인스턴스가 이미 존재하여 새로 생긴 것을 파괴합니다.");
            Destroy(gameObject);
            return; // 파괴될 오브젝트는 아래 로직을 실행할 필요 없음
        }

        if (DoorObject == null)
        {
            DoorObject = GameObject.Find("CheckPoint");
        }

        // 초기 트랜스폼 값 저장
        doorOriginalScale = doorTargetScale;
        doorOriginalPosition = DoorObject.transform.localPosition;
        doorStartScale = new Vector3(0.1f, 0.1f, doorOriginalScale.z);
        DoorObject.SetActive(false);

        gameViewManager = GameObject.Find("GameViewManager").GetComponent<GameViewManager>();
        guageManager = GameObject.FindFirstObjectByType<GuageManager>();
    }

    private void Start()
    {
        // GlobalVariable로부터 현재 스테이지 인덱스 → StageData 가져오기
        int flat = GlobalVariable.Instance.PlayerCurrentPlayerStage;
        var (c, s) = chapterDatabase.GetChapterStageFromFlatIndex(flat);
        var stage = chapterDatabase.allChapterData[c].stagesInChapter[s];

        SubTouchManager.Instance.BeginStage(stage);

        // ★ 시간 기반 초기화
        totalStageTime = Mathf.Max(0.01f, stage.gameTime);
        remainingTime = totalStageTime;
        isRunning = true;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (enableDebugFeverKeys && Input.GetKeyDown(KeyCode.F))
        {
            guageManager?.DebugForceStartFever();
        }

        if (enableDebugFeverKeys && Input.GetKeyDown(KeyCode.G))
        {
            guageManager?.DebugForceEndFever();
        }
#endif

        // 1. 가장 먼저 GameViewManager를 통해 게임이 끝났는지 확인하고, 끝났으면 즉시 함수 종료
        if (gameViewManager.IsGameFinished)
        {
            return;
        }

        // 시간 흐름(스태미나 없으면 타이머 정지하고 싶으면 조건 추가)
        if (isRunning)
        {
            remainingTime -= Time.deltaTime;
            remainingTime = Mathf.Max(0f, remainingTime);

            UpdateTimeBasedState();
            AnimateDoorScaleByTime();

            // 시간 끝 → 게임 종료 처리
            if (remainingTime <= 0f)
            {
                isRunning = false;
                // 필요 시 여기서 GameOver / Finish 트리거
                // gameViewManager.EndGame(); 등
            }
        }
    }

    // 남은 시간 기준으로 문 등장 애니메이션
    private void UpdateTimeBasedState()
    {
        float showThresholdTime = totalStageTime * doorShowThresholdRatio; // 남은 시간이 이 값 이하가 되면 문 등장

        if (!hasDoorOpenStarted && remainingTime <= showThresholdTime)
        {
            hasDoorOpenStarted = true;
            DoorObject.SetActive(true);
            DoorObject.transform.localScale = doorStartScale;
            DoorObject.transform.localPosition = doorOriginalPosition;
        }
    }

    // 남은 시간에 따라 문 스케일 보간
    private void AnimateDoorScaleByTime()
    {
        if (!hasDoorOpenStarted) 
            return;

        float showThresholdTime = totalStageTime * doorShowThresholdRatio;

        // 남은시간이 showThresholdTime일 때 progress=0,
        // 남은시간이 0일 때 progress=1 이 되도록 역보간
        float progress = Mathf.InverseLerp(showThresholdTime, 0f, remainingTime);
        DoorObject.transform.localScale = Vector3.Lerp(doorStartScale, doorOriginalScale, progress);
    }

    /// <summary>
    /// 터치 입력을 받았을 때 GameViewManager에서 호출할 함수
    /// </summary>
    public void OnPlayerTouch()
    {
        if (gameViewManager.IsGameFinished) 
            return;

        //int scoreToAdd = _isFeverTime ? 2 : 1;
        //ScoreManager.Instance.AddScore(scoreToAdd);
    }
}
