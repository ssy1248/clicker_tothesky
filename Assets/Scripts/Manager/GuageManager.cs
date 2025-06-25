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
    private float gaugeValue = 0f;

    private const float MIN_FILL = 0.22f;
    private const float MAX_FILL = 0.927f;

    private float zoneTimer = 0f;
    private bool isInDangerZone = false;
    private bool hasTriggeredBlink = false;
    private bool hasStoppedAnimation = false;

    private const float DANGER_THRESHOLD_LOW = 0.1f;
    private const float DANGER_THRESHOLD_HIGH = 0.9f;
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

                // 진입 즉시 깜빡임 시작
                GuageImageAlpha.Instance.StartLifeRoutine();
            }

            zoneTimer += Time.deltaTime;

            if (zoneTimer >= DANGER_DURATION && !hasStoppedAnimation)
            {
                hasStoppedAnimation = true;

                // 애니메이션도 멈추고
                AnimationManager.Instance.AnimationAllStop();

                // 깜빡임 중지 및 게이지 숨김 후 회복 루틴 시작
                GuageImageAlpha.Instance.StartZeroRoutine(() => {
                    // 콜백에서 게이지 값 초기화
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
        float normalized = Mathf.Clamp01(gaugeValue);
        touchGaugeImage.fillAmount = Mathf.Lerp(MIN_FILL, MAX_FILL, normalized);
    }
}
