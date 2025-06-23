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
        // 2) 기존 로직—로고 패널 닫고, 메인 패널 띄우고
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.STAGE_SELECT_PANEL);
    }
    
    public void OnClickCredit()
    {
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.CREDIT_PANEL);
    }
}
