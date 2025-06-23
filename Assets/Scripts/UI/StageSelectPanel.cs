using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectPanel : BasePanel
{
    public override UIPanelType TypeOfPanel => UIPanelType.STAGE_SELECT_PANEL;

    public void SetStageValue(int stageDistance)
    {
        // 각 스테이지 거리 세팅
        GlobalVariable.Instance.CheckPointDistance = stageDistance;

        SceneManager.LoadScene("ShopScene");
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
}
