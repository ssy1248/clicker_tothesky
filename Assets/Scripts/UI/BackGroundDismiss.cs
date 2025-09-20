using UnityEngine;
using UnityEngine.EventSystems;

public class BackGroundDismiss : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject alertRoot;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (alertRoot != null)
        {
            alertRoot.SetActive(false);
        }
    }
}
