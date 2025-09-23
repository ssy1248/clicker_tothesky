using System;
using UnityEngine;
using UnityEngine.UI;

public class QuickTabGame : SubTouch
{
    [Header("QuickTap 전용")]
    public Image quickTapCircle;
    public Button quickTapButton;
    public float scaleDecreasePerTap = 0.1f;
    public float targetScale = 0.2f;

    private int quickTapCount = 0;

    // 부모의 Initialize 함수를 확장하여 사용
    public override void Initialize(int score, Action onEnded = null)
    {
        base.Initialize(score); // 부모의 기본 초기화(점수 할당) 실행

        quickTapCount = 0;
        quickTapCircle.transform.localScale = Vector3.one;
        quickTapButton.onClick.AddListener(OnQuickTap);
    }

    private void OnQuickTap()
    {
        quickTapCount++;
        quickTapCircle.transform.localScale -= Vector3.one * scaleDecreasePerTap;

        if (quickTapCircle.transform.localScale.x <= targetScale)
        {
            int finalScore = quickTapCount * 2;
            Debug.Log($"QuickTap 클리어! 획득 점수: {finalScore}");
            ScoreManager.Instance.AddScore(finalScore);
            EndMiniGame(); // 부모의 공통 종료 함수 호출
        }
    }
}
