using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TouchGame : SubTouch
{
    [Header("Touch 전용")]
    public TextMeshProUGUI touchCountText;
    public Button touchAreaButton;
    private int targetTouchCount = 5;
    private int currentTouchCount = 0;

    /// <summary>
    /// SubTouchManager가 호출하는 초기화 함수입니다.
    /// </summary>
    public override void Initialize(int score)
    {
        base.Initialize(score); // 부모의 Initialize 함수를 호출하여 successScore를 설정합니다.
        SetupTouch();           // Touch 게임에 필요한 설정을 시작합니다.
    }

    // --- Touch 로직 ---
    private void SetupTouch()
    {
        currentTouchCount = 0;
        UpdateTouchUI();
        touchAreaButton.onClick.AddListener(OnTouch);
    }

    private void OnTouch()
    {
        currentTouchCount++;
        UpdateTouchUI();

        if (currentTouchCount >= targetTouchCount)
        {
            Debug.Log($"Touch 클리어! 획득 점수: {successScore}");
            ScoreManager.Instance.AddScore(successScore);
            EndMiniGame(); // 부모의 공통 종료 함수 호출
        }
    }

    private void UpdateTouchUI()
    {
        touchCountText.text = $"{currentTouchCount} / {targetTouchCount}";
    }
}
