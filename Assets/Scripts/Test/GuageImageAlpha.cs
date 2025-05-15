using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GuageImageAlpha : MonoBehaviour
{
    public static GuageImageAlpha Instance { get; private set; }

    public static event System.Action OnStaminaEmpty;
    public static event System.Action OnStaminaRecovered;

    [Header("이미지 참조")]
    public Image FilledGuageImage;
    public Image BackgroundGuageImage;

    [Header("라이프타임 세팅")]
    public float lifeTime = 5f;  // 왕복 지속 시간
    public float resetDelay = 3f;  // 스테미나 제로 후 리셋까지 딜레이
    public float baseSpeed = 1f;  // 초기 왕복 스피드
    public float acceleration = 1f;  // 초당 스피드 증가량

    private Coroutine lifeRoutine;

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
        // 지금 알파값이 빨라지면서 왕복을 하는데 5초를 버티면 다시 반복함
        while (t < lifeTime)
        {
            t += Time.deltaTime;
            float speed = baseSpeed + acceleration * t;
            float alpha = Mathf.PingPong(t * speed, 1f);
            SetAlpha(alpha);
            yield return null;
        }

        // 5초 버텼다면 제로 상태 돌입
        lifeRoutine = null;

        // 5초 버티면 완전 제로
        PlayerStaminaZero();

        // 이벤트 호출
        OnStaminaEmpty?.Invoke();

        // 3초 기다렸다가 회복
        yield return new WaitForSeconds(resetDelay);

        PlayerStaminaReset();

        // 여기에서 복구 이벤트 호출
        OnStaminaRecovered?.Invoke();
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

    public void PlayerStaminaZero()
    {
        // FilledGuageImage, BackgroundGuageImage 의 alpha 값 -> 0으로 변경
        SetAlpha(0f);
        // 계단과 플레이어 애니메이션 정지
        // 플레이어 거리 증가와 게이지바 오르는 코드 정지
    }

    public void PlayerStaminaReset()
    {
        // FilledGuageImage, BackgroundGuageImage 의 alpha 값 -> 255으로 변경
        SetAlpha(1f);
        // 계단과 플레이어 애니메이션 실행
        // 플레이어 거리 증가와 게이지바 오르는 코드 실행
    }
}
