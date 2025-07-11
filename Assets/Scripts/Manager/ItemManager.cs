using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// System.Serializable을 추가해야 Inspector 창에 표시됩니다.
[System.Serializable]
public class ItemSlot
{
    public Image uiImage;
    public SpriteRenderer worldSpriteRenderer;
}


public class ItemManager : MonoBehaviour
{
    // 아이템 슬롯을 관리할 새로운 배열
    public ItemSlot[] itemSlots;
    public List<ItemScriptableObject> ItemList;

    private void OnEnable()
    {
        if (ItemList == null || ItemList.Count == 0)
        {
            Debug.LogWarning("아이템 리스트가 비어있습니다!");
            return;
        }

        List<ItemScriptableObject> availableItems = new List<ItemScriptableObject>(ItemList);

        // images 배열 대신 itemSlots 배열을 순회합니다.
        foreach (ItemSlot slot in itemSlots)
        {
            if (availableItems.Count == 0)
            {
                Debug.LogWarning("UI 슬롯보다 아이템 개수가 적어 일부 슬롯이 비어있습니다.");
                break;
            }

            int randomIndex = Random.Range(0, availableItems.Count);
            ItemScriptableObject selectedItem = availableItems[randomIndex];

            // DisplayItem 함수에 슬롯의 두 컴포넌트를 모두 전달합니다.
            DisplayItem(slot.uiImage, slot.worldSpriteRenderer, selectedItem);

            // ItemInfo는 uiImage와 같은 게임 오브젝트에 있으므로 이 부분은 그대로 둡니다.
            ItemInfo info = slot.uiImage.GetComponent<ItemInfo>();
            if (info != null)
            {
                info.itemData = selectedItem;
            }
            else
            {
                Debug.LogWarning(slot.uiImage.name + " 게임 오브젝트에 ItemInfo 컴포넌트가 없습니다.");
            }

            availableItems.RemoveAt(randomIndex);
        }
    }

    public void DisplayItem(Image uiImage, SpriteRenderer spriteRenderer, ItemScriptableObject itemData)
    {
        if (itemData == null || itemData.ItemImage == null)
        {
            // UI와 스프라이트 렌더러 모두 비활성화
            if (uiImage != null) uiImage.gameObject.SetActive(false);
            if (spriteRenderer != null) spriteRenderer.gameObject.SetActive(false);
            return;
        }

        // UI 설정
        if (uiImage != null)
        {
            uiImage.gameObject.SetActive(true);
            uiImage.sprite = itemData.ItemImage;
            uiImage.rectTransform.sizeDelta = itemData.displayScale;

            uiImage.rectTransform.localScale = Vector3.one * 0.45f;
        }

        // SpriteRenderer 설정
        if (spriteRenderer != null)
        {
            spriteRenderer.gameObject.SetActive(true);
            spriteRenderer.sprite = itemData.ItemImage;
        }
    }
}
