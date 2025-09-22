using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DB/Item Databases", fileName = "ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemScriptableObject> items = new();
    private Dictionary<int, ItemScriptableObject> map;

    void OnEnable() => BuildMap();

    private void BuildMap()
    {
        map = new Dictionary<int, ItemScriptableObject>();
        foreach (var it in items)
        {
            if (it == null) continue;
            if (!map.ContainsKey(it.ItemId)) map.Add(it.ItemId, it);
            else Debug.LogWarning($"ม฿บน ItemId: {it.ItemId} ({it.name})");
        }
    }

    public ItemScriptableObject GetById(int id)
    {
        if (map == null || map.Count == 0) BuildMap();
        map.TryGetValue(id, out var so);
        return so;
    }
}
