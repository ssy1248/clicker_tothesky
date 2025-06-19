using UnityEngine;
using UnityEngine.UI;

public class GuageManager : MonoBehaviour
{
    [Header("게이지 UI")]
    [SerializeField]
    private Image touchGaugeImage;

    [Header("게이지 설정")]
    [SerializeField]
    private float increaseRate = 0.1f;   // 초당 자동 증가
    [SerializeField]
    private float touchDecrease = 0.05f; // 터치 시 감소량
    private float gaugeValue = 0f;

    private const float MIN_FILL = 0.22f;
    private const float MAX_FILL = 0.927f;

    private void Update()
    {
        // 자동 증가
        gaugeValue += increaseRate * Time.deltaTime;
        gaugeValue = Mathf.Clamp01(gaugeValue);
        UpdateGaugeUI();
    }

    public void OnTouch()
    {
        gaugeValue -= touchDecrease;
        gaugeValue = Mathf.Clamp01(gaugeValue);
        UpdateGaugeUI();
    }

    private void UpdateGaugeUI()
    {
        float normalized = Mathf.Clamp01(gaugeValue);
        touchGaugeImage.fillAmount = Mathf.Lerp(MIN_FILL, MAX_FILL, normalized);

        // 색상 설정
        Color c;
        if (normalized < 0.5f) c = Color.green;
        else if (normalized < 0.8f) c = Color.yellow;
        else c = Color.red;

        GuageColorController.Instance.SetGaugeColor(c);
    }
}
