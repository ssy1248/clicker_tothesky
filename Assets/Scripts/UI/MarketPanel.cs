using UnityEngine;

public class MarketPanel : BasePanel
{
    public override UIPanelType TypeOfPanel => UIPanelType.MARKET_PANEL;

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

    public void ReturnStage()
    {
        OnClose();
        GameObject stagePanel = GameObject.Find("StagePanel(Clone)");

        if (stagePanel != null) // stagePanel를 찾았다면 (null이 아니라면)
        {
            // 해당 오브젝트를 활성화합니다.
            stagePanel.SetActive(true);
        }
        else
        {
            UIManager.Instance.PushPanel(UIPanelType.STAGE_PANEL);
        }
    }

    public void OnClickReturn()
    {
        
    }
}
