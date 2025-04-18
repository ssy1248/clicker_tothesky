using UnityEngine;
using UnityEngine.UI;

public class LogoPanel : BasePanel
{
    public Button buttonStart;
    
    public Button buttonCredit;

    
    public override UIPanelType TypeOfPanel => UIPanelType.LOGO_PANEL;

    private void Awake()
    {
        buttonStart.onClick.RemoveListener(OnClickStart);
        buttonCredit.onClick.RemoveListener(OnClickCredit);
        buttonStart.onClick.AddListener(OnClickStart);
        buttonCredit.onClick.AddListener(OnClickCredit);
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

    public void OnClickStart()
    {
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.MAIN_PANEL);
    }
    
    public void OnClickCredit()
    {
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.CREDIT_PANEL);
    }
}
