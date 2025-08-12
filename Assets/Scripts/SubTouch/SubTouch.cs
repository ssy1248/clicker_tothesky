using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubTouch : MonoBehaviour
{
    [Header("미니게임 공통 설정")]
    public SubTouchType subTouchType; // 이 프리팹의 미니게임 종류
    private int successScore; // 성공 시 획득할 기본 점수

    [Header("QuickTap 전용")]
    public Image quickTapCircle;
    public Button quickTapButton;
    public float scaleDecreasePerTap = 0.1f;
    public float targetScale = 0.2f;
    private int quickTapCount = 0;

    [Header("Touch 전용")]
    public TextMeshProUGUI touchCountText;
    public Button touchAreaButton;
    private int targetTouchCount = 10;
    private int currentTouchCount = 0;

    [Header("Memory 전용")]
    public Button[] memoryButtons = new Button[4];
    public TextMeshProUGUI[] memoryButtonTexts = new TextMeshProUGUI[4];
    public float showNumberDuration = 2f;
    private List<int> memoryNumbers = new List<int>();
    private List<int> sortedMemoryNumbers = new List<int>();
    private int nextNumberIndex = 0;

    /// <summary>
    /// SubTouchManager가 미니게임을 시작시킬 때 호출하는 초기화 함수
    /// </summary>
    public void Initialize(int score)
    {
        this.successScore = score;

        // 타입에 따라 각기 다른 초기화 함수를 호출
        switch (subTouchType)
        {
            case SubTouchType.QuickTap:
                SetupQuickTap();
                break;
            case SubTouchType.Touch:
                SetupTouch();
                break;
            case SubTouchType.Memory:
                SetupMemory();
                break;
        }
    }

    // --- QuickTap 로직 ---
    private void SetupQuickTap()
    {
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
            int finalScore = quickTapCount * 2; // 터치 수 * 2배 점수
            Debug.Log($"QuickTap 클리어! 획득 점수: {finalScore}");
            ScoreManager.Instance.AddScore(finalScore);
            EndMiniGame();
        }
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
            EndMiniGame();
        }
    }

    private void UpdateTouchUI()
    {
        touchCountText.text = $"{currentTouchCount} / {targetTouchCount}";
    }

    // --- Memory 로직 ---
    private void SetupMemory()
    {
        // 1. 랜덤 숫자 4개 생성
        List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        memoryNumbers.Clear();
        for (int i = 0; i < 4; i++)
        {
            int randIndex = Random.Range(0, numbers.Count);
            memoryNumbers.Add(numbers[randIndex]);
            numbers.RemoveAt(randIndex);
        }

        // 2. 정답 순서 저장
        sortedMemoryNumbers = memoryNumbers.OrderBy(n => n).ToList();
        nextNumberIndex = 0;

        // 3. 버튼에 숫자 표시 및 리스너 연결
        for (int i = 0; i < memoryButtons.Length; i++)
        {
            int buttonIndex = i; // 클로저 문제 방지
            memoryButtonTexts[i].text = memoryNumbers[i].ToString();
            memoryButtons[i].onClick.AddListener(() => OnMemoryButtonClick(buttonIndex));
        }

        // 4. 일정 시간 뒤 숫자 숨기기
        StartCoroutine(HideNumbersCoroutine());
    }

    private IEnumerator HideNumbersCoroutine()
    {
        yield return new WaitForSeconds(showNumberDuration);
        foreach (var txt in memoryButtonTexts)
        {
            txt.text = "?";
        }
    }

    private void OnMemoryButtonClick(int buttonIndex)
    {
        int clickedNumber = memoryNumbers[buttonIndex];

        // 정답을 맞췄을 경우
        if (clickedNumber == sortedMemoryNumbers[nextNumberIndex])
        {
            memoryButtonTexts[buttonIndex].text = clickedNumber.ToString(); // 맞춘 숫자 다시 보여주기
            nextNumberIndex++;

            // 모든 숫자를 다 맞췄으면 클리어
            if (nextNumberIndex >= sortedMemoryNumbers.Count)
            {
                Debug.Log($"Memory 클리어! 획득 점수: {successScore}");
                ScoreManager.Instance.AddScore(successScore);
                EndMiniGame();
            }
        }
        // 틀렸을 경우
        else
        {
            Debug.Log("Memory 실패!");
            // (선택) 실패 시 패널티나 효과 추가
            EndMiniGame();
        }
    }

    /// <summary>
    /// 미니게임을 종료하고 오브젝트를 파괴합니다.
    /// </summary>
    private void EndMiniGame()
    {
        // SubTouchManager에게 종료를 알리는 로직 추가 가능
        // SubTouchManager.Instance.OnMiniGameFinished();
        Destroy(gameObject);
    }
}
