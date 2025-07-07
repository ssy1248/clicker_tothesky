using UnityEngine;

public class ItemInfoPanel : MonoBehaviour
{
    // 현재 정보가 표시되고 있는 아이템(의 ItemInfo 스크립트)을 저장할 변수
    private ItemInfo sourceItem;

    // 외부(클릭된 아이템)에서 이 패널을 활성화시킬 때 호출할 함수
    public void DisplayPanelFor(ItemInfo item)
    {
        // 어떤 아이템이 이 패널을 열었는지 저장
        sourceItem = item;
        // 패널 활성화
        gameObject.SetActive(true);
    }

    // Yes/No 버튼이 눌렸을 때, 어떤 아이템이었는지 알려주는 함수
    public ItemInfo GetSourceItem()
    {
        return sourceItem;
    }
}
