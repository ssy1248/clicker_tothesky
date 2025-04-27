using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorePanel : BasePanel
{
    public Button buttonSkip;
    public TMP_Text itemlabelName;
    public TMP_Text itemlabelDesc;
    
    private string nameMessage;
    private string descMessage;
    
    public override UIPanelType TypeOfPanel => UIPanelType.STORE_PANEL;

    private void Awake()
    {
        buttonSkip.onClick.RemoveListener(OnClickClose);
        buttonSkip.onClick.AddListener(OnClickClose);
    }
    
    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);
        SetContainers();
        this.gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        this.gameObject.SetActive(false);
    }

    public void OnClickClose()
    {
        OnClose();
        UIManager.Instance.PushPanel(UIPanelType.MAIN_PANEL);

    }

    public void SetContainers()
    {
        //여기에 엑셀로 아이템 값 받아 넣을 예정
    }
}