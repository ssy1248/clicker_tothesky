using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BackGroundDismiss : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject Root;
    [SerializeField] private UnityEvent onBackgroundClick; // 선택(콜백)

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Root != null)
        {
            Root.SetActive(false);
        }

        onBackgroundClick?.Invoke();
    }
}
