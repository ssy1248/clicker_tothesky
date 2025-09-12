using UnityEngine;
using UnityEngine.EventSystems;

public class SubTouchActivator : MonoBehaviour, IPointerClickHandler
{
    private SubTouchType type;
    private int score;
    private SubTouchManager manager;

    /// <summary>
    /// 생성된 후, 어떤 미니게임을 시작할지 정보를 설정하는 함수
    /// </summary>
    public void Initialize(SubTouchType miniGameType, int successScore, SubTouchManager manager)
    {
        this.type = miniGameType;
        this.score = successScore;
        this.manager = manager;
    }

    /// <summary>
    /// 이 오브젝트가 클릭되었을 때 호출됩니다.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 1. SubTouchManager에게 실제 미니게임 생성을 요청합니다.
        if (manager != null)
        {
            manager.SpawnActualMiniGame(type, score);
        }

        // 2. 자신(활성화 오브젝트)은 역할을 다했으므로 파괴합니다.
        Destroy(gameObject);
    }
}
