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

    private float zoneTimer = 0f;
    private bool isInDangerZone = false;
    private bool hasTriggeredBlink = false;
    private bool hasStoppedAnimation = false;

    private const float DANGER_THRESHOLD_LOW = 0.05f;
    private const float DANGER_THRESHOLD_HIGH = 0.95f;
    private const float DANGER_DURATION = 5f;

    private void Start()
    {
        // 색상 설정
        Color c = Color.green;

        GuageColorController.Instance.SetGaugeColor(c);
    }

    private void Update()
    {
        // 자동 증가
        gaugeValue += increaseRate * Time.deltaTime;
        gaugeValue = Mathf.Clamp01(gaugeValue);
        UpdateGaugeUI();

        // 위험 구간 감지
        if (gaugeValue <= DANGER_THRESHOLD_LOW || gaugeValue >= DANGER_THRESHOLD_HIGH)
        {
            if (!isInDangerZone)
            {
                // 새로 진입
                isInDangerZone = true;
                zoneTimer = 0f;
                hasTriggeredBlink = false;
                hasStoppedAnimation = false;
            }

            zoneTimer += Time.deltaTime;

            if (zoneTimer >= DANGER_DURATION)
            {
                if (!hasTriggeredBlink)
                {
                    GuageImageAlpha.Instance.StartLifeRoutine();
                    hasTriggeredBlink = true;
                }

                if (!hasStoppedAnimation)
                {
                    AnimationManager.Instance.AnimationAllStop();
                    hasStoppedAnimation = true;
                }
            }
        }
        else
        {
            if (isInDangerZone)
            {
                // 구간 이탈 시 초기화
                isInDangerZone = false;
                zoneTimer = 0f;

                // 깜빡임 종료, 애니 재개
                GuageImageAlpha.Instance.CancelLifeRoutine();
                AnimationManager.Instance.AnimationAllPlay();
            }
        }
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
    }
}
