using UnityEngine;

public class StageSelectPanel : BasePanel
{
    public override UIPanelType TypeOfPanel => UIPanelType.STAGE_SELECT_PANEL;

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
}
