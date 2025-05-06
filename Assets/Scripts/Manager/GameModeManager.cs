using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour
{
    // 게임 거리 / 무한모드 나중에 생성될 스토리 모드의 분기를 나눌 스크립트로 사용할 예정
    
    [Header("UI 모음")]
    // 거리를 나타낼 텍스트
    [SerializeField]
    private TextMeshProUGUI GameDistanceText;
    // 체크포인트 거리를 보여줄 텍스트
    [SerializeField]
    private TextMeshProUGUI CheckPointDistanceText;
    [SerializeField]
    private Image FilledImage;
    [SerializeField]
    private Image CharacterImage;
    private GameViewManager gameViewManager;

    [Header("변수 모음")]
    // 거리를 초기화할 초기 변수
    [SerializeField]
    int Distance = 0;
    // 1초를 누적할 타이머 변수
    private float distanceTimer = 0f;
    // 체크포인트 거리
    public int CheckPointDistance = 50;
    // 체크포인트를 넘어가기 위한 터치 횟수
    public int CheckPointTouch = 10;
    // 체크포인트 상태 플래그
    private bool isAtCheckpoint = false;
    // 현재 터치 카운트
    private int currentTouchCount = 0;      

    [Header("스프라이트 모음 & 오브젝트 모음")]
    // 체크포인트 문 오브젝트 -> 문의 최종 크기는 x 2 y 2(스케일)
    [SerializeField]
    GameObject DoorObject;

    // 애니메이션용 설정
    [Header("애니메이션 설정")]
    [SerializeField, Range(0f, 1f)]
    private float doorOpenThreshold = 0.8f;  // 체크포인트 거리의 몇 퍼센트에서 문 열기 시작
    private bool hasDoorOpenStarted = false;

    // 원래 값 보관용
    private Vector3 doorOriginalScale;
    private Vector3 doorOriginalPosition;
    private Vector3 doorStartScale;
    [SerializeField] 
    private Vector3 doorTargetScale = new Vector3(2f, 2f, 1f);

    private void Awake()
    {
        if(GameDistanceText == null)
        {
            GameDistanceText = GameObject.Find("GameDistanceText").GetComponent<TextMeshProUGUI>();
        }
        if (CheckPointDistanceText == null)
        {
            CheckPointDistanceText = GameObject.Find("CheckPointDistanceText").GetComponent<TextMeshProUGUI>();
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

        // 체크포인트 거리 텍스트 설정
        CheckPointDistanceText.text = CheckPointDistance.ToString() + " M";

        // 문 숨김
        DoorObject.SetActive(false);

        // GameViewManager
        gameViewManager = GameObject.Find("Manager").GetComponent<GameViewManager>();

        UpdateDistanceText();
    }

    void Update()
    {
        if (!isAtCheckpoint)
        {
            // 거리 증가 로직
            IncreaseDistanceOverTime();
            // 거리 기반 스케일 업데이트
            AnimateDoorScale();  
        }
        else
            // 체크포인트 터치 대기
            HandleCheckpointTouch();     
    }

    private void IncreaseDistanceOverTime()
    {
        distanceTimer += Time.deltaTime;
        if (distanceTimer >= 1f)
        {
            Distance++;
            distanceTimer -= 1f;
            UpdateDistanceText();

            // 80% 지점에서 문 열기 시작
            float thresholdDistance = CheckPointDistance * doorOpenThreshold;
            if (!hasDoorOpenStarted && Distance >= thresholdDistance)
            {
                hasDoorOpenStarted = true;
                DoorObject.SetActive(true);
                DoorObject.transform.localScale = doorStartScale;
                DoorObject.transform.localPosition = doorOriginalPosition;
            }

            // 체크포인트 도달 시 동작
            if (Distance >= CheckPointDistance)
                EnterCheckpoint();
        }
    }

    // 체크포인트 진입 처리
    private void EnterCheckpoint()
    {
        isAtCheckpoint = true;
        // 문 스케일을 정확히 목표 스케일로 설정
        DoorObject.transform.localScale = doorOriginalScale;
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

    private void UpdateDistanceText()
    {
        GameDistanceText.text = Distance.ToString() + " M";
    }

    // 체크포인트 해제를 위한 터치 입력 대기
    private void HandleCheckpointTouch()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            currentTouchCount++;
            Debug.Log($"Checkpoint Touch: {currentTouchCount}/{CheckPointTouch}");
        }
        #else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            currentTouchCount++;
            Debug.Log($"Checkpoint Touch: {currentTouchCount}/{CheckPointTouch}");
        }
        #endif

        // 터치 횟수 만족 시 다음 체크포인트 준비
        if (currentTouchCount >= CheckPointTouch)
            ExitCheckpoint();
    }

    // 체크포인트 해제 & 다음 단계 설정
    private void ExitCheckpoint()
    {
        // 거리와 터치 요구량 2배로 증가
        CheckPointDistance *= 2;
        CheckPointTouch *= 2;

        // 상태 초기화
        currentTouchCount = 0;
        isAtCheckpoint = false;
        distanceTimer = 0f;
        hasDoorOpenStarted = false;

        // 타이머 리셋
        gameViewManager.ResetTimer(120);


        Debug.Log($"Next CheckPoint: Distance at {CheckPointDistance}, TouchCount {CheckPointTouch}");
    }
}
