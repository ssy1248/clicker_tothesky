using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public Button RestartButton;
    public Button HomeButton;

    [Header("스테이지 데이터베이스")]
    public StageDatabase stageDatabase;

    // 스크립트가 활성화될 때 버튼에 리스너를 자동 등록
    private void OnEnable()
    {
        RestartButton.onClick.AddListener(PressRestart);
        HomeButton.onClick.AddListener(PressHome);
    }

    // 비활성화될 때 리스너를 해제 (메모리 누수 방지)
    private void OnDisable()
    {
        RestartButton.onClick.RemoveListener(PressRestart);
        HomeButton.onClick.RemoveListener(PressHome);
    }

    /// <summary>
    /// 게임을 다시 시작하는 함수
    /// </summary>
    public void PressRestart()
    {
        // 1. 현재 스테이지 인덱스를 가져옵니다.
        int currentStageIndex = GlobalVariable.Instance.PlayerCurrentPlayerStage;

        // 2. SetupStage 함수를 호출하여 현재 스테이지의 초기값으로 모든 상태를 리셋합니다.
        GlobalVariable.Instance.SetupStage(currentStageIndex, stageDatabase);

        // 3. 씬을 다시 로드합니다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 홈 화면으로 돌아가는 함수
    /// </summary>
    public void PressHome()
    {
        // "TitleScene"이라는 이름의 씬으로 이동합니다.
        SceneManager.LoadScene("TitleScene");
    }
}
