using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectPanel : BasePanel
{
    public override UIPanelType TypeOfPanel => UIPanelType.STAGE_PANEL;

    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);
        this.gameObject.SetActive(true);

        if (DragPanel.Instance != null)
        {
            DragPanel.Instance.ActivateDragStagePanel(this);
        }
    }

    public override void OnClose()
    {
        if (DragPanel.Instance != null)
        {
            DragPanel.Instance.DeactivateDragStagePanel();
        }

        base.OnClose();
        this.gameObject.SetActive(false);
    }

    public void OnClickMemoryPanel()
    {
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.MEMORY_PANEL);
    }

    public void OnClickMarketButton()
    {
        OnClose();
    }

    public void OnClickSettingButton()
    {
        GameObject.Find("PopUpUIManager").GetComponent<PopUpUIManager>().SettingPopUpUIShow();
    }

    public void OnClickInventoryButton()
    {

    }
}

/*
    스테이지   클리어 거리
        1       110
        2       162
        3       214
        4       266
        5       318
        6       370
 */