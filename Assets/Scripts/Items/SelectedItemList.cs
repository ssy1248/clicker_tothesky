using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class SelectedItemList : MonoBehaviour
{
    // Singleton 패턴으로 어디서든 쉽게 접근 가능하게 함
    public static SelectedItemList Instance { get; private set; }

    [SerializeField] private ItemDatabase itemDb;

    // 선택된 아이템들의 데이터를 담을 리스트
    public List<ItemScriptableObject> selectedItems = new List<ItemScriptableObject>();

    public IEnumerable<int> GetSelectedItemIds() => selectedItems.ConvertAll(x => x.ItemId);

    private void Awake()
    {
        // Singleton 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            // 이 오브젝트는 씬이 바뀌어도 파괴되지 않음
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 이미 인스턴스가 존재하면 새로 생긴 것은 파괴
            Destroy(gameObject);
        }
    }

    public void LoadFromIds(IEnumerable<int> ids)
    {
        selectedItems.Clear();
        if (itemDb == null) { 
            Debug.LogError("ItemDatabase 미할당"); 
            return; 
        }

        foreach (var id in ids)
        {
            var so = itemDb.GetById(id);
            if (so != null) 
                selectedItems.Add(so);
            else 
                Debug.LogWarning($"ItemId {id} 를 DB에서 찾지 못함");
        }
    }

    // 외부에서 선택된 아이템을 추가하는 함수
    public void AddItem(ItemScriptableObject itemToAdd)
    {
        if (itemToAdd != null)
        {
            selectedItems.Add(itemToAdd);
            Debug.Log($"{itemToAdd.ItemName} 아이템이 목록에 추가되었습니다.");
        }
    }

    /// <summary>
    /// 선택된 아이템 목록을 모두 비웁니다.
    /// </summary>
    public void ClearItems()
    {
        selectedItems.Clear();
        Debug.Log("선택된 아이템 목록이 초기화되었습니다.");
    }
}
