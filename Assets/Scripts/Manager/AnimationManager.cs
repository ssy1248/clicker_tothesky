using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance { get; private set; }

    [Header("애니메이터 참조")]
    public Animator PlayerAnimator;
    public Animator StairAnimator;
    public Animator BackgroundAnimator;
    public Animator PlayerShadowAnimator;

    private void Awake()
    {
        Instance = this;
    }

    public void AnimationAllStop()
    {
        // 시작하자마자 애니메이션 이상하게 시작됨

        // 플레이어는 'IsStamina' 파라미터를 true로 설정하여 지치는 애니메이션으로 전환
        PlayerAnimator.SetBool("IsStamina", true);

        // 나머지 환경 관련 애니메이션은 재생 속도를 0으로 만들어 정지
        StairAnimator.speed = 0f;
        PlayerShadowAnimator.speed = 0f;
        if (BackgroundAnimator != null)
            BackgroundAnimator.speed = 0f;
    }

    public void AnimationAllPlay()
    {
        // 플레이어는 'IsStamina' 파라미터를 false로 설정하여 다시 달리는 애니메이션으로 전환
        PlayerAnimator.SetBool("IsStamina", false);

        // 나머지 환경 관련 애니메이션은 재생 속도를 1로 만들어 다시 재생
        StairAnimator.speed = 1f;
        PlayerShadowAnimator.speed = 1f;
        if (BackgroundAnimator != null)
            BackgroundAnimator.speed = 1f;
    }
}
