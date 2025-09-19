using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectPanel : BasePanel
{
    public override UIPanelType TypeOfPanel => UIPanelType.STAGE_PANEL;

    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);
        this.gameObject.SetActive(true);
        DragPanel.Instance?.ActivateDragStagePanel(this);
    }

    public override void OnResume()
    {
        base.OnResume();
        DragPanel.Instance?.ActivateDragStagePanel(this); // (선택) 스테이지로 되돌아올 때도 안전하게
    }

    public override void OnPause()
    {
        DragPanel.Instance?.DeactivateDragStagePanel();
        base.OnPause();
    }

    public override void OnClose()
    {
        DragPanel.Instance?.DeactivateDragStagePanel();
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