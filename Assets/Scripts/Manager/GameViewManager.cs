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
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI gameTimeText;
    private float totalTime;
    
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

    private void Awake()
    {
        totalTime = GlobalManager.Instance.inGameCountTime;
        slider = GameObject.Find("Slider").GetComponent<Slider>();
        gameTimeText = GameObject.Find("GameTimeText").GetComponent<TextMeshProUGUI>();
        UpdateTimerUI();
    }

    public void ResetTimer(int seconds)
    {
        totalTime = seconds;   // ← 여기에 120을 넣으면 120초로 세팅
        slider.value = 0f;     // 필요시 슬라이더도 초기화
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
        slider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (totalTime > 0f)
        {
            totalTime -= Time.deltaTime;
            if (totalTime < 0f)
                totalTime = 0f;

            UpdateTimerUI();

            if (totalTime == 0f)
                Debug.Log("Time's up! 게임 끝");
        }

        if (slider.value > 0)
        {
            slider.value -= Time.deltaTime * 3f;    
        }

        if (EventSystem.current.IsPointerOverGameObject())
            return;
        //터치 시작.
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Debug.Log(message: "Click");
        // }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(message: "ClickDown");
            Addkiwi(amt:GlobalManager.Instance.GetTouchAmount());
            slider.value += 1f;
            lastclickupdate = 0f;
            kiwiAnim.Play(stateName:"touch", layer: 0, normalizedTime:0);
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
