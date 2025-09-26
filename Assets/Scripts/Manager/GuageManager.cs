using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GuageManager : MonoBehaviour
{
    [Header("게이지 UI")]
    [SerializeField]
    private Image touchGaugeImage;

    [Header("게이지 설정")]
    [SerializeField, Range(0f, 1f)]
    private float startingGaugeValue = 0; // 시작 게이지 값

    [Header("피버 타임 설정")]
    [SerializeField]
    private float feverDuration = 5f; // 피버 타임 지속 시간
    private bool isFeverTime = false;

    private float gaugeValue;

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

        gaugeValue = Mathf.Clamp01(gaugeValue);
        UpdateGaugeUI();
    }

    public void OnTouch()
    {
        // 피버 타임 중에는 터치로 게이지를 올릴 수 없음
        if (isFeverTime)
        {
            return;
        }

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

    public void AddFeverPercent(float delta)
    {
        // 피버 중일땐 게이지 변화 없음
        if (isFeverTime) 
            return;

        gaugeValue = Mathf.Clamp01(gaugeValue + delta);
        UpdateGaugeUI();

        Debug.Log($"[Fever] 게이지 변경됨: {gaugeValue * 100f:0}%");

        // 임계치 도달 시 피버 시작
        if (gaugeValue >= 1f)
        {
            StartCoroutine(FeverCoroutine());
        }
    }

    public void DebugForceStartFever()
    {
        if (isFeverTime) 
            return;
        // 바로 피버 시작
        StartCoroutine(FeverCoroutine());
    }

    public void DebugForceEndFever()
    {
        if (!isFeverTime) 
            return;

        // 진행 중인 피버 코루틴 정지 후 강제 종료 상태로 세팅
        StopAllCoroutines();
        isFeverTime = false;
        Debug.Log("피버 타임 강제 종료(디버그)");
        GuageImageAlpha.Instance.TriggerFeverEnd();

        gaugeValue = startingGaugeValue;
        UpdateGaugeUI();
        GuageColorController.Instance.SetGaugeColor(Color.green);
    }
}
