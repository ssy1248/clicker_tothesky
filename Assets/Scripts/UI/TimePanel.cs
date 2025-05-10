using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimePanel : MonoBehaviour
{
    [Header("깜박일 Panel의 Image")]
    [SerializeField] private Image flashPanel;

    [Header("깜박임 간격(seconds)")]
    [SerializeField] private float blinkInterval = 0.5f;

    [Header("최대 알파(0~255)")]
    [SerializeField, Range(0, 255)] private byte maxAlphaByte = 100;

    private float maxAlphaNormalized;
    private Coroutine blinkCoroutine;

    private void Awake()
    {
        // 0~255byte → 0~1 범위로 변환
        maxAlphaNormalized = maxAlphaByte / 255f;
        SetAlpha(0f);  // 초기에는 완전 투명
    }

    public void StartBlinking()
    {
        if (blinkCoroutine == null)
            blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        // 먼저 최대 알파로 보이게
        SetAlpha(maxAlphaNormalized);

        while (true)
        {
            yield return new WaitForSeconds(blinkInterval);
            // 현재 알파를 보고 토글
            float current = flashPanel.color.a;
            if (current > maxAlphaNormalized * 0.5f)
                SetAlpha(0f);                // 반절 이상이면 투명
            else
                SetAlpha(maxAlphaNormalized); // 아니면 maxAlphaByte
        }
    }

    private void SetAlpha(float normalized)
    {
        Color c = flashPanel.color;
        c.a = normalized;
        flashPanel.color = c;
    }

    public void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        SetAlpha(0f);  // 멈출 때는 투명
    }
}
