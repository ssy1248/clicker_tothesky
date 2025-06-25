using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

// 엔딩에 사용할 아이템 스테이트
public enum ItemState
{
    NONE = 0,
    POSIITIVE = 1,
    NEGETIVE = 2,
}

// 아이템 효과 타입 열거형
public enum ItemEffectType
{
    SpeedMultiplier,        // 전체 속도 증가
    CollectRangeModifier,   // 수집품 획득 범위 조절
    TimeLimitModifier,      // 제한시간 증가/감소
    ConditionalSpeedBoost,  // 조건부 속도 부여
    CooldownModifier,       // 쿨다운 시간 조절 등
}

[System.Serializable]
public class ItemEffect
{
    public ItemEffectType effectType;
    public float value;          // 효과 수치 (예: +2, -0.1 등)
    public string conditionText; // 조건 설명 (선택 사항)
}

[CreateAssetMenu(fileName = "Item", menuName = "ItemData")]
public class ItemScriptableObject : ScriptableObject
{
    // 아이템 아이디
    public int ItemId;
    // 아이템 스프라이트
    public Sprite ItemImage;
    // 아이템 이름
    public string ItemName;
    // 아이템 설명
    public string ItemDescription;
    // 아이템 분류
    public ItemState Itemstate;
    // 아이템 능력치
    public List<ItemEffect> Effects;
}
