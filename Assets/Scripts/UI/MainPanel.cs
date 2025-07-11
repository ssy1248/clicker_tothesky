using UnityEngine;
using TMPro;
public class MainPanel : BasePanel
{
    public RectTransform containerRoot;

    public override UIPanelType TypeOfPanel => UIPanelType.MAIN_PANEL;

    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);
        //SetContainers();
        this.gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        this.gameObject.SetActive(false);
    }

    //public void SetContainers()
    //{
    //    GameObject mainObj = Instantiate(mainPref, Vector3.one, Quaternion.identity);
    //    mainObj.transform.SetParent(containerRoot);
    //    mainObj.transform.localScale = Vector3.one;

    //    for (int i = 0; i < 5; i++)
    //    {
    //        GameObject assistObj = Instantiate(assistPref, Vector3.one, Quaternion.identity);
    //        assistObj.transform.SetParent(containerRoot);
    //        assistObj.GetComponent<AssistContainer>().order = i + 1;
    //        assistObj.transform.localScale = Vector3.one;
    //    }
    //}
}
