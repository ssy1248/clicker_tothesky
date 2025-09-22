using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] 
    private GameObject inGameUI; // 남은 시간, 게이지 이미지

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
        if (scene.name != "MainScene")
            return;

        // 너무 하드코딩인데
        if (inGameUI == null)
            inGameUI = GameObject.Find("InGameUI");

        inGameUI.SetActive(true);
    }
}
