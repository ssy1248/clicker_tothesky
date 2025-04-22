using System;
using BigNumber;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameViewManager : MonoBehaviour
{
    public Animator kiwiAnim;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI gameTimeText;
    private float totalTime;
    private int totalDistance = 500;
    private int currentDistance =0;
    
    private void Addkiwi(BigDouble amt)
    {
        GlobalManager.Instance.kiwiAmount += amt;
    }
    

    private void Awake()
    {
        totalTime = GlobalManager.Instance.inGameCountTime;
        slider = GameObject.Find("Slider").GetComponent<Slider>();
        gameTimeText = GameObject.Find("GameTimeText").GetComponent<TextMeshProUGUI>();
        GameTimeCaculater(Mathf.FloorToInt(totalTime));
    }

    private void GameTimeCaculater(int time)
    {
        int minutes = (time / 60);
        int seconds = 0;
        if ((time % 60) > 0)
        {
            seconds = (time % 60);
        }
        else
        {
            seconds = 0;
        }
        gameTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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
        if (totalTime <= 0f)
        {
            totalTime = 0f;
            // 게임 종료 처리
            Debug.Log("Time's up! 게임끝 게임엔드");
        }

        if (totalDistance == currentDistance)
        {
            //상점 패널로 점프
            //UIManager.Instance.PushPanel(UIPanelType.ITEM_PANEL);
        }
        
        totalTime -= Time.deltaTime;
        if (slider.value > 0)
        {
            slider.value -= Time.deltaTime * 3f;
            if (slider.value <= 50)
            {
                GlobalManager.Instance.speedGain = 1;
            }

            if (slider.value <= 70&&slider.value>50)
            {
                GlobalManager.Instance.speedGain = 2;
            }
            if (slider.value <= 100&&slider.value>70)
            {
                GlobalManager.Instance.speedGain = 3;
            }
        }
        GameTimeCaculater(Mathf.FloorToInt(totalTime));
        
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
            currentDistance += GlobalManager.Instance.speedGain;
            kiwiAnim.Play(stateName:"touch", layer: 0, normalizedTime:0);
        }
       

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log(message: "ClickUp");
        }
        
    }
}
