using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GuageManager : MonoBehaviour
{
    [Header("게이지 UI")]
    [SerializeField]
    private Image touchGaugeImage;

    [Header("게이지 설정")]
    [SerializeField]
    private float decreaseRate = 0.05f;  // 초당 자동 감소량
    [SerializeField]
    private float touchIncrease = 0.1f;  // 터치 시 증가량
    [SerializeField, Range(0f, 1f)]
    private float startingGaugeValue = 0.5f; // 시작 게이지 값

    [Header("피버 타임 설정")]
    [SerializeField]
    private float feverDuration = 5f; // 피버 타임 지속 시간
    private bool isFeverTime = false;

    private float gaugeValue;
    private bool hasStoppedAnimation = false; // 스태미나 0 중복 실행 방지

    public float GaugeValue => gaugeValue;

    private void Start()
    {
        gaugeValue = startingGaugeValue;
        UpdateGaugeUI();
        GuageColorController.Instance.SetGaugeColor(Color.green); // 색상 초기화
    }

    private void Update()
    {
        // 피버 타임 중에는 게이지가 변하지 않음
        if (isFeverTime)
        {
            return;
        }

        // 게이지 자동 감소
        gaugeValue -= decreaseRate * Time.deltaTime;
        gaugeValue = Mathf.Clamp01(gaugeValue);
        UpdateGaugeUI();

        // 게이지가 0이 되면 스태미나 고갈 처리
        //if (gaugeValue <= 0f && !hasStoppedAnimation)
        //{
        //    hasStoppedAnimation = true;
        //    // 기존의 스태미나 0 처리 루틴을 재사용
        //    GuageImageAlpha.Instance.StartZeroRoutine(() => {
        //        gaugeValue = startingGaugeValue; // 게이지 회복
        //        UpdateGaugeUI();
        //        hasStoppedAnimation = false; // 다시 체크 가능하도록 플래그 리셋
        //    });
        //}
    }

    public void OnTouch()
    {
        // 피버 타임 중에는 터치로 게이지를 올릴 수 없음
        if (isFeverTime)
        {
            return;
        }

        gaugeValue += touchIncrease;
        gaugeValue = Mathf.Clamp01(gaugeValue);
        UpdateGaugeUI();

        // 게이지가 100%에 도달하면 피버 타임 시작
        if (gaugeValue >= 1f)
        {
            StartCoroutine(FeverCoroutine());
        }
    }

    /// <summary>
    /// 피버 타임을 관리하는 코루틴
    /// </summary>
    private IEnumerator FeverCoroutine()
    {
        isFeverTime = true;
        Debug.Log("피버 타임 시작!");
        GuageImageAlpha.Instance.TriggerFeverStart();

        // 피버 타임 시각 효과
        GuageColorController.Instance.SetGaugeColor(Color.red);

        // 피버 타임 지속 시간만큼 대기
        yield return new WaitForSeconds(feverDuration);

        // 피버 타임 종료
        isFeverTime = false;
        Debug.Log("피버 타임 종료!");
        GuageImageAlpha.Instance.TriggerFeverEnd();

        // 게이지를 시작 값으로 리셋하고 원래 색으로 복귀
        gaugeValue = startingGaugeValue;
        UpdateGaugeUI();
        GuageColorController.Instance.SetGaugeColor(Color.green);
    }

    private void UpdateGaugeUI()
    {
        touchGaugeImage.fillAmount = gaugeValue;
    }
}
