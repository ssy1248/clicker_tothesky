using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearPanel : MonoBehaviour
{
    [Header("UI 텍스트")]
    public TextMeshProUGUI clearTimeText;
    public TextMeshProUGUI collectCountText; // 수집품 텍스트를 인스펙터에서 연결

    [Header("UI 버튼")]
    public Button nextRoundButton; // 마지막 스테이지에서 비활성화하기 위해 연결

    [Header("스테이지 데이터베이스")]
    public StageDatabase stageDatabase;

    private void OnEnable()
    {
        // 1. 클리어 시간 표시
        float time = GlobalVariable.Instance.LastClearTime;
        int minutes = (int)time / 60;
        int secs = (int)time % 60;
        clearTimeText.text = $"{minutes:00}:{secs:00}";

        // 2. 수집품 개수 표시
        int currentStage = GlobalVariable.Instance.PlayerCurrentPlayerStage;
        int collected = GlobalVariable.Instance.GetCollectedCountByStage(currentStage + 1); // 스테이지 번호는 1부터 시작
        int max = GlobalVariable.Instance.StageMaxCollectCount;
        collectCountText.text = $"{collected} / {max}";

        // 3. 마지막 라운드일 경우 '다음' 버튼 비활성화
        int currentPlayStage = GlobalVariable.Instance.PlayerCurrentPlayerStage;
        // 데이터베이스의 전체 길이를 기준으로 마지막 스테이지인지 확인
        if (currentPlayStage >= stageDatabase.allStageData.Length - 1)
        {
            nextRoundButton.gameObject.SetActive(false);
        }
        else
        {
            nextRoundButton.gameObject.SetActive(true);
        }
    }

    // 'Home' 버튼을 눌렀을 때 호출될 함수
    public void OnClickHomeButton()
    {
        // 타이틀 씬으로 이동
        SceneManager.LoadScene("TitleScene");

        // UIManager가 있다면 특정 패널을 띄우는 것을 보장할 수 있습니다.
        // UIManager.Instance.ShowPanel(UIPanelType.LOGO_PANEL);
    }

    // 'NextRound' 버튼을 눌렀을 때 호출될 함수
    public void OnClickNextRoundButton()
    {
        int nextStageIndex = GlobalVariable.Instance.PlayerCurrentPlayerStage + 1;

        // 다음 스테이지 정보 세팅
        GlobalVariable.Instance.SetupStage(nextStageIndex, stageDatabase);

        // 상점 씬으로 이동
        SceneManager.LoadScene("ShopScene");

    }
}
