using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("UI 연결")]
    public GameObject pauseMenuUI; // 인스펙터에서 PopUpUI를 연결

    [Header("챕터 데이터베이스")]
    public ChapterDatabase chapterDatabase; // 인스펙터에서 연결

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 게임을 일시정지하고 메뉴를 띄웁니다.
    /// </summary>
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // 게임의 시간을 멈춤
    }

    /// <summary>
    /// 게임을 다시 재개합니다. (Resume 버튼에 연결)
    /// </summary>
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // 게임의 시간을 다시 흐르게 함
    }

    /// <summary>
    /// 홈 화면으로 돌아갑니다. (HOME 버튼에 연결)
    /// </summary>
    public void GoToHome()
    {
        Time.timeScale = 1f; // 씬을 바꾸기 전에는 항상 시간을 되돌려야 함
        SceneManager.LoadScene("TitleScene");
    }

    /// <summary>
    /// 현재 스테이지를 다시 시작합니다. (RESTART 버튼에 연결)
    /// </summary>
    public void RestartStage()
    {
        Time.timeScale = 1f; // 씬을 바꾸기 전에는 항상 시간을 되돌려야 함

        // 1. 현재 스테이지 인덱스를 가져옵니다.
        int currentStageIndex = GlobalVariable.Instance.PlayerCurrentPlayerStage;

        // 2. 현재 스테이지의 초기값으로 모든 상태를 리셋합니다.
        GlobalVariable.Instance.SetupStage(currentStageIndex, chapterDatabase);

        // 3. 아이템을 다시 고르기 위해 상점 씬으로 이동합니다.
        SceneManager.LoadScene("ShopScene");
    }
}
