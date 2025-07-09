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
    private bool isStaminaEmpty = false;
    private bool inputEnabled = true;

    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI gameTimeText;

    [Header("타임 패널")]
    [SerializeField]
    private TimePanel timePanel;
    private bool blinkStarted = false;

    private float totalTime;

    [Header("게이지 매니저")]
    [SerializeField]
    private GuageManager gaugeManager;

    [Header("아이템 관련 변수")]
    public float GameTimePlus = 0;

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
        UpdateTimerUI();
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

        GlobalVariable.Instance.CheckPointDistance = 0;
        GlobalVariable.Instance.PlayerCurrentDistance = 0;
    }

    void Update()
    {
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

            if (totalTime == 0f)
                OnGameOver();
        }

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!inputEnabled)
                return;

            //SEManager.instance.PlaySE("click");

            Debug.Log("ClickDown");

            // 게이지 감소는 GaugeManager에서 관리
            gaugeManager.OnTouch();

            lastclickupdate = 0f;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("ClickUp");
        }
    }
}
