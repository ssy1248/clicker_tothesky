using UnityEngine;

public class ItemInfoPanel : MonoBehaviour
{
    public SpriteRenderer descriptionImageRenderer;

    // 현재 정보가 표시되고 있는 아이템(의 ItemInfo 스크립트)을 저장할 변수
    private ItemInfo sourceItem;

    // 외부(클릭된 아이템)에서 이 패널을 활성화시킬 때 호출할 함수
    public void DisplayPanelFor(ItemInfo item)
    {
        // 1. 어떤 아이템이 이 패널을 열었는지 저장합니다.
        sourceItem = item;

        // 2. 표시할 아이템의 ScriptableObject에서 설명 이미지를 가져옵니다.
        Sprite descSprite = item.itemData.ItemDescriptionImage;

        // 3. 참조된 Sprite Renderer의 스프라이트를 교체합니다. (null 체크는 안전을 위해)
        if (descriptionImageRenderer != null)
        {
            descriptionImageRenderer.sprite = descSprite;
        }
        else
        {
            Debug.LogError("descriptionImageRenderer가 할당되지 않았습니다!");
        }

        // 4. 패널을 활성화합니다.
        gameObject.SetActive(true);
    }

    // Yes/No 버튼이 눌렸을 때, 어떤 아이템이었는지 알려주는 함수
    public ItemInfo GetSourceItem()
    {
        return sourceItem;
    }
}
