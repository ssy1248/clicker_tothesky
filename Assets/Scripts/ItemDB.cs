using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// 조건 타입 열거형
[System.Serializable]
public enum ConditionType
{
    None = 0,                // 조건 없음
    DistancePercentage = 1,  // 거리 퍼센트 조건
    TimeRemaining = 2,       // 남은 시간 조건
    StateActive = 3,         // 특정 상태 활성화 조건
    StateDuration = 4,       // 상태 지속 시간 조건
    StateTransition = 5      // 상태 전환 조건
}

// 상태 타입 열거형
[System.Serializable]
public enum StateType
{
    Normal = 0,    // 노말
    Fever = 1,     // 피버
    Excited = 2,   // 흥분
    Overheated = 3 // 과열
}

// 비교 연산자 열거형
[System.Serializable]
public enum ComparisonOperator
{
    LessThan = 0,           // <
    LessThanOrEqual = 1,    // <=
    GreaterThan = 2,        // >
    GreaterThanOrEqual = 3, // >=
    Equal = 4,              // ==
    NotEqual = 5            // !=
}

// 조건부 효과 구조체
[System.Serializable]
public class ConditionalEffect
{
    [Header("조건 설정")]
    public ConditionType conditionType = ConditionType.None;
    public ComparisonOperator comparisonOperator = ComparisonOperator.LessThan;
    public float conditionValue = 0f;          // 조건 기준값
    public StateType targetState = StateType.Normal;  // 대상 상태
    public float timeValue = 0f;               // 시간 관련 값
    
    [Header("조건 만족 시 효과")]
    public bool destroyItemAfterUse = false;   // 사용 후 아이템 삭제
    
    [Header("조건부 효과 값들")]
    public SpeedStats conditionalSpeedEffect = new SpeedStats();
    public TimeStats conditionalTimeEffect = new TimeStats();
    public ClickStats conditionalClickEffect = new ClickStats();
    public ProbabilityStats conditionalProbabilityEffect = new ProbabilityStats();
    public SweetSpotStats conditionalSweetSpotEffect = new SweetSpotStats();
    
    [Header("조건 설명")]
    [TextArea(1, 3)]
    public string conditionDescription = "조건 설명을 입력하세요";
    
    // 조건 체크 메서드 (게임에서 사용)
    public bool CheckCondition(float currentValue, float timeValue = 0f, StateType currentState = StateType.Normal)
    {
        switch (conditionType)
        {
            case ConditionType.None:
                return true;
                
            case ConditionType.DistancePercentage:
                return CompareValues(currentValue, conditionValue);
                
            case ConditionType.TimeRemaining:
                return CompareValues(timeValue, this.timeValue);
                
            case ConditionType.StateActive:
                return currentState == targetState;
                
            case ConditionType.StateDuration:
                return currentState == targetState && CompareValues(timeValue, this.timeValue);
                
            case ConditionType.StateTransition:
                return currentState == targetState;
                
            default:
                return false;
        }
    }
    
    private bool CompareValues(float value1, float value2)
    {
        switch (comparisonOperator)
        {
            case ComparisonOperator.LessThan:
                return value1 < value2;
            case ComparisonOperator.LessThanOrEqual:
                return value1 <= value2;
            case ComparisonOperator.GreaterThan:
                return value1 > value2;
            case ComparisonOperator.GreaterThanOrEqual:
                return value1 >= value2;
            case ComparisonOperator.Equal:
                return Mathf.Approximately(value1, value2);
            case ComparisonOperator.NotEqual:
                return !Mathf.Approximately(value1, value2);
            default:
                return false;
        }
    }
}

[System.Serializable]
public class HPStats
{
    [Header("HP 관련 (양수: 증가, 음수: 감소)")]
    public int stageHP = 0;        // 거리(스테이지 HP)
    public int bossHP = 0;         // 보스 HP(문)
    
    public bool HasEffect()
    {
        return stageHP != 0 || bossHP != 0;
    }
}

[System.Serializable]
public class TimeStats
{
    [Header("시간 관련 (양수: 증가, 음수: 감소)")]
    public float stageTime = 0f;         // 스테이지 시간
    public float normalStateTime = 0f;   // 노말 상태 시간
    public float feverStateTime = 0f;    // 피버 상태 시간
    public float excitedStateTime = 0f;  // 흥분 상태 시간 (과열지침 상태 시간)
    public float overheatedPenaltyTime = 0f; // 과열 상태 패널티 시간
    
    public bool HasEffect()
    {
        return stageTime != 0f || normalStateTime != 0f || feverStateTime != 0f || 
               excitedStateTime != 0f || overheatedPenaltyTime != 0f;
    }
}

[System.Serializable]
public class ClickStats
{
    [Header("클릭(스택) 관련 (양수: 증가, 음수: 감소)")]
    public int basicClickValue = 0;           // 기본 클릭 수치
    public int autoClickValue = 0;            // 오토 클릭 수치
    public int normalAutoClickValue = 0;      // 노말 상태 오토클릭 수치 값 1
    public int feverAutoClickValue = 0;       // 피버 상태 오토클릭 수치 값 2
    public int excitedAutoClickValue = 0;     // 흥분 상태 오토클릭 수치 값 3
    
    public bool HasEffect()
    {
        return basicClickValue != 0 || autoClickValue != 0 || normalAutoClickValue != 0 ||
               feverAutoClickValue != 0 || excitedAutoClickValue != 0;
    }
}

[System.Serializable]
public class SpeedStats
{
    [Header("속도 관련 (양수: 증가, 음수: 감소)")]
    public float globalSpeedMultiplier = 0f;     // 전체 배수 증가
    public float normalSpeedBonus = 0f;          // 노말 상태 속도 보너스
    public float feverSpeedBonus = 0f;           // 피버 상태 속도 보너스
    public float excitedSpeedBonus = 0f;         // 흥분 상태 속도 보너스
    
    public bool HasEffect()
    {
        return globalSpeedMultiplier != 0f || normalSpeedBonus != 0f || 
               feverSpeedBonus != 0f || excitedSpeedBonus != 0f;
    }
}

[System.Serializable]
public class SweetSpotStats
{
    [Header("스윗 스팟 관련 (양수: 증가, 음수: 감소)")]
    public float sweetSpotRangeChange = 0f;      // 스윗 스팟 범위 변화 (%)
    public float sweetSpotAccuracy = 0f;         // 스윗 스팟 정확도 변화
    
    public bool HasEffect()
    {
        return sweetSpotRangeChange != 0f || sweetSpotAccuracy != 0f;
    }
}

[System.Serializable]
public class ReviveStats
{
    [Header("부활 관련 (양수: 증가, 음수: 감소)")]
    public int stageRestart = 0;        // 부활(스테이지 재시작)
    public int currentPointRestart = 0; // 부활(현시점 재시작)
    
    public bool HasEffect()
    {
        return stageRestart != 0 || currentPointRestart != 0;
    }
}

[System.Serializable]
public class SkipStats
{
    [Header("건너뛰기 관련 (양수: 증가, 음수: 감소)")]
    public int stageSkip = 0;          // 스테이지 건너뛰기
    public int doorSkip = 0;           // 문 건너뛰기
    
    public bool HasEffect()
    {
        return stageSkip != 0 || doorSkip != 0;
    }
}

[System.Serializable]
public class ProbabilityStats
{
    [Header("확률 관련 (양수: 증가, 음수: 감소) - 단위: %")]
    public float itemProbability = 0f;      // 아이템 확률
    public float criticalProbability = 0f;  // 크리티컬 확률
    public float resetProbability = 0f;     // 리셋 확률(노말상태로 돌아갈 확률)
    
    public bool HasEffect()
    {
        return itemProbability != 0f || criticalProbability != 0f || resetProbability != 0f;
    }
}

[System.Serializable]
public class CountStats
{
    [Header("개수 관련 (양수: 증가, 음수: 감소)")]
    public int itemSpawn = 0;          // 아이템 등장 개수
    public int refreshCount = 0;       // 새로고침 개수
    
    public bool HasEffect()
    {
        return itemSpawn != 0 || refreshCount != 0;
    }
}

[System.Serializable]
public class Item
{
    [Header("기본 정보")]
    public int ItemOrder;
    public Sprite icon;
    public string name = "";
    [Range(1, 5)]
    public int rareness = 1;           // 희귀도
    [TextArea(2, 4)]
    public string description = "";    // 아이템 설명
    
    [Header("기본 효과 (즉시 적용)")]
    public HPStats hpStats = new HPStats();
    public TimeStats timeStats = new TimeStats();
    public ClickStats clickStats = new ClickStats();
    public SpeedStats speedStats = new SpeedStats();
    public SweetSpotStats sweetSpotStats = new SweetSpotStats();
    public ReviveStats reviveStats = new ReviveStats();
    public SkipStats skipStats = new SkipStats();
    public ProbabilityStats probabilityStats = new ProbabilityStats();
    public CountStats countStats = new CountStats();
    
    [Header("조건부 효과")]
    public List<ConditionalEffect> conditionalEffects = new List<ConditionalEffect>();
    
    [Header("아이템 특성")]
    public bool isConsumable = true;   // 소모품인지 여부
    public bool isStackable = false;   // 중첩 가능한지 여부
    public int maxStackCount = 1;      // 최대 중첩 개수
    
    // 아이템이 어떤 효과를 가지고 있는지 체크
    public bool HasAnyEffect()
    {
        return hpStats.HasEffect() || timeStats.HasEffect() || clickStats.HasEffect() ||
               speedStats.HasEffect() || sweetSpotStats.HasEffect() || reviveStats.HasEffect() ||
               skipStats.HasEffect() || probabilityStats.HasEffect() || countStats.HasEffect() ||
               (conditionalEffects != null && conditionalEffects.Count > 0);
    }
    
    // 유효성 검사
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(name) && ItemOrder > 0 && rareness >= 1 && rareness <= 5;
    }
}

[CreateAssetMenu(fileName = "ItemDB", menuName = "Kit/Data/ItemDB", order=0)]
public class ItemDB : ScriptableObject
{
    [Header("아이템 데이터베이스")]
    public List<Item> items = new List<Item>();
    
    [Header("데이터베이스 정보")]
    public string databaseName = "Main Item Database";
    [TextArea(2, 4)]
    public string description = "게임의 모든 아이템을 관리하는 데이터베이스";
    
    // 아이템 검색 메서드들 (null 안전성 보장)
    public Item GetItemByOrder(int itemOrder)
    {
        if (items == null || items.Count == 0) return null;
        return items.FirstOrDefault(item => item != null && item.ItemOrder == itemOrder);
    }
    
    public Item GetItemByName(string itemName)
    {
        if (items == null || items.Count == 0 || string.IsNullOrEmpty(itemName)) return null;
        return items.FirstOrDefault(item => item != null && !string.IsNullOrEmpty(item.name) && 
                                          item.name.Equals(itemName, System.StringComparison.OrdinalIgnoreCase));
    }
    
    public List<Item> GetItemsByRareness(int rareness)
    {
        if (items == null || items.Count == 0) return new List<Item>();
        return items.Where(item => item != null && item.rareness == rareness).ToList();
    }
    
    // 조건부 효과가 있는 아이템들
    public List<Item> GetItemsWithConditionalEffects()
    {
        if (items == null || items.Count == 0) return new List<Item>();
        return items.Where(item => item != null && item.conditionalEffects != null && item.conditionalEffects.Count > 0).ToList();
    }
    
    // 소모품 아이템들
    public List<Item> GetConsumableItems()
    {
        if (items == null || items.Count == 0) return new List<Item>();
        return items.Where(item => item != null && item.isConsumable).ToList();
    }
    
    // 중첩 가능한 아이템들
    public List<Item> GetStackableItems()
    {
        if (items == null || items.Count == 0) return new List<Item>();
        return items.Where(item => item != null && item.isStackable).ToList();
    }
    
    // 효과가 있는 아이템들
    public List<Item> GetItemsWithEffects()
    {
        if (items == null || items.Count == 0) return new List<Item>();
        return items.Where(item => item != null && item.HasAnyEffect()).ToList();
    }
    
    // HP 효과 아이템들
    public List<Item> GetItemsWithStageHPEffect()
    {
        if (items == null || items.Count == 0) return new List<Item>();
        return items.Where(item => item != null && item.hpStats != null && item.hpStats.stageHP != 0).ToList();
    }
    
    public List<Item> GetItemsWithBossHPEffect()
    {
        if (items == null || items.Count == 0) return new List<Item>();
        return items.Where(item => item != null && item.hpStats != null && item.hpStats.bossHP != 0).ToList();
    }
    
    // 시간 효과 아이템들
    public List<Item> GetItemsWithTimeEffect()
    {
        if (items == null || items.Count == 0) return new List<Item>();
        return items.Where(item => item != null && item.timeStats != null && item.timeStats.HasEffect()).ToList();
    }
    
    // 클릭 효과 아이템들
    public List<Item> GetItemsWithClickEffect()
    {
        if (items == null || items.Count == 0) return new List<Item>();
        return items.Where(item => item != null && item.clickStats != null && item.clickStats.HasEffect()).ToList();
    }
    
    // 속도 효과 아이템들
    public List<Item> GetItemsWithSpeedEffect()
    {
        if (items == null || items.Count == 0) return new List<Item>();
        return items.Where(item => item != null && item.speedStats != null && item.speedStats.HasEffect()).ToList();
    }
    
    // 스윗 스팟 효과 아이템들
    public List<Item> GetItemsWithSweetSpotEffect()
    {
        if (items == null || items.Count == 0) return new List<Item>();
        return items.Where(item => item != null && item.sweetSpotStats != null && item.sweetSpotStats.HasEffect()).ToList();
    }
    
    public List<Item> GetAllItemsSorted()
    {
        if (items == null || items.Count == 0) return new List<Item>();
        return items.Where(item => item != null).OrderBy(item => item.ItemOrder).ToList();
    }
    
    public void AddItem(Item newItem)
    {
        if (newItem != null && !items.Contains(newItem))
        {
            if (items == null) items = new List<Item>();
            items.Add(newItem);
        }
    }
    
    public bool RemoveItem(int itemOrder)
    {
        if (items == null || items.Count == 0) return false;
        
        Item itemToRemove = GetItemByOrder(itemOrder);
        if (itemToRemove != null)
        {
            items.Remove(itemToRemove);
            return true;
        }
        return false;
    }
    
    public int GetItemCount()
    {
        return items?.Count ?? 0;
    }
    
    public bool IsItemOrderDuplicate(int itemOrder)
    {
        if (items == null || items.Count == 0) return false;
        return items.Count(item => item != null && item.ItemOrder == itemOrder) > 1;
    }
    
    // 유효성 검사
    public List<string> ValidateDatabase()
    {
        List<string> errors = new List<string>();
        
        if (items == null)
        {
            errors.Add("아이템 리스트가 null입니다.");
            return errors;
        }
        
        for (int i = 0; i < items.Count; i++)
        {
            Item item = items[i];
            if (item == null)
            {
                errors.Add($"인덱스 {i}: 아이템이 null입니다.");
                continue;
            }
            
            if (!item.IsValid())
            {
                errors.Add($"아이템 '{item.name}' (인덱스 {i}): 유효하지 않은 데이터입니다.");
            }
            
            if (IsItemOrderDuplicate(item.ItemOrder))
            {
                errors.Add($"아이템 '{item.name}': 중복된 순번 {item.ItemOrder}입니다.");
            }
        }
        
        return errors;
    }
    
    #if UNITY_EDITOR
    [ContextMenu("데이터베이스 유효성 검사")]
    public void ValidateDatabaseDebug()
    {
        List<string> errors = ValidateDatabase();
        
        if (errors.Count == 0)
        {
            Debug.Log("✅ 데이터베이스 유효성 검사 통과!");
        }
        else
        {
            Debug.LogError($"❌ 데이터베이스에 {errors.Count}개의 오류가 발견되었습니다:");
            foreach (string error in errors)
            {
                Debug.LogError($"- {error}");
            }
        }
    }
    
    [ContextMenu("아이템 순번으로 정렬")]
    public void SortItemsByOrder()
    {
        if (items != null && items.Count > 0)
        {
            // null 아이템들을 제거하고 정렬
            items = items.Where(item => item != null).OrderBy(item => item.ItemOrder).ToList();
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log("아이템들이 순번으로 정렬되었습니다.");
        }
    }
    
    [ContextMenu("null 아이템 제거")]
    public void RemoveNullItems()
    {
        if (items != null)
        {
            int originalCount = items.Count;
            items.RemoveAll(item => item == null);
            int removedCount = originalCount - items.Count;
            
            if (removedCount > 0)
            {
                UnityEditor.EditorUtility.SetDirty(this);
                Debug.Log($"{removedCount}개의 null 아이템이 제거되었습니다.");
            }
            else
            {
                Debug.Log("제거할 null 아이템이 없습니다.");
            }
        }
    }
    
    [ContextMenu("아이템 통계 보기")]
    public void ShowItemStatistics()
    {
        if (items == null || items.Count == 0)
        {
            Debug.Log("아이템이 없습니다.");
            return;
        }
        
        Debug.Log($"=== 아이템 데이터베이스 통계 ===");
        Debug.Log($"총 아이템 수: {GetItemCount()}");
        Debug.Log($"조건부 효과 아이템: {GetItemsWithConditionalEffects().Count}개");
        Debug.Log($"소모품: {GetConsumableItems().Count}개");
        Debug.Log($"중첩 가능: {GetStackableItems().Count}개");
        Debug.Log($"속도 효과 아이템: {GetItemsWithSpeedEffect().Count}개");
        Debug.Log($"스윗 스팟 효과 아이템: {GetItemsWithSweetSpotEffect().Count}개");
        Debug.Log($"효과가 있는 아이템: {GetItemsWithEffects().Count}개");
        
        // 희귀도별 통계
        for (int i = 1; i <= 5; i++)
        {
            int count = GetItemsByRareness(i).Count;
            if (count > 0)
            {
                string rareName = i == 1 ? "일반" : i == 2 ? "고급" : i == 3 ? "희귀" : i == 4 ? "영웅" : "전설";
                Debug.Log($"{rareName} 등급: {count}개");
            }
        }
    }
    #endif
}