using TMPro;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    // 게임 거리 / 무한모드 나중에 생성될 스토리 모드의 분기를 나눌 스크립트로 사용할 예정
    
    [Header("UI 모음")]
    // 거리를 나타낼 텍스트 변수
    [SerializeField]
    private TextMeshProUGUI GameDistanceText;
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

    [Header("스프라이트 모음")]
    // 체크포인트 문 오브젝트 -> 문의 최종 크기는 x 2 y 2(스케일)
    [SerializeField]
    GameObject DoorObject;
    
    private void Awake()
    {
        if(GameDistanceText == null)
        {
            GameDistanceText = GameObject.Find("GameDistanceText").GetComponent<TextMeshProUGUI>();
        }
        if(DoorObject == null)
        {
            DoorObject = GameObject.Find("CheckPoint");
            DoorObject.SetActive(false);
        }
        if(gameViewManager == null)
        {
            gameViewManager = GameObject.Find("Manager").GetComponent<GameViewManager>();
        }

        UpdateDistanceText();
    }

    void Update()
    {
        if (!isAtCheckpoint)
            // 거리 증가 로직
            IncreaseDistanceOverTime();   
        else
            // 체크포인트 터치 대기
            HandleCheckpointTouch();     
    }

    private void IncreaseDistanceOverTime()
    {
        distanceTimer += Time.deltaTime;

        if (distanceTimer >= 1f)
        {
            Distance += 1;
            distanceTimer -= 1f;

            UpdateDistanceText();

            // 체크포인트 도달 체크
            if (Distance >= CheckPointDistance)
                EnterCheckpoint();
        }
    }

    // 체크포인트 진입 처리
    private void EnterCheckpoint()
    {
        isAtCheckpoint = true;
        DoorObject.SetActive(true);
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
        DoorObject.SetActive(false);
        distanceTimer = 0f;

        // 게임 시간 초기화시키고 체크포인트에 도착을 하면 애니메이션을 멈춰야 할듯
        gameViewManager.ResetTimer(120);

        Debug.Log($"Next CheckPoint: Distance at {CheckPointDistance}, TouchCount {CheckPointTouch}");
    }
}
