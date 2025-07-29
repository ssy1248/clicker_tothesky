using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GuageImageAlpha : MonoBehaviour
{
    public static GuageImageAlpha Instance { get; private set; }

    public static event System.Action OnStaminaEmpty;
    public static event System.Action OnStaminaRecovered;

    public static event System.Action OnFeverStart;
    public static event System.Action OnFeverEnd;

    [Header("이미지 참조")]
    public Image FilledGuageImage;
    public Image BackgroundGuageImage;

    [Header("라이프타임 세팅")]
    public float lifeTime = 3f;  // 왕복 지속 시간
    public float resetDelay = 5f;  // 스테미나 제로 후 리셋까지 딜레이
    public float baseSpeed = 1f;  // 초기 왕복 스피드
    public float acceleration = 1f;  // 초당 스피드 증가량

    private Coroutine lifeRoutine;

    [Header("아이템 변수 관련")]
    public float RecoverTime = 0;

    private void Awake()
    {
        Instance = this;
    }

    // 임시 라이프 타임 5초 / 게이지 밸류가 0.8이 되는 순간 시작 255~0 을 왕복 시간이 지날수록 더 빠르게
    // 시간이 다되면 PlayerStaminaZero 함수 호출
    // 일정 시간(임시 3초) 후 PlayerStaminaReset 함수 호출
    public void StartLifeRoutine()
    {
        if (lifeRoutine == null)
            lifeRoutine = StartCoroutine(LifeCoroutine());
    }

    public void CancelLifeRoutine()
    {
        if (lifeRoutine != null)
        {
            StopCoroutine(lifeRoutine);
            lifeRoutine = null;
            // 즉시 완전 회복
            PlayerStaminaReset();
        }
    }

    private IEnumerator LifeCoroutine()
    {
        float t = 0f;

        while (true) // 무한 깜빡임
        {
            t += Time.deltaTime;
            float speed = baseSpeed + acceleration * t;
            float alpha = Mathf.PingPong(t * speed, 1f);
            SetAlpha(alpha);
            yield return null;
        }
    }

    private void SetAlpha(float a)
    {
        // Image.color.a는 0~1 사이
        var c1 = FilledGuageImage.color;
        c1.a = a;
        FilledGuageImage.color = c1;

        var c2 = BackgroundGuageImage.color;
        c2.a = a;
        BackgroundGuageImage.color = c2;
    }

    public void StartZeroRoutine(System.Action onReset = null)
    {
        if (lifeRoutine != null)
            StopCoroutine(lifeRoutine);

        lifeRoutine = StartCoroutine(ZeroAndResetCoroutine(onReset));
    }

    private IEnumerator ZeroAndResetCoroutine(System.Action onReset)
    {
        PlayerStaminaZero();  // 알파값 0 (게이지 숨기기)

        // resetDelay에 아이템 효과 값인 RecoverTime을 더해서 대기 시간을 계산합니다.
        float totalDelay = resetDelay + RecoverTime;
        Debug.Log($"총 회복 대기 시간: {totalDelay}초 (기본: {resetDelay} + 아이템: {RecoverTime})");

        yield return new WaitForSeconds(totalDelay);
        PlayerStaminaReset();  // 알파값 1 (게이지 보이기)
        lifeRoutine = null;

        onReset?.Invoke();  // 예: 게이지 값을 0.2로
    }

    public void PlayerStaminaZero()
    {
        // FilledGuageImage, BackgroundGuageImage 의 alpha 값 -> 0으로 변경
        SetAlpha(0f);

        OnStaminaEmpty?.Invoke();
    }

    public void PlayerStaminaReset()
    {
        // FilledGuageImage, BackgroundGuageImage 의 alpha 값 -> 255으로 변경
        SetAlpha(1f);

        OnStaminaRecovered?.Invoke();
    }

    /// <summary>
    /// 외부에서 피버 타임 시작을 요청할 때 호출하는 함수입니다.
    /// </summary>
    public void TriggerFeverStart()
    {
        // OnFeverStart 이벤트를 '알림'합니다.
        OnFeverStart?.Invoke();
    }

    /// <summary>
    /// 외부에서 피버 타임 종료를 요청할 때 호출하는 함수입니다.
    /// </summary>
    public void TriggerFeverEnd()
    {
        // OnFeverEnd 이벤트를 '알림'합니다.
        OnFeverEnd?.Invoke();
    }
}
