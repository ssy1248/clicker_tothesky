using UnityEngine;
using UnityEngine.UI;

public class CreditPanel : BasePanel
{
    public Button buttonBack;

    
    public override UIPanelType TypeOfPanel => UIPanelType.CREDIT_PANEL;

    private void Awake()
    {
        buttonBack.onClick.RemoveListener(OnClickBack);
        buttonBack.onClick.AddListener(OnClickBack);
    }
    
    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);
        this.gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        this.gameObject.SetActive(false);
    }

    public void OnClickBack()
    {
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.LOGO_PANEL);
    }
}