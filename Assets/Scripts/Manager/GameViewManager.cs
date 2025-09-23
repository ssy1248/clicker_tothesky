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

    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI gameTimeText;
    public GameObject GameOverPanel;
    public GameObject GameClearPanel;

    [Header("타임 패널")]
    [SerializeField]
    private TimePanel timePanel;

    private float totalTime;
    private float initialTotalTime;

    [Header("게이지 매니저")]
    [SerializeField]
    private GuageManager gaugeManager;

    [Header("아이템 관련 변수")]
    public float GameTimePlus = 0;

    // 다른 스크립트에서 게임이 끝났는지(클리어 또는 오버) 확인할 수 있도록 해주는 프로퍼티
    public bool IsGameFinished => gameOver || gameClear;


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
    }

    public void ResetTimer(int seconds)
    {
        totalTime = seconds;
        UpdateTimerUI();
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

        // 바로 게임 오버가 아닌 점수 계산 후 클리어 또는 게임 오버 처리

        // 게임 오버 UI 띄움
        //GameOverPanel.SetActive(true);
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
        }

        // 스테이지 종료
        if (totalTime <= 0f)
        {
            // 시간이 다 되면 스테이지 완료 처리하고 점수 나오면서 클리어, 게임 오버 처리
            OnGameOver();
            return; // 게임 오버 처리 후 즉시 Update 종료
        }

        if (EventSystem.current.IsPointerOverGameObject())
            return;
    }

    private void OnGameClear()
    {
        gameClear = true; // 클리어 플래그 설정
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
}
