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

    [Header("변수 모음")]
    // 거리를 초기화할 초기 변수
    [SerializeField]
    int Distance;
    // 체크포인트 거리
    public int CheckPointDistance;

    [Header("스프라이트 모음 & 오브젝트 모음")]
    // 체크포인트 문 오브젝트 -> 문의 최종 크기는 x 0.2 y 0.18(스케일)
    [SerializeField]
    GameObject DoorObject;

    // 애니메이션용 설정
    [Header("애니메이션 설정")]
    [SerializeField, Range(0f, 1f)]
    private float doorOpenThreshold = 0.8f;  // 체크포인트 거리의 몇 퍼센트에서 문 나타나기 시작
    private bool hasDoorOpenStarted = false;
    // 원래 값 보관용
    private Vector3 doorOriginalScale;
    private Vector3 doorOriginalPosition;
    private Vector3 doorStartScale;
    [SerializeField] 
    private Vector3 doorTargetScale = new Vector3(0.2f, 0.18f, 1f);

    private bool isStaminaEmpty = false;
    private GuageManager guageManager;

    [Header("스테이지 데이터베이스")]
    public ChapterDatabase chapterDatabase;

    [Header("아이템 효과 변수")]
    public float SpeedItemPlus = 0;

    [Header("거리 및 속도")]
    private float currentFloatDistance = 0f; // 정밀한 거리 계산을 위한 float 변수

    [Header("터치 설정")]
    [SerializeField] 
    private int touchesPerMeter = 4; // 1미터 전진에 필요한 터치 횟수
    private int touchCount = 0; // 현재 터치 횟수를 기록

    private bool _isFeverTime = false;
    public bool IsFeverTime => _isFeverTime;

    private void OnEnable()
    {
        GuageImageAlpha.OnStaminaEmpty += HandleStaminaEmpty;
        GuageImageAlpha.OnStaminaRecovered += HandleStaminaRecovered;

        GuageImageAlpha.OnFeverStart += HandleFeverStart;
        GuageImageAlpha.OnFeverEnd += HandleFeverEnd;
    }

    private void OnDisable()
    {
        GuageImageAlpha.OnStaminaEmpty -= HandleStaminaEmpty;
        GuageImageAlpha.OnStaminaRecovered -= HandleStaminaRecovered;

        GuageImageAlpha.OnFeverStart -= HandleFeverStart;
        GuageImageAlpha.OnFeverEnd -= HandleFeverEnd;
    }

    private void HandleStaminaEmpty() => isStaminaEmpty = true;
    private void HandleStaminaRecovered() => isStaminaEmpty = false;

    private void HandleFeverStart() => _isFeverTime = true;
    private void HandleFeverEnd() => _isFeverTime = false;

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
            DoorObject.SetActive(false);
        }

        // 초기 트랜스폼 값 저장
        doorOriginalScale = doorTargetScale;
        doorOriginalPosition = DoorObject.transform.localPosition;
        // 초기 스케일
        doorStartScale = new Vector3(0.1f, 0.1f, doorOriginalScale.z);

        // 문 숨김
        DoorObject.SetActive(false);

        // GameViewManager
        gameViewManager = GameObject.Find("GameViewManager").GetComponent<GameViewManager>();

        guageManager = GameObject.FindFirstObjectByType<GuageManager>();

        //// 현재 스테이지 데이터에서 미니게임 정보를 가져옴
        //SubTouchType type = currentStageData.miniGameType;
        //int score = currentStageData.miniGameSuccessScore;

        //// SubTouchManager에게 미니게임 생성을 요청
        //SubTouchManager.Instance.SpawnMiniGame(type, score);
    }

    private void Start()
    {
        // 글로벌 변수에서 값을 가져오기
        Distance = GlobalVariable.Instance.PlayerCurrentDistance;

        currentFloatDistance = Distance;

        CheckPointDistance = GlobalVariable.Instance.CheckPointDistance;

        if (gameViewManager != null)
        {
            int remaining = CheckPointDistance - Distance;
            gameViewManager.UpdateRemainingDistanceUI(remaining);
        }
    }

    void Update()
    {
        // 1. 가장 먼저 GameViewManager를 통해 게임이 끝났는지 확인하고, 끝났으면 즉시 함수 종료
        if (gameViewManager.IsGameFinished || isStaminaEmpty)
        {
            return;
        }

        // 거리 증가 로직
        UpdateDistanceBasedState();
        // 거리 기반 스케일 업데이트
        AnimateDoorScale();
    }

    private void UpdateDistanceBasedState()
    {
        Distance = (int)currentFloatDistance;
        GlobalVariable.Instance.PlayerCurrentDistance = this.Distance;

        float thresholdDistance = CheckPointDistance * doorOpenThreshold;
        if (!hasDoorOpenStarted && Distance >= thresholdDistance)
        {
            hasDoorOpenStarted = true;
            DoorObject.SetActive(true);
            DoorObject.transform.localScale = doorStartScale;
            DoorObject.transform.localPosition = doorOriginalPosition;
        }
    }

    // 거리에 따라 문 스케일 보간
    private void AnimateDoorScale()
    {
        if (!hasDoorOpenStarted)
            return;

        float thresholdDist = CheckPointDistance * doorOpenThreshold;
        float progress = Mathf.Clamp01((Distance - thresholdDist) / (CheckPointDistance - thresholdDist));

        // doorTargetScale 사용
        DoorObject.transform.localScale = Vector3.Lerp(doorStartScale, doorOriginalScale, progress);
    }

    /// <summary>
    /// 터치 입력을 받았을 때 GameViewManager에서 호출할 함수
    /// </summary>
    public void OnPlayerTouch()
    {
        if (isStaminaEmpty || gameViewManager.IsGameFinished)
            return;

        // 1. 피버 타임 여부에 따라 점수 추가
        int scoreToAdd = _isFeverTime ? 2 : 1;
        ScoreManager.Instance.AddScore(scoreToAdd);

        // 2. 피버 타임 여부에 따라 거리 증가
        touchCount++;
        int requiredTouches = _isFeverTime ? touchesPerMeter / 2 : touchesPerMeter;

        if (touchCount >= requiredTouches)
        {
            currentFloatDistance++; // 1미터 전진
            touchCount = 0;

            if (gameViewManager != null)
            {
                int remaining = CheckPointDistance - (int)currentFloatDistance;
                gameViewManager.UpdateRemainingDistanceUI(remaining);
            }
        }
    }
}
