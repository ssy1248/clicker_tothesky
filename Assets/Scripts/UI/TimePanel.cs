using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimePanel : MonoBehaviour
{
    [Header("������ Panel�� Image")]
    [SerializeField] private Image flashPanel;

    [Header("������ ����(seconds)")]
    [SerializeField] private float blinkInterval = 0.5f;

    [Header("�ִ� ����(0~255)")]
    [SerializeField, Range(0, 255)] private byte maxAlphaByte = 100;

    private float maxAlphaNormalized;
    private Coroutine blinkCoroutine;

    private void Awake()
    {
        // 0~255byte �� 0~1 ������ ��ȯ
        maxAlphaNormalized = maxAlphaByte / 255f;
        SetAlpha(0f);  // �ʱ⿡�� ���� ����
    }

    public void StartBlinking()
    {
        if (blinkCoroutine == null)
            blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        // ���� �ִ� ���ķ� ���̰�
        SetAlpha(maxAlphaNormalized);

        while (true)
        {
            yield return new WaitForSeconds(blinkInterval);
            // ���� ���ĸ� ���� ���
            float current = flashPanel.color.a;
            if (current > maxAlphaNormalized * 0.5f)
                SetAlpha(0f);                // ���� �̻��̸� ����
            else
                SetAlpha(maxAlphaNormalized); // �ƴϸ� maxAlphaByte
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
        SetAlpha(0f);  // ���� ���� ����
    }
}
