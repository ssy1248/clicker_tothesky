using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryGame : SubTouch
{
    [Header("Memory 전용")]
    public Button[] memoryButtons = new Button[4];
    public TextMeshProUGUI[] memoryButtonTexts = new TextMeshProUGUI[4];
    public float showNumberDuration = 2f;
    private List<int> memoryNumbers = new List<int>();
    private List<int> sortedMemoryNumbers = new List<int>();
    private int nextNumberIndex = 0;

    /// <summary>
    /// SubTouchManager가 호출하는 초기화 함수입니다.
    /// 부모의 Initialize를 먼저 실행하고, 그 다음 Memory 게임 전용 설정을 합니다.
    /// </summary>
    public override void Initialize(int score)
    {
        base.Initialize(score); // 부모의 Initialize 함수를 호출하여 successScore를 설정합니다.
        SetupMemory();          // Memory 게임에 필요한 설정을 시작합니다.
    }

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
        // 이미 눌린 버튼(정답)은 다시 누를 수 없도록 막습니다.
        if (!memoryButtons[buttonIndex].interactable) return;

        int clickedNumber = memoryNumbers[buttonIndex];

        // 정답을 맞췄을 경우
        if (clickedNumber == sortedMemoryNumbers[nextNumberIndex])
        {
            memoryButtonTexts[buttonIndex].text = clickedNumber.ToString(); // 맞춘 숫자 다시 보여주기
            memoryButtons[buttonIndex].interactable = false; // 맞춘 버튼은 비활성화
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
}
