using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AssistContainer : MonoBehaviour
{
    public TMP_Text labelDesc;
    public TMP_Text labelCost;
    public TMP_Text labelLevel;
    
    public Button upgradeButton;

    public int order;

    private int level=0;

    private float lasttimeupdate = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetData();
    }
    public void SetData()
    {
       
        labelDesc.text = $"초당 키위 {GlobalManager.Instance.GetAssistAmount(order,level)}개";
        labelLevel.text = $"Lv.{level}";
        labelCost.text = $"+{GlobalManager.Instance.GetAssistUpgradeCost(order,level)}";
    }

    public void EarnKiwi()
    {
        if (level<=0)
            return;
        lasttimeupdate += Time.deltaTime;
        if (lasttimeupdate >= 1f)
        {
            lasttimeupdate = 0f;
            GlobalManager.Instance.kiwiAmount += GlobalManager.Instance.GetAssistAmount(order, level);
        }
    }
    public void OnClickUpgrade()
    {
   
        Debug.Log(message: "Upgrade");
        GlobalManager.Instance.kiwiAmount -= GlobalManager.Instance.GetUpgradeCost();
        level += 1;
        SetData();

    }
    // Update is called once per frame
    void Update()
    {
        EarnKiwi();
        if (GlobalManager.Instance.kiwiAmount >= GlobalManager.Instance.GetAssistUpgradeCost(order,level))
        {
            upgradeButton.interactable = true;
            ColorBlock colorBlock = upgradeButton.colors;
            colorBlock.normalColor = Color. yellow;
        }

        if (GlobalManager.Instance.kiwiAmount < GlobalManager.Instance.GetAssistUpgradeCost(order, level))
        {
            upgradeButton.interactable = false;
            ColorBlock colorBlock = upgradeButton.colors;
            colorBlock.normalColor = Color.grey;
        }
    }
}
