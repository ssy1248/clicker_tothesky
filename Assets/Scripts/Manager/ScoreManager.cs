using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // 싱글톤 패턴으로 어디서든 쉽게 접근 가능하게 함
    public static ScoreManager Instance { get; private set; }

    [Header("UI 연결")]
    public TextMeshProUGUI scoreText;

    // 현재 게임의 점수를 저장하는 변수
    private int currentScore = 0;

    private void Awake()
    {
        // 싱글톤 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // 게임 시작 시 점수를 0으로 초기화하고 UI를 업데이트
        currentScore = 0;
        UpdateScoreUI();
    }

    /// <summary>
    /// 외부에서 점수를 추가할 때 호출하는 공용 함수입니다.
    /// </summary>
    /// <param name="amount">추가할 점수량</param>
    public void AddScore(int amount)
    {
        // 피버 중이면 양수 점수만 2배 (미스는 그대로)
        if (GameModeManager.Instance.IsFeverTime)
            amount *= 2;

        float comboMultiplier = ComboManager.Instance != null ? ComboManager.Instance.GetComboMultiplier() : 1f;

        int finalScore = Mathf.RoundToInt(amount * comboMultiplier);

        currentScore += finalScore;

        if (currentScore < 0)
            currentScore = 0;

        UpdateScoreUI();
        Debug.Log($"점수 획득! {amount}점 추가, 현재 점수: {currentScore}");
    }

    /// <summary>
    /// 현재 점수를 기반으로 UI 텍스트를 업데이트합니다.
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            // "현재 점수 / 목표 점수" 형식으로 텍스트를 표시
            scoreText.text = $"{currentScore} / {GlobalVariable.Instance.GameClearScore}";
        }
    }
}
