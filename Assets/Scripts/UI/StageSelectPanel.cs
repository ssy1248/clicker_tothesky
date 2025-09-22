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
        DragPanel.Instance?.ActivateDragStagePanel(this);

        FindObjectOfType<StagePanelManager>(true)?.ResetToProgress();
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
        UIManager.Instance.PushPanel(UIPanelType.MARKET_PANEL);
    }

    public void OnClickSettingButton()
    {
        GameObject.Find("PopUpUIManager").GetComponent<PopUpUIManager>().SettingPopUpUIShow();
    }

    public void OnClickInventoryButton()
    {
        // if -> 현재 스테이지 카운트, 아이템 카운트 이걸 비교해서 만약 아이템이랑 스테이지가 같으면 바로 게임시작 / 아니면 리셋
        var inv = FindObjectOfType<InventoryPanel>(true); 
        if (inv != null)
        {
            inv.gameObject.SetActive(true);
            return;
        }
    }

    public void OnClickBuffButton()
    {
        var buff = FindObjectOfType<BuffPanel>(true);
        if (buff != null)
        {
            buff.gameObject.SetActive(true);
            return;
        }
    }

    public void ReturnMenu()
    {
        OnClose();
        GameObject chapterPanel = GameObject.Find("ChapterPanel(Clone)");

        if (chapterPanel != null) // stagePanel을 찾았다면 (null이 아니라면)
        {
            // 해당 오브젝트를 활성화합니다.
            chapterPanel.SetActive(true);
        }
        else
        {
            UIManager.Instance.PushPanel(UIPanelType.CHAPTER_PANEL);
        }
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