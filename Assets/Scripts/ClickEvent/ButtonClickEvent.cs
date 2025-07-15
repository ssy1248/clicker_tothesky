using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonClickEvent : MonoBehaviour, IPointerClickHandler
{
    // 클릭 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        // 1. 부모 계층에서 ItemInfoPanel 스크립트를 찾음
        ItemInfoPanel panel = GetComponentInParent<ItemInfoPanel>();
        if (panel == null) return; // 패널을 못찾으면 중단

        if (gameObject.CompareTag("NoBtn"))
        {
            // 그냥 패널만 닫기
            panel.gameObject.SetActive(false);
        }
        else if (gameObject.CompareTag("YesBtn"))
        {
            // 2. 패널에게 sourceItem을 받아옴
            ItemInfo sourceItem = panel.GetSourceItem();
            if (sourceItem != null)
            {
                // 3. 받아온 아이템의 'itemData'를 SelectedItemList에 추가
                SelectedItemList.Instance.AddItem(sourceItem.itemData);

                // 4. 패널을 닫음
                panel.gameObject.SetActive(false);

                // 5. 게임씬 이동
                SceneManager.LoadScene("MainScene");
            }
        }
    }
}
