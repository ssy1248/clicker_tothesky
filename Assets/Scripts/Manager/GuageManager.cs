using UnityEngine;
using UnityEngine.UI;

public class GuageManager : MonoBehaviour
{
    [Header("게이지 UI")]
    [SerializeField]
    private Image touchGaugeImage; // 지금은 차오르는 스프라이트만 가져오지만 나중에 배경까지 가져와서 알파값 건들어야 할듯

    [Header("게이지 설정")]
    [SerializeField]
    private float increaseRate = 0.1f;   // 초당 자동 증가
    [SerializeField]
    private float touchDecrease = 0.05f; // 터치 시 감소량

    [SerializeField, Range(0.11f, 0.89f)] // 안전한 범위 내에서만 조절 가능하게 설정
    private float startingGaugeValue = 0.15f;

    private float gaugeValue;

    private float zoneTimer = 0f;
    private bool isInDangerZone = false;
    private bool hasTriggeredBlink = false;
    private bool hasStoppedAnimation = false;

    public float DANGER_THRESHOLD_LOW = 0.1f;
    public float DANGER_THRESHOLD_HIGH = 0.9f;
    public float DANGER_DURATION = 5f;

    public float GaugeValue => gaugeValue;

    private void Start()
    {
        gaugeValue = startingGaugeValue;

        // 색상 설정
        Color c = Color.green;

        GuageColorController.Instance.SetGaugeColor(c);
    }

    private void Update()
    {
        // 1. 현재 게이지 위치에 따른 속도 배율을 결정합니다.
        float rateMultiplier = 1.0f; // 기본 배율은 1배

        // 게이지의 1/3 지점 (약 0.333)
        const float LOWER_THIRD = 1f / 3f;
        // 게이지의 2/3 지점 (약 0.666)
        const float UPPER_THIRD = 2f / 3f;

        if (gaugeValue <= LOWER_THIRD)
        {
            // 아래쪽 구간(0 ~ 1/3): 1배속
            rateMultiplier = 1.0f;
        }
        else if (gaugeValue <= UPPER_THIRD)
        {
            // 중간 구간(1/3 ~ 2/3): 1.5배속
            rateMultiplier = 1.5f;
        }
        else
        {
            // 위쪽 구간(2/3 ~ 1): 2배속
            rateMultiplier = 2.0f;
        }

        // 2. 결정된 배율을 적용하여 게이지 값을 증가시킵니다.
        gaugeValue += increaseRate * rateMultiplier * Time.deltaTime;
        gaugeValue = Mathf.Clamp01(gaugeValue);
        UpdateGaugeUI();

        // 3. 위험 구간 로직은 그대로 유지됩니다.
        bool isDangerNow = gaugeValue <= DANGER_THRESHOLD_LOW || gaugeValue >= DANGER_THRESHOLD_HIGH;

        if (isDangerNow)
        {
            if (!isInDangerZone)
            {
                Debug.Log("위험구간 진입");
                isInDangerZone = true;
                zoneTimer = 0f;
                hasTriggeredBlink = false;
                hasStoppedAnimation = false;

                GuageImageAlpha.Instance.StartLifeRoutine();
            }

            zoneTimer += Time.deltaTime;

            if (zoneTimer >= DANGER_DURATION && !hasStoppedAnimation)
            {
                hasStoppedAnimation = true;

                AnimationManager.Instance.AnimationAllStop();

                GuageImageAlpha.Instance.StartZeroRoutine(() => {
                    gaugeValue = 0.2f;
                    UpdateGaugeUI();
                });
            }
        }
        else
        {
            if (isInDangerZone)
            {
                isInDangerZone = false;
                zoneTimer = 0f;

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
        touchGaugeImage.fillAmount = gaugeValue;
    }
}
