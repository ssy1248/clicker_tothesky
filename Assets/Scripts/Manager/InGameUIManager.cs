using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] 
    private GameObject titlePanel; // 타이틀 UI
    [SerializeField] 
    private GameObject inGameUI; // 남은 시간, 게이지 이미지
    [SerializeField] 
    private GameObject UIPercent; // 이동거리 이미지 바

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 완전히 로드된 직후에 호출됩니다
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded 호출됨");

        if (scene.name != "MainScene")
            return;

        // 너무 하드코딩인데
        if (titlePanel == null)
            titlePanel = GameObject.Find("SafeArea");
        if (inGameUI == null)
            inGameUI = GameObject.Find("InGameUI");
        if (UIPercent == null)
            UIPercent = GameObject.Find("UIPercentFill");

        bool fromShop = GlobalVariable.Instance.ShopCount > 0;

        //titlePanel.SetActive(!fromShop);
        UIPercent.SetActive(true);
        inGameUI.SetActive(true);
    }

}
