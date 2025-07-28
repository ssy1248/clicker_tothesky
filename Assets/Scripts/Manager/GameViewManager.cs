using System;
using BigNumber;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameViewManager : MonoBehaviour
{
    private float lastclickupdate = 0f;

    [Header("트리거 모음")]
    private bool gameOver = false;
    private bool gameClear = false;
    private bool isStaminaEmpty = false;
    private bool inputEnabled = true;

    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI gameTimeText;
    [SerializeField]
    private TextMeshProUGUI distanceText; // 목표 거리를 표시할 텍스트
    public GameObject GameOverPanel;
    public GameObject GameClearPanel;

    [Header("타임 패널")]
    [SerializeField]
    private TimePanel timePanel;
    private bool blinkStarted = false;

    private float totalTime;
    private float initialTotalTime;

    [Header("게이지 매니저")]
    [SerializeField]
    private GuageManager gaugeManager;

    [Header("아이템 관련 변수")]
    public float GameTimePlus = 0;

    // 다른 스크립트에서 게임이 끝났는지(클리어 또는 오버) 확인할 수 있도록 해주는 프로퍼티
    public bool IsGameFinished => gameOver || gameClear;

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

        // 애니메이션 정지
        AnimationManager.Instance.AnimationAllStop();
    }

    private void HandleStaminaRecovered()
    {
        isStaminaEmpty = false;
        inputEnabled = true;

        // 애니메이션 재생
        AnimationManager.Instance.AnimationAllPlay();
    }

    private void Awake()
    {
        gameTimeText = GameObject.Find("GameTimeText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        // InGameItemManager가 GameTimePlus를 먼저 설정한 후, Start에서 타이머를 초기화합니다.
        totalTime = GlobalManager.Instance.inGameCountTime + GameTimePlus;
        initialTotalTime = totalTime;
        UpdateTimerUI();

        // 1. GlobalVariable에서 현재 스테이지의 목표 거리를 가져옵니다.
        int clearDistance = GlobalVariable.Instance.CheckPointDistance;

        // 2. "목표거리 M" 형식의 문자열을 만듭니다.
        string distanceString = $"{clearDistance} M";

        // 3. 텍스트 UI에 문자열을 할당합니다.
        if (distanceText != null)
        {
            distanceText.text = distanceString;
        }
    }

    public void ResetTimer(int seconds)
    {
        totalTime = seconds;
        UpdateTimerUI();

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

        // 게임 오버 UI 띄움
        GameOverPanel.SetActive(true);

        GlobalVariable.Instance.CheckPointDistance = 0;
        GlobalVariable.Instance.PlayerCurrentDistance = 0;
    }

    void Update()
    {
        // 게임이 끝나거나 클리어 상태이면 아래 로직을 실행하지 않음
        if (gameOver || gameClear)
            return;

#if UNITY_EDITOR
        // 'A' 키를 누르면 강제 클리어
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.LogWarning("DEBUG: 'A' 키 입력으로 강제 클리어 실행");
            OnGameClear();
            return; // 클리어 처리를 했으므로 즉시 Update 함수 종료
        }

        // 'S' 키를 누르면 강제 게임 오버
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.LogWarning("DEBUG: 'S' 키 입력으로 강제 게임 오버 실행");
            OnGameOver();
            return; // 게임 오버 처리를 했으므로 즉시 Update 함수 종료
        }
#endif

        if (totalTime > 0f)
        {
            totalTime -= Time.deltaTime;
            if (totalTime < 0f) 
                totalTime = 0f;

            UpdateTimerUI();

            if (totalTime <= 30f && !blinkStarted)
            {
                timePanel.StartBlinking();
                blinkStarted = true;
            }
        }

        // 1. 시간 초과를 먼저 확인
        if (totalTime <= 0f)
        {
            OnGameOver();
            return; // 게임 오버 처리 후 즉시 Update 종료
        }

        // 2. 시간 초과가 아닐 경우에만 클리어 조건을 확인
        if (GlobalVariable.Instance.PlayerCurrentDistance >= GlobalVariable.Instance.CheckPointDistance)
        {
            OnGameClear(); // 목표 거리에 도달하면 클리어 처리
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!inputEnabled)
                return;

            //SEManager.instance.PlaySE("click");

            // 게이지 감소는 GaugeManager에서 관리
            gaugeManager.OnTouch();

            // GameModeManager에게 터치 신호를 전달하여 거리를 계산하도록 합니다.
            GameModeManager.Instance.OnPlayerTouch();

            lastclickupdate = 0f;
        }
    }

    private void OnGameClear()
    {
        gameClear = true; // 클리어 플래그 설정
        inputEnabled = false; // 추가 입력 방지
        Debug.Log("목표 달성! 게임 클리어");

        // 1. 클리어 시간 계산 (시작 시간) - (남은 시간) = (플레이한 시간)
        float clearTime = initialTotalTime - totalTime;

        // 2. 계산된 시간을 GlobalVariable에 저장
        GlobalVariable.Instance.LastClearTime = clearTime;

        // 클리어한 라운드 정보 갱신
        int currentStage = GlobalVariable.Instance.PlayerCurrentPlayerStage;
        int oldClearRound = GlobalVariable.Instance.PlayerClearRound;

        // 현재 클리어한 스테이지와 이전 기록을 비교하여 더 높은 쪽을 저장
        GlobalVariable.Instance.PlayerClearRound = Mathf.Max(oldClearRound, currentStage + 1);

        // (선택사항) 클리어한 스테이지가 현재 최고 기록보다 높으면 갱신
        // if (GlobalVariable.Instance.PlayerCurrentPlayerStage > GlobalVariable.Instance.PlayerClearRound)
        // {
        //     GlobalVariable.Instance.PlayerClearRound = GlobalVariable.Instance.PlayerCurrentPlayerStage;
        // }

        // 3. 클리어 UI 띄움
        GameClearPanel.SetActive(true);

        // 4. 모든 애니메이션, 움직임 정지
        AnimationManager.Instance.AnimationAllStop();

        // 5. 진행상황 저장
        GlobalVariable.Instance.SaveGame();
    }

    /// <summary>
    /// 남은 거리를 계산하여 UI 텍스트를 업데이트합니다.
    /// GameModeManager가 호출해 줄 함수입니다.
    /// </summary>
    /// <param name="remainingDistance">표시할 남은 거리 값</param>
    public void UpdateRemainingDistanceUI(int remainingDistance)
    {
        if (distanceText != null)
        {
            distanceText.text = $"{remainingDistance} M";
        }
    }
}
