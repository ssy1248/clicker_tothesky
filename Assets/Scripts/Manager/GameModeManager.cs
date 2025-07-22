using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;

    [Header("UI 모음")]
    [SerializeField]
    private Image CharacterImage;
    [SerializeField]
    private RectTransform progressTrackBar;
    private GameViewManager gameViewManager;

    [Header("변수 모음")]
    // 거리를 초기화할 초기 변수
    [SerializeField]
    int Distance;
    // 1초를 누적할 타이머 변수
    private float distanceTimer = 0f;
    // 체크포인트 거리
    public int CheckPointDistance;
    // 체크포인트 상태 플래그
    private bool isAtCheckpoint = false;

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
    // 캐릭터 최초 X 좌표 저장용
    private float charStartX;

    private bool isStaminaEmpty = false;

    private GuageManager guageManager;

    [Header("스테이지 데이터베이스")]
    public StageDatabase stageDatabase;

    [Header("수집품 생성 관련")]
    private int nextCollectIndex = 0;
    private float collectSpawnInterval;

    [Header("아이템 효과 변수")]
    public float SpeedItemPlus = 0;

    [Header("거리 및 속도")]
    [SerializeField] private float distancePerSecond = 5f; // 초당 이동하는 거리(속도)
    private float currentFloatDistance = 0f; // 정밀한 거리 계산을 위한 float 변수

    private GameObject currentCollectible = null;
    private void OnEnable()
    {
        GuageImageAlpha.OnStaminaEmpty += HandleStaminaEmpty;
        GuageImageAlpha.OnStaminaRecovered += HandleStaminaRecovered;
    }

    private void OnDisable()
    {
        GuageImageAlpha.OnStaminaEmpty -= HandleStaminaEmpty;
        GuageImageAlpha.OnStaminaRecovered -= HandleStaminaRecovered;
    }

    private void HandleStaminaEmpty() => isStaminaEmpty = true;
    private void HandleStaminaRecovered() => isStaminaEmpty = false;

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

        // 캐릭터의 시작 AnchoredPosition.x를 한 번 저장
        charStartX = CharacterImage.rectTransform.anchoredPosition.x;

        guageManager = GameObject.FindFirstObjectByType<GuageManager>();
    }

    private void Start()
    {
        // 글로벌 변수에서 값을 가져오기
        Distance = GlobalVariable.Instance.PlayerCurrentDistance;

        currentFloatDistance = Distance;

        CheckPointDistance = GlobalVariable.Instance.CheckPointDistance;

        int totalCollectCount = GlobalVariable.Instance.StageMaxCollectCount;
        collectSpawnInterval = totalCollectCount > 0
            ? CheckPointDistance / (float)(totalCollectCount + 1)
            : CheckPointDistance;
    }

    void Update()
    {
        // 1. 가장 먼저 GameViewManager를 통해 게임이 끝났는지 확인하고, 끝났으면 즉시 함수 종료
        if (gameViewManager.IsGameFinished)
        {
            return;
        }

        // 스태미나 비어있으면 거리 로직 통째로 스킵
        if (isStaminaEmpty) 
            return;

        // 거리 증가 로직
        IncreaseDistanceOverTime();
        // 거리 기반 스케일 업데이트
        AnimateDoorScale();
        // 이동 게이지 조절 함수
        AnimateProgressFill();
    }

    private void IncreaseDistanceOverTime()
    {
        // 스태미나가 비었으면 진행 중단
        if (isStaminaEmpty)
            return;

        // 1. 게이지 상태에 따른 속도 배율 계산 (기존과 동일)
        float speedMultiplier = 1f;
        if (guageManager != null && guageManager.GaugeValue <= guageManager.DANGER_THRESHOLD_LOW)
        {
            speedMultiplier = 2f;
        }
        else if (guageManager != null && guageManager.GaugeValue >= guageManager.DANGER_THRESHOLD_HIGH)
        {
            speedMultiplier = 0.5f;
        }

        // 2. 아이템 효과에 따른 속도 배율 계산 (기존과 동일)
        float itemSpeedMultiplier = 1f + SpeedItemPlus;

        // 3. 최종 속도를 계산하여 이번 프레임에 이동한 거리를 구함
        float distanceThisFrame = distancePerSecond * speedMultiplier * itemSpeedMultiplier * Time.deltaTime;

        // 4. 정밀 거리에 더해줌
        currentFloatDistance += distanceThisFrame;

        // 5. UI나 로직에 사용할 정수형 Distance 변수도 업데이트
        Distance = (int)currentFloatDistance;

        // 6. 전역 변수에도 반영
        GlobalVariable.Instance.PlayerCurrentDistance = this.Distance;

        // 7. 수집품 생성 조건 확인
        //TrySpawnCollectible();

        // 8. 문이 나타날 거리에 도달했는지 확인합니다.
        float thresholdDistance = CheckPointDistance * doorOpenThreshold;
        if (!hasDoorOpenStarted && Distance >= thresholdDistance)
        {
            // 문이 나타나기 시작했다는 플래그를 true로 설정합니다. (이래야 AnimateDoorScale이 작동)
            hasDoorOpenStarted = true;

            // 문 오브젝트를 활성화하고 초기 상태(작은 크기)로 설정합니다.
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

    // AnimationAllStop을 하면 AnimateProgressFill을 멈춰야함
    private void AnimateProgressFill()
    {
        // 애니메이션 정지 상태라면 위치 갱신 중단
        if (isStaminaEmpty)
            return;

        // 1) 진행도 계산 (기존과 동일)
        float progress = Mathf.SmoothStep(0, 1, Distance / (float)CheckPointDistance);

        // 2) 연결된 트랙의 높이를 기준으로 시작과 끝 Y좌표를 동적으로 계산
        //    (트랙의 Pivot이 중앙(0.5, 0.5)에 있다고 가정)
        float barHeight = progressTrackBar.rect.height;
        float startY = -barHeight / 2f;
        float endY = barHeight / 2f;
        float newY = Mathf.Lerp(startY, endY, progress);

        // 3) 캐릭터 위치 이동
        RectTransform charRT = CharacterImage.rectTransform;
        // x좌표는 0으로 고정하여 항상 트랙의 중앙에 있도록 함
        charRT.anchoredPosition = new Vector2(0, newY);
    }

    //private void TrySpawnCollectible()
    //{
    //    float expectedSpawnDistance = collectSpawnInterval * (nextCollectIndex + 1);

    //    if (Distance >= expectedSpawnDistance && nextCollectIndex < GlobalVariable.Instance.StageMaxCollectCount)
    //    {
    //        // 1. 현재 스테이지 정보를 가져옵니다.
    //        int currentStageIndex = GlobalVariable.Instance.PlayerCurrentPlayerStage;
    //        StageData currentStageData = stageDatabase.allStageData[currentStageIndex];

    //        Debug.Log($"스테이지 {currentStageIndex + 1}의 수집품 생성 시도. 목록 개수: {currentStageData.collectiblesInStage.Count}, 다음 인덱스: {nextCollectIndex}");

    //        // 목록에 접근하기 전에 개수를 확인
    //        if (nextCollectIndex >= currentStageData.collectiblesInStage.Count)
    //        {
    //            Debug.LogError($"생성하려는 수집품 인덱스({nextCollectIndex})가 스테이지({currentStageIndex + 1})의 수집품 목록 크기({currentStageData.collectiblesInStage.Count})를 벗어났습니다! StageDatabase를 확인해주세요.");
    //            return;
    //        }

    //        // 2. 이번에 생성할 수집품 데이터를 스테이지 정보에서 가져옵니다.
    //        CollectScriptableObject collectibleToSpawn = currentStageData.collectiblesInStage[nextCollectIndex];

    //        // 3. CollectManager에게 특정 데이터를 가진 수집품 생성을 요청합니다.
    //        currentCollectible = CollectManager.Instance.CreateCollectObject(collectibleToSpawn);
    //        nextCollectIndex++;
    //    }
    //}

    public void OnCollectibleCollected()
    {
        currentCollectible = null;
    }
}
