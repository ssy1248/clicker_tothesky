using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance { get; private set; }

    [Header("애니메이터 참조")]
    public Animator PlayerAnimator;
    public Animator StairAnimator;
    public Animator BackgroundAnimator;

    private void Awake()
    {
        Instance = this;
    }

    public void AnimationAllStop()
    {
        PlayerAnimator.speed = 0f;
        StairAnimator.speed = 0f;
        if (BackgroundAnimator != null)
            BackgroundAnimator.speed = 0f;
    }

    public void AnimationAllPlay()
    {
        PlayerAnimator.speed = 1f;
        StairAnimator.speed = 1f;
        if (BackgroundAnimator != null)
            BackgroundAnimator.speed = 1f;
    }
}
