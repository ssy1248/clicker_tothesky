using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    int Distance;
    // 1초를 누적할 타이머 변수
    private float distanceTimer = 0f;
    // 체크포인트 거리
    public int CheckPointDistance;
    // 체크포인트 상태 플래그
    private bool isAtCheckpoint = false;  

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
    // 캐릭터 최초 X 좌표 저장용
    private float charStartX;

    private bool isStaminaEmpty = false;

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

    private void HandleStaminaEmpty()
    {
        isStaminaEmpty = true;
    }

    private void HandleStaminaRecovered()
    {
        isStaminaEmpty = false;
    }

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

        // 문 숨김
        DoorObject.SetActive(false);

        // GameViewManager
        gameViewManager = GameObject.Find("GameViewManager").GetComponent<GameViewManager>();

        // 캐릭터의 시작 AnchoredPosition.x를 한 번 저장
        charStartX = CharacterImage.rectTransform.anchoredPosition.x;
    }

    private void Start()
    {
        // 글로벌 변수에서 값을 가져오기
        Distance = GlobalVariable.Instance.PlayerCurrentDistance;
        CheckPointDistance = GlobalVariable.Instance.CheckPointDistance;

        // 체크포인트 거리 텍스트 설정
        CheckPointDistanceText.text = CheckPointDistance.ToString() + " M";

        UpdateDistanceText();
    }

    void Update()
    {
        // ① 스태미나 비어있으면 거리 로직 통째로 스킵
        if (isStaminaEmpty) 
            return;

        // GameViewManager에서 HandleStaminaZero 함수가 실행이 되면 트리거를 보내서 거리 증가 로직을 막는다
        if (!isAtCheckpoint)
        {
            // 거리 증가 로직
            IncreaseDistanceOverTime();
            // 거리 기반 스케일 업데이트
            AnimateDoorScale();
            // 이동 게이지 조절 함수
            AnimateProgressFill();
        }
        else
        {
            // 클리어 패널
        } 
    }

    private void IncreaseDistanceOverTime()
    {
        // 1) 시간 흐름에 따라 거리 증가
        distanceTimer += Time.deltaTime; //* speedMultiplier;

        // 2) 1초마다 거리 1 증가
        while (distanceTimer >= 1f)
        {
            Distance++;
            distanceTimer -= 1f;
            UpdateDistanceText();

            // 기존 체크포인트 문 열기/진입 로직
            float thresholdDistance = CheckPointDistance * doorOpenThreshold;
            if (!hasDoorOpenStarted && Distance >= thresholdDistance)
            {
                hasDoorOpenStarted = true;
                DoorObject.SetActive(true);
                DoorObject.transform.localScale = doorStartScale;
                DoorObject.transform.localPosition = doorOriginalPosition;
            }
            if (Distance >= CheckPointDistance)
            {
                EnterCheckpoint();
                break;
            }
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

    private void AnimateProgressFill()
    {
        // 1) 진행도 계산
        float progress = Mathf.Clamp01(Distance / (float)CheckPointDistance);

        // 2) 게이지 채우기
        FilledImage.fillAmount = progress;

        // 3) 캐릭터 이동: 원래 위치(charStartX) + 진행도*게이지폭
        RectTransform gaugeRT = FilledImage.rectTransform;
        float gaugeWidth = gaugeRT.rect.width;

        RectTransform charRT = CharacterImage.rectTransform;
        Vector2 anchored = charRT.anchoredPosition;

        anchored.x = charStartX + gaugeWidth * progress;
        charRT.anchoredPosition = anchored;
    }
}
