using UnityEngine;

public class GameRoot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManager.Instance.PushPanel(UIPanelType.LOGO_PANEL);
    }

    // Update is called once per frame

}
