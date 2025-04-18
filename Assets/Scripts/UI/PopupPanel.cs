using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupPanel : BasePanel
{
    public Button buttonClose;
    public TMP_Text labelName;
    public TMP_Text labelDesc;
    
    private string nameMessage;
    private string descMessage;
    
    public override UIPanelType TypeOfPanel => UIPanelType.POPUP_PANEL;

    private void Awake()
    {
        buttonClose.onClick.RemoveListener(OnClickClose);
        buttonClose.onClick.AddListener(OnClickClose);
    }
    
    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);

        nameMessage = "null";
        descMessage = "null";
        
        if(datas.Length>0)
            nameMessage = (string)datas[0];
        
        if(datas.Length>1)
            descMessage = (string)datas[1];
        
        labelName.text = nameMessage;
        labelDesc.text = descMessage;
        
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

    }
}
