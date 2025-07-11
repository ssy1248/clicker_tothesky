using UnityEngine;

public class PopUPUI : MonoBehaviour
{
    public static PopUPUI Instance { get; private set; }

    public GameObject popUpUI;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            // Instance가 비어있으면 자기 자신을 할당
            Instance = this;
        }
        else if (Instance != this)
        {
            // 새로 생긴 이 오브젝트는 파괴하여 단 하나만 존재하도록 보장
            Debug.LogWarning("GameModeManager의 인스턴스가 이미 존재하여 새로 생긴 것을 파괴합니다.");
            Destroy(gameObject);
            return; // 파괴될 오브젝트는 아래 로직을 실행할 필요 없음
        }
        popUpUI.SetActive(false);
    }

    public void PopUpUIShow()
    {
        popUpUI.SetActive(true);
    }

    public void PopUpUIClose()
    {
        popUpUI.SetActive(false);
    }
}
