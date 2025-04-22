using UnityEngine;
using BigNumber;
using UnityEngine.UI;

public class GlobalManager : SingletonBehaviour<GlobalManager>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
public BigDouble kiwiAmount;
public int speedGain=0;
public float decreaseFactor=1f;

public int clickLevel = 1;
public int inGameCountTime = 150;

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
