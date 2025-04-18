using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainContainer : MonoBehaviour
{
    public TMP_Text labelDesc;
    public TMP_Text labelCost;
    public TMP_Text labelLevel;
    
    public Button upgradeButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetData();
        
    }

    public void SetData()
    {
        labelDesc.text = $"키위 {GlobalManager.Instance.GetTouchAmount()}개";
        labelLevel.text = $"Lv.{GlobalManager.Instance.clickLevel}";
        labelCost.text = $"+{GlobalManager.Instance.GetUpgradeCost()}";
    }
    public void OnClickUpgrade()
    {
   
            Debug.Log(message: "Upgrade");
            GlobalManager.Instance.kiwiAmount -= GlobalManager.Instance.GetUpgradeCost();
            GlobalManager.Instance.clickLevel += 1;
            SetData();

    }

    void Update()
    {
        if (GlobalManager.Instance.kiwiAmount >= GlobalManager.Instance.GetUpgradeCost())
        {
            upgradeButton.interactable = true;
            ColorBlock colorBlock = upgradeButton.colors;
            colorBlock.normalColor = Color. yellow;
        }

        if (GlobalManager.Instance.kiwiAmount < GlobalManager.Instance.GetUpgradeCost())
        {
            upgradeButton.interactable = false;
            ColorBlock colorBlock = upgradeButton.colors;
            colorBlock.normalColor = Color.grey;
        }
    }
    
}
