using UnityEngine;
using BigNumber;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GlobalManager : SingletonBehaviour<GlobalManager>
{
    public BigDouble kiwiAmount;

    public int clickLevel = 1;
    public float inGameCountTime = 120;

    private void Start()
    {
        // GlobalVariable 싱글톤 인스턴스가 존재하는지 안전하게 확인합니다.
        if (GlobalVariable.Instance != null)
        {
            // StagePanel에서 설정한 GameTime 값을 inGameCountTime에 할당합니다.
            inGameCountTime = GlobalVariable.Instance.GameTime;
        }
    }

    public BigDouble GetTouchAmount()
    {
        return clickLevel ;
    }
    
    public BigDouble GetUpgradeCost()
    {
        return clickLevel *10;
    }
    
    public BigDouble GetAssistUpgradeCost(int order, int lv)
    {
        BigDouble baseCost = 10 * BigDouble.Pow(2, order + 1);
        return BigDouble.Round(baseCost * BigDouble.Pow(1.2f, lv));
    }
    
    public BigDouble GetAssistAmount(int order, int lv)
    {
        BigDouble baseAmount = 1 * BigDouble.Pow(1.5f, order + 1);
        return BigDouble.Round(baseAmount * BigDouble.Pow(lv, 1.15f));
    }
}
