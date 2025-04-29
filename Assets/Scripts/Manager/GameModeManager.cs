using System.Collections;
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

    [Header("스프라이트 모음 & 오브젝트 모음")]
    // 체크포인트 문 오브젝트 -> 문의 최종 크기는 x 2 y 2(스케일)
    [SerializeField]
    GameObject DoorObject;

    // 애니메이션용 설정
    [Header("애니메이션 설정")]
    // 스케일 애니메이션 시간
    [SerializeField] 
    private float openDuration = 1f;
    // 하강 애니메이션 시간
    [SerializeField] 
    private float closeDuration = 1f;  

    // 원래 값 보관용
    private Vector3 doorOriginalScale;
    private Vector3 doorOriginalPosition;
    private Vector3 doorStartScale;

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

        // 초기 트랜스폼 값 저장
        doorOriginalScale = DoorObject.transform.localScale;
        doorOriginalPosition = DoorObject.transform.localPosition;
        // 초기 스케일
        doorStartScale = new Vector3(0.1f, 0.1f, doorOriginalScale.z);

        // 문 숨김
        DoorObject.SetActive(false);

        // GameViewManager
        gameViewManager = GameObject.Find("Manager").GetComponent<GameViewManager>();

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

        // 문을 작게 세팅 후 열기 애니메이션
        DoorObject.transform.localScale = doorStartScale;
        DoorObject.transform.localPosition = doorOriginalPosition;

        // 지금은 거리가 도달하면 보이는데 
        // 이건 정해줘야 할듯? -> 체크포인트의 거리의 80퍼센트도착하면 그때부터 시작?
        StartCoroutine(OpenDoorRoutine());
    }

    private IEnumerator OpenDoorRoutine()
    {
        float elapsed = 0f;
        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / openDuration);
            DoorObject.transform.localScale = Vector3.Lerp(
                doorStartScale, doorOriginalScale, t);
            yield return null;
        }
        DoorObject.transform.localScale = doorOriginalScale;
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

        // 타이머 리셋
        gameViewManager.ResetTimer(120);

        // 닫기 애니메이션 시작
        StartCoroutine(CloseDoorRoutine());

        Debug.Log($"Next CheckPoint: Distance at {CheckPointDistance}, TouchCount {CheckPointTouch}");
    }

    private IEnumerator CloseDoorRoutine()
    {
        float elapsed = 0f;
        Vector3 startPos = doorOriginalPosition;
        Vector3 endPos = new Vector3(
            startPos.x, -5f, startPos.z);

        while (elapsed < closeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / closeDuration);
            DoorObject.transform.localPosition =
                Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // 완전히 내린 뒤 비활성화 및 원복
        DoorObject.transform.localPosition = endPos;
        DoorObject.SetActive(false);
        DoorObject.transform.localPosition = doorOriginalPosition;
        DoorObject.transform.localScale = doorOriginalScale;
    }
}
