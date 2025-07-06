using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInfo : MonoBehaviour, IPointerClickHandler
{
    // 이 아이템 UI가 어떤 ScriptableObject 데이터를 가지고 있는지 저장하는 변수
    public ItemScriptableObject itemData;

    // 이 오브젝트가 클릭되었을 때 자동으로 호출되는 함수입니다.
    public void OnPointerClick(PointerEventData eventData)
    {
        // 여기에 버튼을 클릭했을 때 실행하고 싶은 코드를 작성하세요.
        Debug.Log(gameObject.name + "가 클릭되었습니다!");

        // 예시: 오브젝트 비활성화
        // gameObject.SetActive(false);
    }
}
