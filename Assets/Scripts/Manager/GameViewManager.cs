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

    [Header("UI")]
    // Radial 360 세팅된 Image
    [SerializeField] private Image touchGaugeImage;     
    [SerializeField] private TextMeshProUGUI gameTimeText;

    [Header("게이지 세팅")]
    // 게이지가 초당 이만큼 줄어듦
    [SerializeField] private float decayRate = 0.1f;
    // 최소 fillAmount
    const float MIN_FILL = 0.22f;
    // 최대 fillAmount
    const float MAX_FILL = 0.927f;

    private float totalTime;

    // 내부 게이지 값 (0~∞) → 0~1 로 클램프
    public float gaugeValue = 0f;  // -> 0.7 이상이 되면 GameModeManager에 가져와서 거리 이속 저하

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

    private void UpdateGauge()
    {
        // 내부 값(gaugeValue)을 0~1로 정규화
        float normalized = Mathf.Clamp01(gaugeValue);
        // 0~1 사이를 MIN_FILL~MAX_FILL 사이로 보간
        touchGaugeImage.fillAmount = Mathf.Lerp(MIN_FILL, MAX_FILL, normalized);
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
    }

    private void UpdateTimerUI()
    {
        int t = Mathf.FloorToInt(totalTime);
        int minutes = t / 60;
        int secs = t % 60;
        gameTimeText.text = $"{minutes:00}:{secs:00}";
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

            if (totalTime == 0f)
                Debug.Log("Time's up! 게임 끝");
        }

        // 2) 게이지 자동 감소
        if (gaugeValue > 0f)
        {
            gaugeValue -= decayRate * Time.deltaTime;
            if (gaugeValue < 0f) 
                gaugeValue = 0f;
            UpdateGauge();  // fillAmount 갱신
        }

        // UI 위 터치는 무시
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("ClickDown");
            Addkiwi(GlobalManager.Instance.GetTouchAmount());

            // ★ 클릭할 때마다 0.05 더하되 0~1 사이로 클램프
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
