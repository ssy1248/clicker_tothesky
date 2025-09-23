using UnityEngine;
using UnityEngine.EventSystems;

public class MainTouchArea : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        // 기존 Update에서 하던 처리 그대로 호출
        if (GameViewManagerExistsAndReady())
        {
            // 게이지
            var gm = Object.FindFirstObjectByType<GuageManager>();
            gm?.OnTouch();

            // 게임 로직
            GameModeManager.Instance?.OnPlayerTouch();
        }
    }

    bool GameViewManagerExistsAndReady()
    {
        var gvm = Object.FindFirstObjectByType<GameViewManager>();
        return gvm != null && !gvm.IsGameFinished;
    }
}
