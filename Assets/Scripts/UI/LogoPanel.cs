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
        // 1) 게임 시작 플래그 설정
        GlobalVariable.Instance.StartedGame();

        // 2) 기존 로직—로고 패널 닫고, 메인 패널 띄우고
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.MAIN_PANEL);

        // 3) In-Game UI 오브젝트 활성화
        for (int i = 0; i < GlobalManager.Instance.UIObjects.Length; i++)
            GlobalManager.Instance.UIObjects[i].SetActive(true);
    }
    
    public void OnClickCredit()
    {
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.CREDIT_PANEL);
    }
}
