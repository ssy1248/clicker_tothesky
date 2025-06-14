using UnityEngine;
using BigNumber;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GlobalManager : SingletonBehaviour<GlobalManager>
{
    public BigDouble kiwiAmount;

    public int clickLevel = 1;
    public int inGameCountTime = 120;

    [Header("씬 전용 UI 오브젝트들")]
    public GameObject[] UIObjects;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // 처음에도 현재 씬 정보를 한 번 처리
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool fromShop = GlobalVariable.Instance.ShopCount > 0;

        // 타이틀(TitleScene) 진입 시 → UIObjects 모두 비활성화
        if (scene.name == "TitleScene")
        {
            foreach (var go in UIObjects)
                go.SetActive(false);
            return;
        }

        // 메인 게임 씬(MainScene) 진입 시 → ShopCount 여부에 따라 토글
        if (scene.name == "MainScene")
        {
            foreach (var go in UIObjects)
                go.SetActive(!fromShop);
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
