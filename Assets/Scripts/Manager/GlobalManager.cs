using UnityEngine;
using BigNumber;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GlobalManager : SingletonBehaviour<GlobalManager>
{
    public BigDouble kiwiAmount;

    public int clickLevel = 1;
    public int inGameCountTime = 120;

    //void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    //void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    //private void Start()
    //{
    //    // 처음에도 현재 씬 정보를 한 번 처리
    //    OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    //}

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    if (scene.name != "MainScene")
    //        return;

    //    // 게임 시작 플래그 or 상점 복귀 플래그
    //    bool showUI = GlobalVariable.Instance.GameStarted
    //               || GlobalVariable.Instance.ShopCount > 0;

    //    foreach (var go in UIObjects)
    //        go.SetActive(showUI);
    //}

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
