using System;
using BigNumber;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameViewManager : MonoBehaviour
{
    public Animator kiwiAnim;
    private float lastclickupdate = 0f;

    [Header("트리거 모음")]
    // 한번만 실행될 수 있도록
    private bool gameOver = false;
    // 기준선을 넘겼는지 나타내는 플래그
    private bool isAboveThreshold = false;
    private bool isStaminaEmpty = false;
    private bool inputEnabled = true;

    [Header("UI")]
    // Radial 360 세팅된 Image
    [SerializeField] 
    private Image touchGaugeImage;     
    [SerializeField]
    private TextMeshProUGUI gameTimeText;

    [Header("게이지 세팅")]
    // 게이지가 초당 이만큼 줄어듦
    [SerializeField] 
    private float decayRate = 0.1f;
    // 최소 fillAmount
    const float MIN_FILL = 0.22f;
    // 최대 fillAmount
    const float MAX_FILL = 0.927f;

    [Header("타임 패널")]
    [SerializeField] 
    private TimePanel timePanel;      // TimePanel 스크립트 연결
    private bool blinkStarted = false;                 // 한 번만 실행 플래그

    private float totalTime;

    // 내부 게이지 값 (0~∞) → 0~1 로 클램프
    public float gaugeValue = 0f;  

    private void Addkiwi(BigDouble amt)
    {
        GlobalManager.Instance.kiwiAmount += amt;
    }


    public void TimeKiwi()
    {
        lastclickupdate += Time.deltaTime;
        if (lastclickupdate <= 1f)
            return;
        if (lastclickupdate > 1f)
        {
            GlobalManager.Instance.kiwiAmount -= 1;
        }
    }

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
        inputEnabled = false;

        // 게이지 투명 고정
        var col = touchGaugeImage.color;
        col.a = 0f;
        touchGaugeImage.color = col;

        // 애니메이션 정지
        AnimationManager.Instance.AnimationAllStop();
    }

    private void HandleStaminaRecovered()
    {
        isStaminaEmpty = false;
        inputEnabled = true;
        isAboveThreshold = false;

        // 애니메이션 재생
        AnimationManager.Instance.AnimationAllPlay();
    }

    private void UpdateGauge()
    {
        // 스태미너 고갈 후에는 더 이상 게이지 업데이트 금지
        if (isStaminaEmpty)
            return;

        // 내부 값(gaugeValue)을 0~1로 정규화
        float normalized = Mathf.Clamp01(gaugeValue);
        // 0~1 사이를 MIN_FILL~MAX_FILL 사이로 보간
        touchGaugeImage.fillAmount = Mathf.Lerp(MIN_FILL, MAX_FILL, normalized);

        // 구간별 색상 결정
        Color c;
        if (normalized < 0.5f)
            c = Color.green;      // 초록: 0.00 ~ 0.5
        else if (normalized < 0.8f)
            c = Color.yellow;     // 노랑: 0.5 ~ 0.8
        else
            c = Color.red;        // 빨강: 0.8 ~ 1.00

        // 임시로 빨간색일땐 5초로 두고 깜빡거리는 애니메이션 제작
        // 0.8 이상/미만에 따라 스태미나 라이프 시작/취소
        if (normalized >= 0.8f)
        {
            if (!isAboveThreshold)
            {
                isAboveThreshold = true;
                GuageImageAlpha.Instance.StartLifeRoutine();
            }
        }
        else
        {
            if (isAboveThreshold)
            {
                isAboveThreshold = false;
                GuageImageAlpha.Instance.CancelLifeRoutine();
            }
        }

        // GuageColorController를 통해 실제 UI 색 변경
        GuageColorController.Instance.SetGaugeColor(c);
    }

    private void Awake()
    {
        gameTimeText = GameObject.Find("GameTimeText").GetComponent<TextMeshProUGUI>();

        // 초기 게이지
        touchGaugeImage.fillAmount = MIN_FILL;

        totalTime = GlobalManager.Instance.inGameCountTime;
        UpdateTimerUI();
    }

    public void ResetTimer(int seconds)
    {
        totalTime = seconds;   // ← 여기에 120을 넣으면 120초로 세팅
        UpdateTimerUI();

        // 타이머가 리셋됐으니 깜박임 중지
        timePanel.StopBlinking();
        blinkStarted = false;
    }

    private void UpdateTimerUI()
    {
        int t = Mathf.FloorToInt(totalTime);
        int minutes = t / 60;
        int secs = t % 60;
        gameTimeText.text = $"{minutes:00}:{secs:00}";
    }

    private void OnGameOver()
    {
        gameOver = true;
        Debug.Log("Time's up! 게임 끝");

        // 1) 전역 변수 초기화
        GlobalVariable.Instance.CheckPointDistance = 50;
        GlobalVariable.Instance.CheckPointTouchCount = 10;
        GlobalVariable.Instance.PlayerCurrentDistance = 0;

        // 2) 필요하다면 타이머나 UI도 초기화
        // ResetTimer(120);

        // 3) 씬 전환 혹은 리플레이 로직 호출
        // SceneManager.LoadScene("TitleScene");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GlobalManager.Instance.kiwiAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // 1) 타이머 처리
        if (totalTime > 0f)
        {
            totalTime -= Time.deltaTime;
            if (totalTime < 0f) 
                totalTime = 0f;
            UpdateTimerUI();

            // 남은 시간이 30초 이하가 되고, 아직 깜박임을 시작하지 않았다면
            if (totalTime <= 30f && !blinkStarted)
            {
                timePanel.StartBlinking();
                blinkStarted = true;
            }

            if (totalTime == 0f)
                OnGameOver();
        }

        // 2) 게이지 자동 감소
        if (gaugeValue > 0f)
        {
            gaugeValue -= decayRate * Time.deltaTime;
            if (gaugeValue < 0f) 
                gaugeValue = 0f;
            UpdateGauge();  
        }

        // UI 위 터치는 무시
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // 입력 무시
            if (!inputEnabled)
                return;

            SEManager.instance.PlaySE("click");

            Debug.Log("ClickDown");
            Addkiwi(GlobalManager.Instance.GetTouchAmount());

            // 클릭할 때마다 0.05 더하되 0~1 사이로 클램프
            gaugeValue = Mathf.Clamp01(gaugeValue + 0.05f);
            UpdateGauge();

            lastclickupdate = 0f;
            kiwiAnim.Play("touch", 0, 0);
        }

        if (GlobalManager.Instance.kiwiAmount > 0)
        {
            TimeKiwi();
            
        }

        if (GlobalManager.Instance.kiwiAmount == 0)
        {
            return;
        }
       

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log(message: "ClickUp");
        }
        
    }
}
