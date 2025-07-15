using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInfo : MonoBehaviour, IPointerClickHandler
{
    // 이 아이템 UI가 어떤 ScriptableObject 데이터를 가지고 있는지 저장하는 변수
    public ItemScriptableObject itemData;
    // 아이템 설명할 오브젝트
    public ItemInfoPanel itemInfoPanel;

    // 클릭 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        // 1. ItemInfoPanel을 찾거나, 인스펙터에서 할당된 것을 사용
        if (itemInfoPanel == null)
        {
            // 씬에서 비활성화된 것도 포함해서 찾기
            itemInfoPanel = FindObjectOfType<ItemInfoPanel>(true);
        }

        // 2. ItemInfoPanel에게 "나(this)에 대한 정보창을 띄워줘"라고 요청
        if (itemInfoPanel != null)
        {
            itemInfoPanel.DisplayPanelFor(this);
        }
    }
}
