using UnityEngine;

public class InGameItemManager : MonoBehaviour
{
    public static InGameItemManager Instance { get; private set; }

    [Header("적용 대상 컴포넌트")]
    public GameModeManager gameModeManager;
    public GameViewManager gameViewManager;
    public GuageImageAlpha guageImageAlpha;

    private void Awake()
    {
        // Singleton 패턴 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 컴포넌트 자동 찾기
        if (gameModeManager == null) gameModeManager = FindObjectOfType<GameModeManager>();
        if (gameViewManager == null) gameViewManager = FindObjectOfType<GameViewManager>();
        if (guageImageAlpha == null) guageImageAlpha = FindObjectOfType<GuageImageAlpha>();
    }

    void Start()
    {
        // SelectedItemList 인스턴스가 존재하는지 확인
        if (SelectedItemList.Instance != null)
        {
            ApplyAllItemEffects();
        }
        else
        {
            Debug.LogWarning("SelectedItemList 인스턴스를 찾을 수 없습니다!");
        }
    }

    /// <summary>
    /// 선택된 모든 아이템의 효과를 게임에 적용합니다.
    /// </summary>
    public void ApplyAllItemEffects()
    {
        // SelectedItemList에서 선택된 아이템 리스트를 가져옵니다.
        var items = SelectedItemList.Instance.selectedItems;

        Debug.Log($"적용할 아이템 개수: {items.Count}");

        // 각 아이템을 순회합니다.
        foreach (var item in items)
        {
            // 아이템의 각 효과를 순회합니다.
            foreach (var effect in item.Effects)
            {
                ApplyEffect(effect);
            }
        }
    }

    /// <summary>
    /// 개별 아이템 효과를 타입에 따라 적용합니다.
    /// </summary>
    /// <param name="effect">적용할 아이템 효과</param>
    private void ApplyEffect(ItemEffect effect)
    {
        // 효과 타입에 따라 적절한 로직을 실행합니다.
        switch (effect.effectType)
        {
            case ItemEffectType.SpeedMultiplier:
                if (gameModeManager != null)
                {
                    gameModeManager.SpeedItemPlus += effect.value;
                    Debug.Log($"속도 증가 아이템 효과 적용! 추가 속도: {effect.value}");
                }
                break;

            case ItemEffectType.TimeLimitModifier:
                if (gameViewManager != null)
                {
                    gameViewManager.GameTimePlus += effect.value;
                    Debug.Log($"시간 아이템 효과 적용! 추가 시간: {effect.value}초");
                }
                break;

            case ItemEffectType.ConditionalSpeedBoost:
                // 조건부 효과는 별도의 로직이 필요합니다. (예: 특정 조건 만족 시 발동)
                // 이 부분은 게임 로직에 맞춰 구현해야 합니다.
                Debug.Log($"{effect.conditionText} 조건의 속도 부스트 효과는 게임 로직 내에서 별도 구현이 필요합니다.");
                break;

            case ItemEffectType.CooldownModifier:
                if (guageImageAlpha != null)
                {
                    // 만약 쿨감 아이템이라면 음수(-)로 설정
                    guageImageAlpha.RecoverTime += effect.value;
                    Debug.Log($"스태미나 회복 시간 {effect.value}초 변경!");
                }
                break;
        }
    }
}
